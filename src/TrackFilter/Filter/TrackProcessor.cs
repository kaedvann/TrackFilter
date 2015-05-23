 using System;
 using System.CodeDom;
 using System.Collections.Generic;
 using System.Linq;
 using System.Windows.Media;
 using Domain;

namespace Filter
{
    public class TrackProcessor
    {
        private readonly KalmanFilter _kalman = new KalmanFilter();

        private readonly StopsDetector _stopsDetector = new StopsDetector();

        private readonly SpikeRemover _spikeRemover = new SpikeRemover();

        private readonly TrackCombiner _trackCombiner = new TrackCombiner();

        public bool KalmanEnabled { get; set; }
        public bool StopsDetection { get; set; }
        public bool SpikeDetection { get; set; }

        public double AccelerationVariance { get; set; }

        public double StopsTreshold { get; set; }
        public TimeSpan CombineTreshold { get; set; }

        public TrackProcessor()
        {
            KalmanEnabled = true;
            StopsDetection = true;
            SpikeDetection = true;
            AccelerationVariance = 3;
            StopsTreshold = 15;
            CombineTreshold = TimeSpan.FromSeconds(2);
        }
        public Track ProcessTracks(IList<Track> tracks)
        {
            _trackCombiner.Threshold = CombineTreshold;
            var combined = _trackCombiner.Combine(tracks);
            return ProcessTrack(combined);
        }

        public Track ProcessTrack(Track combined)
        {
            var withoutSpikes = SpikeDetection ? _spikeRemover.Process(combined.Coordinates) : combined.Coordinates;

            _stopsDetector.Threshold = StopsTreshold;
            var withoutStops = StopsDetection ? _stopsDetector.RemoveStops(withoutSpikes) : withoutSpikes;

            _kalman.AccelerationVariance = AccelerationVariance;
            var result = KalmanEnabled ? _kalman.Filter(withoutStops) : withoutStops;
            return  new Track{Coordinates = result.ToList(), Color = Colors.Purple};
        }
    }
}
