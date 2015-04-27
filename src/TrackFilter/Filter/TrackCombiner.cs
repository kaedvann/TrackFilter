using System;
using System.Collections.Generic;
using System.Linq;
using Domain;

namespace Filter
{
    public class TrackCombiner
    {
        public TimeSpan Treshold { get; set; }

        private class IndexedTrack
        {
            public IList<Coordinate> Coordinates;
            public int Index;
        }
        public Track Combine(IList<Track> tracks)
        {
            var result = new Track();
            var indexedTracks = tracks.Select(t => new IndexedTrack{Coordinates = t.Coordinates, Index = 0}).ToArray();
            while (indexedTracks.All(t => t.Coordinates.Count != t.Index))
            {
                var currentTime = indexedTracks.Min(t => t.Coordinates[t.Index].Time);
                var coordinatesToCombine =
                    indexedTracks.Where(
                        t => t.Index < t.Coordinates.Count && (t.Coordinates[t.Index].Time - currentTime) < Treshold)
                        .Select(t => t.Coordinates[t.Index++])
                        .ToArray();
                var combined = Combine(coordinatesToCombine);
                result.Coordinates.Add(combined);
            }
            return result;
        }

        private Coordinate Combine(Coordinate[] coordinatesToCombine)
        {
            var accuracySum = coordinatesToCombine.Sum(c => c.Accuracy);
            var weightSum = coordinatesToCombine.Sum(c => accuracySum - c.Accuracy);
            var times = coordinatesToCombine.Select(c => c.Time).OrderBy(t => t);
            var result = new Coordinate
            {
                Latitude = coordinatesToCombine.Sum(c => (accuracySum - c.Accuracy) * c.Latitude / weightSum),
                Longitude = coordinatesToCombine.Sum(c => (accuracySum - c.Accuracy) * c.Longitude / weightSum),
                Time = times.First().AddSeconds(times.Average(d => (d - times.First()).TotalSeconds)),
                Azimuth = coordinatesToCombine.Average(c => c.Azimuth),
                Speed = coordinatesToCombine.Average(c => c.Speed)

            };
            

            return result;
        }
    }
}
