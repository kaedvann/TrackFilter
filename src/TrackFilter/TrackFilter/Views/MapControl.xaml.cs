using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
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
                if (PointsEnabled)
                {
                    var markers = track.Coordinates.Select(c => new GMapMarker(new PointLatLng(c.Latitude, c.Longitude))
                    {
                        Shape = new Ellipse()
                        {
                            Fill = new SolidColorBrush(track.Color),
                            Height = 6,
                            Width = 6,
                            Margin = new Thickness(-3, -3, 0, 0)

                        }
                    });
                    foreach (var marker in markers)
                    {
                        Map.Markers.Add(marker);
                    }
                }
                Map.Markers.Add(route);
                route.RegenerateShape(Map);
                var path = route.Shape as Path;
                if (path != null)
                {
                    path.Effect = null;
                    path.Stroke = new SolidColorBrush(track.Color);
                    path.StrokeThickness = 2;
                    path.ToolTip = String.Format("{0} track, {1} points", track.Name, track.Coordinates.Count);
                    path.IsManipulationEnabled = true;
                    path.IsHitTestVisible = true;

                }
            }
        }





        public bool PointsEnabled
        {
            get { return (bool)GetValue(PointsEnabledProperty); }
            set { SetValue(PointsEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PointsEnabledProperty =
            DependencyProperty.Register("PointsEnabled", typeof(bool), typeof(MapControl), new PropertyMetadata(default(bool), PointsEnabledCallback));

        private static void PointsEnabledCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var mapControl = (dependencyObject as MapControl);
            mapControl.RenderTracks();
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
