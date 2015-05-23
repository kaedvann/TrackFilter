using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Analysis;
using Caliburn.Micro;
using Domain;
using Filter;
using Microsoft.Win32;
using TrackFilter.Utility;

namespace TrackFilter.ViewModels
{
    public class AnalysisViewModel: PropertyChangedBase
    {
        private Track _actualTrack;
        private Track _referenceTrack;
        private readonly Analyzer _analyzer = new Analyzer();
        private readonly TrackProcessor _trackProcessor = new TrackProcessor();
        public DelegateCommand OpenReferenceCommand { get; set; }
        public DelegateCommand OpenActualCommand { get; set; }

        public AnalysisViewModel()
        {
            OpenReferenceCommand = new DelegateCommand(OpenReference);
            OpenActualCommand = new DelegateCommand(OpenActual);

            Tracks = new BindableCollection<Track>();
        }

        public BindableCollection<Track> Tracks { get; set; }

        private void OpenActual()
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
                    var tracks = worker.ReadTracks(file);
                    _actualTrack = tracks.First();
                    if (ReadyToAnalyse)
                        PerformAnalysis();
                }
            }
            catch
            {
                MessageBox.Show("Bad file format");
            }
        }
        private void OpenReference()
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
                    var tracks = worker.ReadTracks(file);
                    _referenceTrack = tracks.First();
                    if (ReadyToAnalyse)
                        PerformAnalysis();
                }
            }
            catch
            {
                MessageBox.Show("Bad file format");
            }
        }

        private void PerformAnalysis()
        {
            var result = _trackProcessor.ProcessTrack(_actualTrack);
            var analysis = _analyzer.Analyze(_actualTrack.Coordinates, result.Coordinates, _referenceTrack.Coordinates);
            Tracks.Clear();
            _referenceTrack.Color = Colors.DarkBlue;
            _actualTrack.Color = Colors.SeaGreen;
            result.Color = Colors.Red;
            Tracks.Add(result);
            Tracks.Add(_referenceTrack);
            Tracks.Add(_actualTrack);
        }

        public bool ReadyToAnalyse
        {
            get { return _actualTrack != null && _referenceTrack != null; }
            
        }

    }
}
