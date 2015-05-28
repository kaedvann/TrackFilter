using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Analysis;
using Caliburn.Micro;
using Domain;
using Filter;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Series;
using TrackFilter.Utility;

namespace TrackFilter.ViewModels
{
    public class AnalysisViewModel: PropertyChangedBase
    {
        private Track _actualTrack;
        private Track _referenceTrack;
        private readonly Analyzer _analyzer = new Analyzer();
        private readonly TrackProcessor _trackProcessor = new TrackProcessor{StopsDetection = false, SpikeDetection = false};
        private PlotModel _plot;
        private List<Track> _tracks;
        private double _resultAverage;
        private double _sourceAverage;
        public DelegateCommand OpenReferenceCommand { get; set; }
        public DelegateCommand OpenActualCommand { get; set; }

        public PlotModel Plot
        {
            get { return _plot; }
            set
            {
                if (Equals(value, _plot)) return;
                _plot = value;
                NotifyOfPropertyChange(() => Plot);
            }
        }

        public AnalysisViewModel()
        {
            OpenReferenceCommand = new DelegateCommand(OpenReference);
            OpenActualCommand = new DelegateCommand(OpenActual);
            CloseCommand = new DelegateCommand(Close);
            Plot = new PlotModel();
            Tracks = new BindableCollection<Track>();
        }

        private void Close(object obj)
        {
            var window = obj as Window;
            if (window != null)
                window.Close();
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
                    _tracks = worker.ReadTracks(file);
                    _actualTrack = _tracks.First();
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

            var fullProcessor = new TrackProcessor();
            var fullResult = fullProcessor.ProcessTracks(_tracks);
            var trackDerivations =
                _tracks.Select(t => _analyzer.Derivations(t.Coordinates, _referenceTrack.Coordinates)).ToList();
            var fullResultDerivations =
                _analyzer.Derivations(fullResult.Coordinates, _referenceTrack.Coordinates).ToList();
            var sourceDerivations = new LineSeries();
            sourceDerivations.Points.AddRange(analysis.Select((a,i)=> new DataPoint(i, a.SourceDerivation)));
            sourceDerivations.Color = _actualTrack.Color.ToOxyColor();
            var resultDerivations = new LineSeries();
            resultDerivations.Points.AddRange(analysis.Select((a, i) => new DataPoint(i, a.ResultDerivation)));
            resultDerivations.Color = result.Color.ToOxyColor();

            SourceAverage = trackDerivations.SelectMany(s => s).Average();
            ResultAverage = fullResultDerivations.Average();
            Plot.Series.Clear();
            Plot.Series.Add(sourceDerivations);
            Plot.Series.Add(resultDerivations);
        }

        public double SourceAverage
        {
            get { return _sourceAverage; }
            set
            {
                if (value.Equals(_sourceAverage)) return;
                _sourceAverage = value;
                NotifyOfPropertyChange(() => SourceAverage);
            }
        }

        public double ResultAverage
        {
            get { return _resultAverage; }
            set
            {
                if (value.Equals(_resultAverage)) return;
                _resultAverage = value;
                NotifyOfPropertyChange(() => ResultAverage);
            }
        }

        public bool ReadyToAnalyse
        {
            get { return _actualTrack != null && _referenceTrack != null; }
            
        }

        public DelegateCommand CloseCommand { get; set; }
    }
    public static class ColorExtensions
{
        public static OxyColor ToOxyColor(this Color color)
        {
            return OxyColor.FromArgb(color.A, color.R, color.G, color.B);
        }
}
}
