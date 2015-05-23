using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using System.Xml.Linq;

namespace Domain
{
    public class TrackXmlWorker
    {
        private readonly Random _rand = new Random();
        public List<Track> ReadTracks(string filename)
        {
            var doc = XDocument.Load(filename);
            return doc.Descendants("Coordinates").Select(ReadTrack).ToList();
        }
        public Track ReadTrack(XElement coordinates)
        {
            try
            {
                var result = coordinates.Descendants("Coordinate").Select(n => new Coordinate
                {
                    Accuracy = double.Parse(n.Element("Accuracy").Value, NumberStyles.Any, CultureInfo.InvariantCulture),
                    Azimuth = double.Parse(n.Element("Azimuth").Value, NumberStyles.Any, CultureInfo.InvariantCulture),
                    Longitude = double.Parse(n.Element("Longitude").Value, NumberStyles.Any, CultureInfo.InvariantCulture),
                    Latitude = double.Parse(n.Element("Latitude").Value, NumberStyles.Any, CultureInfo.InvariantCulture),
                    Speed = double.Parse(n.Element("Speed").Value, NumberStyles.Any, CultureInfo.InvariantCulture) / 3.6,
                    Time = DateTimeOffset.Parse(n.Element("Time").Value, DateTimeFormatInfo.InvariantInfo)
                });
                return new Track
                {
                    Coordinates = result.ToList(),
                    Color = Color.FromRgb((byte) _rand.Next(256), (byte) _rand.Next(256), (byte) _rand.Next(256))
                };
            }
            catch
            {
                throw new ArgumentException("Bad xml format");
            }
        }
}
}
