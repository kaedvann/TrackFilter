﻿using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Domain.Extensions;
using MathNet.Numerics.LinearAlgebra;

namespace Filter
{
    /// <summary>
    /// Implementation of Kalman filter algorythm applying to coordinate sequence task
    /// </summary>
    public class KalmanFilter
    {
        private double AccelerationOx(Coordinate coordinate)
        {
            var point = VincentyEllipsoid.GetPointFromDistance(90, Math.Sqrt(AccelerationVariance), coordinate.Longitude,
                coordinate.Latitude);
            var res = point.X - coordinate.Longitude;
            return res*res;
        }

        private double AccelerationOy(Coordinate coordinate)
        {
            var point = VincentyEllipsoid.GetPointFromDistance(0, Math.Sqrt(AccelerationVariance), coordinate.Longitude,
                coordinate.Latitude);
            var res = point.Y - coordinate.Latitude;
            return res * res;
        }
        /// <summary>
        ///     Gets or sets variance of acceleration (σ^2) to be used in process noise covariance matrix computation
        /// </summary>
        public double AccelerationVariance { get; set; }

        /// <summary>
        ///     Applies Kalman filter to coordinate sequence
        /// </summary>
        /// <param name="input">
        ///     Input sequence of coordinates. Must be a complete prepared non-empty sequence without stops for
        ///     best results
        /// </param>
        /// <returns>Processed coordinate sequence with improved (so I hope, at least) accuracy</returns>
        public IList<Coordinate> Filter(IList<Coordinate> input)
        {
            if (input.Count == 0)
            {
                throw new ArgumentException("Coordinate sequence can't be empty");
            }
            var result = new List<Coordinate>(input.Count);
            var statePrevious = CreateState(input.First());
            result.Add(input.First());
            var pPrevious = Matrix<double>.Build.Dense(4, 4, 0.0);
            for (var i = 1; i < input.Count; ++i)
            {
                var dt = (input[i].Time - input[i - 1].Time).TotalSeconds;
                var transition = CreateStateTransition(dt);
                var stateEstimation = transition.Multiply(statePrevious);
                var processNoiseCovariance = CalculateProcessNoiseCovariance(dt, input[i]);
                var pEstimation = CalculateErrorEstimation(pPrevious, transition, processNoiseCovariance);
                var measureTransition = CreateMeasureTransition();
                var measureError = CalculateMeasureErrorCovariance(input[i]);
                var kalmanGain = CalculateKalmanGain(pEstimation, measureTransition, measureError);
                var measureCurrent = CreateState(input[i]);
                var stateCurrent = CorrectCurrentState(stateEstimation, measureCurrent, kalmanGain, measureTransition);
                var pCurrent = CorrectCurrentError(pEstimation, kalmanGain, measureTransition);
                result.Add(StateToCoordinate(stateCurrent, input[i]));
                statePrevious = stateCurrent;
                pPrevious = pCurrent;
            }
            return result;
        }

        /// <summary>
        /// Converts state vector to a coordinate
        /// </summary>
        /// <param name="state">4x1 matrix {x, y, vx, vy}T</param>
        /// <param name="coordinate">Original coordinate</param>
        /// <returns>Constructed coordinate</returns>
        private Coordinate StateToCoordinate(Matrix<double> state, Coordinate coordinate)
        {
            if (state.RowCount!=4 || state.ColumnCount!=1)
                throw new ArgumentException("Not a state vector");
            return new Coordinate{Longitude = state[0,0],Latitude = state[1,0], Time = coordinate.Time, Accuracy = coordinate.Accuracy, Azimuth = coordinate.Azimuth, Speed = coordinate.Speed};
        }

        /// <summary>
        /// Corrects predicted error matrix
        /// </summary>
        /// <param name="errorEstimation">Error matrix estimation</param>
        /// <param name="kalmanGain">Kalman gain for this step</param>
        /// <param name="measureTransition">Measure transtion matrix</param>
        /// <returns>Corrected error matrix</returns>
        private Matrix<double> CorrectCurrentError(Matrix<double> errorEstimation, Matrix<double> kalmanGain, Matrix<double> measureTransition)
        {
            return (Matrix<double>.Build.DenseIdentity(4) - kalmanGain*measureTransition)*errorEstimation;

        }

