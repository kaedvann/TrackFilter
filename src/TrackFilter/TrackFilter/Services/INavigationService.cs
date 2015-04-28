namespace TrackFilter.Services
{
    public interface INavigationService
    {
        void Navigate(ViewType type);
    }

    public enum ViewType
    {
        MainWindow
    }
}