using System;
using System.Collections.Generic;
using System.Linq;
using Domain;

namespace Filter
{
    public class SpikeRemover
    {
        private const double MaxDegSpeed = 0.0008;// 47e-5;
        public List<Coordinate> Process(List<Coordinate> track)
        {
            var result = new List<Coordinate>{track.First()};
            for(int i = 1; i < track.Count-1; i++)
                if (!Spike(result.Last(), track[i], track[i+1]))
                    result.Add(track[i]);
            return result;
        }

        private bool Spike(Coordinate previous, Coordinate current, Coordinate next)
        {
            var dist =
                Math.Sqrt(Math.Pow(current.Latitude - previous.Latitude, 2) +
                          Math.Pow(current.Longitude - previous.Longitude, 2));
            var nextDist =                 Math.Sqrt(Math.Pow(next.Latitude - previous.Latitude, 2) +
                          Math.Pow(next.Longitude - previous.Longitude, 2));
            return dist/(current.Time - previous.Time).TotalSeconds > MaxDegSpeed || dist >10*nextDist;
        }
    }
}
