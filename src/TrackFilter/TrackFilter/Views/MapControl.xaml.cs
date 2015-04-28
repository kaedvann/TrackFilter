using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using Domain;
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

        public static readonly DependencyProperty TracksProperty = DependencyProperty.Register(
            "Tracks", typeof (ICollection<Track>), typeof (MapControl), new PropertyMetadata(default(ICollection<Track>), TrackChangedCallback));

        private static void TrackChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var mapControl = (dependencyObject as MapControl);
            var oldCOllection = dependencyPropertyChangedEventArgs.OldValue as INotifyCollectionChanged;
            if (oldCOllection != null)
                oldCOllection.CollectionChanged -= mapControl.TracksChanged;
            var collection = dependencyPropertyChangedEventArgs.NewValue as INotifyCollectionChanged;
            if (collection != null)
                collection.CollectionChanged += mapControl.TracksChanged;
        }

        private void TracksChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RenderTracks();
        }

        private void RenderTracks()
        {
            Map.Markers.Clear();
            foreach (var track in Tracks)
            {
                var route = new GMapRoute(track.Coordinates.Select(c => new PointLatLng(c.Latitude, c.Longitude)));
                Map.Markers.Add(route);
                route.RegenerateShape(Map);
            }
        }

        public ICollection<Track> Tracks
        {
            get { return (ICollection<Track>) GetValue(TracksProperty); }
            set { SetValue(TracksProperty, value); }
        }

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

    }
}
