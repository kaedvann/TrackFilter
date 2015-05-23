using System;
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
        public DelegateCommand LoadFileCommand { get; set; }

        public BindableCollection<Track> Tracks { get; set; }

        public DelegateCommand StartAnalysisCommand { get; set; }

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
                    var tracks= worker.ReadTracks(file);
                    
                   
                    Tracks.Clear();
                    Tracks.AddRange(tracks);

                    var filterResult = _trackProcessor.ProcessTracks(tracks);

                    Tracks.Add(filterResult);
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