        /// <summary>
        /// Corrects predicted state matrix
        /// </summary>
        /// <param name="stateEstimation">State estimation</param>
        /// <param name="measureCurrent">Current measured state</param>
        /// <param name="kalmanGain">Kalman gain</param>
        /// <param name="measureTransition">Measure transition matrix</param>
        /// <returns>Corrected state</returns>
        private Matrix<double> CorrectCurrentState(Matrix<double> stateEstimation, Matrix<double> measureCurrent, Matrix<double> kalmanGain, Matrix<double> measureTransition)
        {
            return stateEstimation + kalmanGain.Multiply(measureCurrent - measureTransition*stateEstimation);
        }

        private Matrix<double> CalculateKalmanGain(Matrix<double> errorEstimation, Matrix<double> measureTransition, Matrix<double> measureError)
        {
            var inverse = (measureTransition.Multiply(errorEstimation).Multiply(measureTransition.Transpose()).Add(measureError)).Inverse();
            return
                errorEstimation.Multiply(measureTransition.Transpose())
                    .Multiply(
                        inverse);
        }

        private Matrix<double> CalculateMeasureErrorCovariance(Coordinate coordinate)
        {
            //TODO actual error matrix calculation
            var speedx = VincentyEllipsoid.GetPointFromDistance(90, 0.0005, coordinate.Longitude, coordinate.Latitude);
            var speedy = VincentyEllipsoid.GetPointFromDistance(0, 0.0005, coordinate.Longitude, coordinate.Latitude);
            return Matrix<double>.Build.DenseOfArray(new[,]
            {
                {coordinate.AccuracyOx()/50,0,0,0},
                {0,coordinate.AccuracyOy()/50,0,0},
                {0,0,(speedx.X - coordinate.Longitude)/10,0},
                {0,0,0,(speedy.Y - coordinate.Latitude)/10}
            });
        }

        /// <summary>
        /// Creates measure transition matrix
        /// </summary>
        /// <returns>Measure transition matrix</returns>
        private Matrix<double> CreateMeasureTransition()
        {
            return Matrix<double>.Build.DenseIdentity(4);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="previousError"></param>
        /// <param name="transition"></param>
        /// <param name="processNoiseCovariance"></param>
        /// <returns></returns>
        private Matrix<double> CalculateErrorEstimation(Matrix<double> previousError, Matrix<double> transition, Matrix<double> processNoiseCovariance)
        {
            return transition.Multiply(previousError).Multiply(transition.Transpose()).Add(processNoiseCovariance);
        }

        /// <summary>
        /// Calculates Q matrix from equation Q = G*G^t*σ^2
        /// </summary>
        /// <param name="dt">Current coordinate period</param>
        /// <param name="coordinate">Coordinate to calculate acceleration noise against</param>
        /// <returns>Q matrix</returns>
        private Matrix<double> CalculateProcessNoiseCovariance(double dt, Coordinate coordinate)
        {
            var g = Matrix<double>.Build.DenseOfArray(new[,]
            {
                {dt*dt/2.0, 0},
                {0, dt*dt/2.0},
                {dt, 0},
                {0, dt}
            });
            var accelerationMatrix = Matrix<double>.Build.DenseOfArray(new[,]
            {
                {AccelerationOx(coordinate), 0},
                {0, AccelerationOy(coordinate)}
            });
            return g.Multiply(accelerationMatrix).Multiply(g.Transpose());
        }

        /// <summary>
        ///     Creates matrix A 4x4 in the following way:
        ///     |1 0 dt 0|
        ///     |0 1 0 dt|
        ///     |0 0 1  0|
        ///     |0 0 0  1|
        /// </summary>
        /// <param name="dt">Current coordinate period</param>
        /// <returns>A matrix</returns>
        private Matrix<double> CreateStateTransition(double dt)
        {
            return Matrix<double>.Build.DenseOfArray(new[,]
            {
                {1, 0, dt, 0},
                {0, 1, 0, dt},
                {0, 0, 1, 0},
                {0, 0, 0, 1}
            });
        }

        /// <summary>
        ///     Initialize state vector from coordinate
        /// </summary>
        /// <param name="coordinate">First coordinate in sequence</param>
        /// <returns>4x1 matrix {x, y, vx, vy}T</returns>
        private Matrix<double> CreateState(Coordinate coordinate)
        {
            var result = Matrix<double>.Build.Dense(4, 1);
            result[0, 0] = coordinate.Longitude;
            result[1, 0] = coordinate.Latitude;
            result[2, 0] = coordinate.SpeedOx();
            result[3, 0] = coordinate.SpeedOy();
            return result;
        }
    }
}