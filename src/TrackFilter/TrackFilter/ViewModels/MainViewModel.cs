using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using Domain;
using Filter;
using Microsoft.Win32;
using TrackFilter.Services;
using TrackFilter.Utility;

namespace TrackFilter.ViewModels
{
    public class MainViewModel: PropertyChangedBase
    {
        private readonly INavigationService _navigationService;

        private readonly TrackProcessor _trackProcessor = new TrackProcessor();
        private Track _filterResult;
        private bool _stopsDetection = true;
        private bool _spikeDetection = true;
        private bool _filtering = true;
        public DelegateCommand LoadFileCommand { get; set; }

        public BindableCollection<Track> Tracks { get; set; }
        public DelegateCommand CloseCommand { get; set; }
        public DelegateCommand StartAnalysisCommand { get; set; }
        public DelegateCommand ExportTrackCommand { get; set; }
        public List<Track> SourceTracks { get; set; }

        public Track FilterResult
        {
            get { return _filterResult; }
            set
            {
                _filterResult = value;
                ExportTrackCommand.RaiseCanExecuteChanged();
            }
        }

        private void Close(object obj)
        {
            var window = obj as Window;
            if (window != null)
                window.Close();
        }
        public MainViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            Tracks = new BindableCollection<Track>();
            CreateCommands();
        }

        private void CreateCommands()
        {
            LoadFileCommand = new DelegateCommand(LoadFile);
            StartAnalysisCommand = new DelegateCommand(()=>_navigationService.Navigate(ViewType.AnalysisWindow));
            CloseCommand = new DelegateCommand(Close);
            ExportTrackCommand = new DelegateCommand(ExportTrack, () => FilterResult!=null);
            ShowSettingsCommand = new DelegateCommand(ShowSettings);
        }

        public DelegateCommand ShowSettingsCommand { get; set; }

        public bool Filtering
        {
            get { return _filtering; }
            set
            {
                if (value == _filtering) return;
                _filtering = value;
                NotifyOfPropertyChange(() => Filtering);
            }
        }

        public bool SpikeDetection
        {
            get { return _spikeDetection; }
            set
            {
                if (value == _spikeDetection) return;
                _spikeDetection = value;
                NotifyOfPropertyChange(() => SpikeDetection);
            }
        }

        public bool StopsDetection
        {
            get { return _stopsDetection; }
            set
            {
                if (value == _stopsDetection) return;
                _stopsDetection = value;
                NotifyOfPropertyChange(() => StopsDetection);
            }
        }

        private void ShowSettings()
        {
            _navigationService.Navigate(ViewType.SettingsWindow);
            ProcessTracks();
        }

        private void ExportTrack()
        {
            try
            {
                var dialog = new SaveFileDialog()
                {
                    Filter = "XML documents | *.xml",
                    FileName = "Result.xml",
                    Title = "Choose location to save result track"
                };
                var result = dialog.ShowDialog();
                if (result == true)
                {
                    var file = dialog.FileName;
                    var worker = new TrackXmlWorker();
                    worker.WriteTrack(FilterResult, file);
                }
            }
            catch
            {
                MessageBox.Show("Error occured while saving track");
            }
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
                    SourceTracks= worker.ReadTracks(file);
                    
                   
                    ProcessTracks();
                }
            }
            catch
            {
                MessageBox.Show("Bad file format");
            }
        }

        private void ProcessTracks()
        {
            if (SourceTracks != null)
            {
                _trackProcessor.KalmanEnabled = Filtering;
                _trackProcessor.StopsDetection = StopsDetection;
                _trackProcessor.SpikeDetection = SpikeDetection;
                Tracks.Clear();
                Tracks.AddRange(SourceTracks);

                FilterResult = _trackProcessor.ProcessTracks(SourceTracks);

                Tracks.Add(FilterResult);
                
            }
        }
    }
}
