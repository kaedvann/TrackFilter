using System;
using System.Windows;
using Microsoft.Practices.Unity;
using TrackFilter.ViewModels;
using TrackFilter.Views;

namespace TrackFilter.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IUnityContainer _container;
        private Window _mainWindow;
        public NavigationService(IUnityContainer container)
        {
            _container = container;
        }

        public void Navigate(ViewType type)
        {
            switch (type)
            {
                case ViewType.MainWindow:
                    _mainWindow = new MainWindow {DataContext = _container.Resolve<MainViewModel>()};
                    _mainWindow.Show();
                    break;
                case ViewType.AnalysisWindow:
                    var window = new AnalysisWindow
                    {
                        ShowInTaskbar = false,
                        DataContext = _container.Resolve<AnalysisViewModel>(),
                        Owner = _mainWindow
                    };
                    window.ShowDialog();
                    break;
                case ViewType.SettingsWindow:
                    var settingsWindow = new SettingsWindow
                    {
                        ShowInTaskbar = false,
                        DataContext = _mainWindow.DataContext,
                        Owner = _mainWindow
                    };
                    settingsWindow.ShowDialog();
                    break;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
