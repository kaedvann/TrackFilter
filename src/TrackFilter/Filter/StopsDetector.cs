using System;
using System.Collections.Generic;
using System.Linq;
using Domain;

namespace Filter
{
    public class StopsDetector
    {
        public class CoordinateGroup
        {
            public List<Coordinate> Coordinates { get; set; }
            public double AzimuthVariance { get; set; }
            public double AzimuthMean { get; set; }
            public double SpeedVariance { get; set; }
            public double SpeedMean { get; set; }
            public bool IsStop { get; set; }
        }

        public double Threshold { get; set; }

        private const double MaxAzimuthVariance = 10;
        private const double MaxSpeedVariance = 5e-5;

        public List<List<Coordinate>> SplitSequence(List<Coordinate> track, double threshold)
        {
            var result = new List<List<Coordinate>>
            {
                new List<Coordinate>
                {
                    track.First()
                }
            };
            for (int i = 1; i < track.Count; i++)
            {
                if ((result.Last().First().Time - track[i].Time).TotalSeconds < threshold)
                    result.Last().Add(track[i]);
                else
                {
                    result.Add(new List<Coordinate>{track[i]});   
                }
            }
            return result;
        }

        public List<Coordinate> ComputeParametersByCoordinates(List<Coordinate> coordinates)
        {
            var result = new List<Coordinate>{coordinates.First()};
            result.AddRange(coordinates.Skip(1).Select((c, i) =>
            {
                var coord = c.Copy();
                c.Azimuth = CalculateAzimuth(coordinates[i], c);
                c.Speed = CalculateSpeed(coordinates[i], c);
                return coord;
            }));
            return result;
        }

        public double CalculateSpeed(Coordinate start, Coordinate end)
        {
            var x = end.Longitude - start.Longitude;
            var y = end.Latitude - end.Latitude;
            return Math.Sqrt(x*x + y*y)/(end.Time - start.Time).TotalSeconds;
        }

        public double CalculateAzimuth(Coordinate start, Coordinate end)
        {
            var x = end.Longitude - start.Longitude;
            var y = end.Latitude - end.Latitude;
            return Math.Acos(y / Math.Sqrt(x * x + y * y)) * 180 / Math.PI;           
        }

        public List<Coordinate> RemoveStops(List<Coordinate> track)
        {
            var groups = SplitSequence(track, Threshold).Select(CalculateStatistics).ToList();
            groups.ForEach(c => c.IsStop = IsStop(c));
            return groups.SelectMany(g => g.IsStop ? new List<Coordinate>
            {
                new Coordinate
                {
                    Latitude = g.Coordinates.Average(c => c.Latitude),
                    Longitude = g.Coordinates.Average(c => c.Longitude),
                    Azimuth = g.Coordinates.Average(c => c.Azimuth),
                    Accuracy = g.Coordinates.Average(c => c.Accuracy),
                    Speed = g.Coordinates.Average(c => c.Speed),
                    Time = g.Coordinates.First().Time.AddSeconds(g.Coordinates.Average(d => (d.Time - g.Coordinates.First().Time).TotalSeconds))
                },
                
            } :g.Coordinates).ToList();
        }

        private bool IsStop(CoordinateGroup coordinateGroup)
        {
            return coordinateGroup.AzimuthVariance > MaxAzimuthVariance && coordinateGroup.SpeedVariance > MaxSpeedVariance;
        }

        private CoordinateGroup CalculateStatistics(List<Coordinate> coordinates)
        {
            var temp = ComputeParametersByCoordinates(coordinates);
            var speed = MathNet.Numerics.Statistics.Statistics.MeanVariance(temp.Select(t => t.Speed));
            var azimuth = MathNet.Numerics.Statistics.Statistics.MeanVariance(temp.Select(t => t.Azimuth));
            return new CoordinateGroup{Coordinates = coordinates, SpeedMean = speed.Item1, SpeedVariance = speed.Item2, AzimuthMean = azimuth.Item1, AzimuthVariance = azimuth.Item2};
        }
    }
}
