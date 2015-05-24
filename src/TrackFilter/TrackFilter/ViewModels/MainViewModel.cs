using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
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
                    
                   
                    Tracks.Clear();
                    Tracks.AddRange(SourceTracks);

                    FilterResult = _trackProcessor.ProcessTracks(SourceTracks);

                    Tracks.Add(FilterResult);
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
