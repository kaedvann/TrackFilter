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
            public double LongitudeVariance { get; set; }
            public double LatitudeVariance { get; set; }
        }

        public double Threshold { get; set; }


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
                if ((track[i].Time - result.Last().First().Time).TotalSeconds < threshold)
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
                coord.Azimuth = CalculateAzimuth(coordinates[i], c);
                coord.Speed = CalculateSpeed(coordinates[i], c);
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
            var result =  Math.Acos(y / Math.Sqrt(x * x + y * y)) * 180 / Math.PI;
            return double.IsNaN(result) ? 0 : result;
        }

        public List<Coordinate> RemoveStops(List<Coordinate> track)
        {
            var groups = SplitSequence(track, Threshold).Select(CalculateStatistics).ToList();
            groups.ForEach(c => c.IsStop = IsStop(c));
            var squashed = SquashStops(groups);
            return (squashed.SelectMany(g => g.IsStop ? new List<Coordinate>
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
                
            } :g.Coordinates).ToList());
        }

        private List<CoordinateGroup> SquashStops(List<CoordinateGroup> groups)
        {
            var result = new List<CoordinateGroup>();
            var firstStops = groups.TakeWhile(g => g.IsStop).ToList();
            if (firstStops.Count != 0)
                result.Add(MergeStops(firstStops));
            result.Add(groups[firstStops.Count]);
            for (int i = firstStops.Count + 1; i < groups.Count; ++i)
            {
                if (!groups[i].IsStop)
                    result.Add(groups[i]);
                else
                {
                    int j = i;
                    while (j != groups.Count && groups[j].IsStop)
                    {
                        j++;
                    }
                    var sublist = groups.GetRange(i, j - i);
                    result.Add(MergeStops(sublist));
                    i = j - 1;
                }
            }
            return result;
        }

        private CoordinateGroup MergeStops(IEnumerable<CoordinateGroup> stops)
        {
            var result = new CoordinateGroup()
            {
                Coordinates = new List<Coordinate>(),
                IsStop = true
            };
            foreach (var coordinateGroup in stops)
            {
                result.Coordinates.AddRange(coordinateGroup.Coordinates);
            }
            return result;
        }
        private const double MaxAzimuthVariance = 50000;
        private const double MaxSpeedVariance = 0;
        private const double MaxLatLongVariance = 0.1e-10;
        private bool IsStop(CoordinateGroup coordinateGroup)
        {
            return 
                (coordinateGroup.AzimuthVariance > MaxAzimuthVariance &&
                   coordinateGroup.SpeedVariance > MaxSpeedVariance) ||
              (coordinateGroup.LatitudeVariance < MaxLatLongVariance && coordinateGroup.LongitudeVariance<MaxLatLongVariance);
        }

        private CoordinateGroup CalculateStatistics(List<Coordinate> coordinates)
        {
            var temp = ComputeParametersByCoordinates(coordinates);
            var speed = MathNet.Numerics.Statistics.Statistics.MeanVariance(temp.Select(t => t.Speed));
            var azimuth = MathNet.Numerics.Statistics.Statistics.MeanVariance(temp.Select(t => t.Azimuth));
            var latitude = MathNet.Numerics.Statistics.Statistics.MeanVariance(temp.Select(t => t.Latitude));
            var longitude = MathNet.Numerics.Statistics.Statistics.MeanVariance(temp.Select(t => t.Longitude));
            return new CoordinateGroup 
            {
                Coordinates = coordinates, 
                SpeedMean = speed.Item1, 
                SpeedVariance = speed.Item2, 
                AzimuthMean = azimuth.Item1, 
                AzimuthVariance = azimuth.Item2,
                LatitudeVariance = latitude.Item2,
                LongitudeVariance = longitude.Item2
            };
        }
    }
}
