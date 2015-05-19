using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using Domain;
using Filter;
using Microsoft.Win32;
using TrackFilter.Utility;

namespace TrackFilter.ViewModels
{
    public class MainViewModel: PropertyChangedBase
    {
        public DelegateCommand LoadFileCommand { get; set; }

        public BindableCollection<Track> Tracks { get; set; }

        public MainViewModel()
        {
            Tracks = new BindableCollection<Track>();
            CreateCommands();
        }

        private void CreateCommands()
        {
            LoadFileCommand = new DelegateCommand(LoadFile);
        }

        private void LoadFile()
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "XML documents | *.xml",
                    Multiselect = false,
                    Title = "Choose xml file with track"
                };
                var result = dialog.ShowDialog();
                if (result == true)
                {
                    var combiner = new TrackCombiner(){Threshold = TimeSpan.FromSeconds(2.0)};
                    var file = dialog.FileName;
                    var worker = new TrackXmlWorker();
                    var tracks= worker.ReadTracks(file);
                    var track = combiner.Combine(tracks);
                    track.Color = Colors.Black;
                   
                    Tracks.Clear();
                    Tracks.Add(track);
                    var kalmanfilter = new KalmanFilter
                    {
                        AccelerationVariance = 3
                    };
                    var filter = new StopsDetector()
                    {
                        Threshold = 15
                    };
                    var spikes = new SpikeRemover();
                    var filtered = new Track
                    {
                        Coordinates = filter.RemoveStops(spikes.Process(track.Coordinates)),
                        Color = Colors.Red
                    };
                    var kalmanresult = new Track()
                    {
                        Coordinates = (kalmanfilter.Filter(filtered.Coordinates).ToList()).ToList(),
                        Color = Colors.BlueViolet
                    };
                    Tracks.Add(kalmanresult);
                   // Tracks.Add(filtered);
                }
            }
            catch
            {
                MessageBox.Show("Bad file format");
            }
        }
    }
}
