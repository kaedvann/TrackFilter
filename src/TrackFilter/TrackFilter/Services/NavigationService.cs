using System;
using Microsoft.Practices.Unity;
using TrackFilter.ViewModels;
using TrackFilter.Views;

namespace TrackFilter.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IUnityContainer _container;

        public NavigationService(IUnityContainer container)
        {
            _container = container;
        }

        public void Navigate(ViewType type)
        {
            switch (type)
            {
                case ViewType.MainWindow:
                    new MainWindow{DataContext = _container.Resolve<MainViewModel>()}.Show();
                    break;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
