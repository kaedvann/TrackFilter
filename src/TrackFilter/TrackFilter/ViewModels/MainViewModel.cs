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
                    var file = dialog.FileName;
                    var worker = new TrackXmlWorker();
                    var track = worker.ReadTrack(file);
                    track.Color = Colors.Black;
                    Tracks.Clear();
                    Tracks.Add(track);
                    var filter = new KalmanFilter();
                    filter.AccelerationVariance = 2;
                    var filtered = new Track {Coordinates = filter.Filter(track.Coordinates).ToList()};
                    filtered.Color = Colors.Red;
                    Tracks.Add(filtered);
                }
            }
            catch
            {
                MessageBox.Show("Bad file format");
            }
        }
    }
}
