using System.Collections.Generic;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;

namespace TrackFilter.Views
{
    /// <summary>
    /// Interaction logic for MapControl.xaml
    /// </summary>
    public partial class MapControl
    {
        private  readonly PointLatLng _moscow = new PointLatLng(55.750303, 37.622656);

        public MapControl()
        {
            InitializeComponent();
            Map.MapProvider = GMapProviders.GoogleMap;
            GMapProvider.WebProxy = null;
            Map.MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;
            Map.CanDragMap = true;
            Map.Manager.Mode = AccessMode.ServerAndCache;
            Map.Zoom = 14;
            Map.Position = _moscow;
        }

        public void RenderTrack(IEnumerable<PointLatLng> points)
        {
            var marker = new GMapRoute(points);
            
            Map.Markers.Add(marker);
        }
    }
}
