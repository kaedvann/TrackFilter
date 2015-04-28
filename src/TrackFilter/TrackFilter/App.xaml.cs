using System.Windows;
using Microsoft.Practices.Unity;
using TrackFilter.Services;

namespace TrackFilter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private readonly IUnityContainer _container = new UnityContainer();

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            RegisterDependencies();
            _container.Resolve<INavigationService>().Navigate(ViewType.MainWindow);
        }

        private void RegisterDependencies()
        {
            _container.RegisterType<INavigationService, NavigationService>(new ContainerControlledLifetimeManager());
            _container.RegisterInstance(_container);
        }
    }
}
