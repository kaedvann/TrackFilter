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

        public void WriteTrack(Track track, string filename)
        {
            var xmlroot = new XElement("Tracks");
            var coords = new XElement("Coordinates");
            xmlroot.Add(coords);
            foreach (var coordinate in track.Coordinates)
            {
                coords.Add(new XElement("Coordinate", new XElement("Accuracy", coordinate.Accuracy),
                    new XElement("Azimuth", coordinate.Azimuth), new XElement("Longitude", coordinate.Longitude),
                    new XElement("Latitude", coordinate.Latitude), new XElement("Speed", coordinate.Speed),
                    new XElement("Time", coordinate.Time.ToString(DateTimeFormatInfo.InvariantInfo))));
            }
            xmlroot.Save(filename);
        }

        public Track ReadTrack(XElement coordinates)
        {
            try
            {
                var result = coordinates.Descendants("Coordinate").Select(n => new Coordinate
                {
                    Accuracy = double.Parse(n.Element("Accuracy").Value, NumberStyles.Any, CultureInfo.InvariantCulture),
                    Azimuth = double.Parse(n.Element("Azimuth").Value, NumberStyles.Any, CultureInfo.InvariantCulture),
                    Longitude =
                        double.Parse(n.Element("Longitude").Value, NumberStyles.Any, CultureInfo.InvariantCulture),
                    Latitude = double.Parse(n.Element("Latitude").Value, NumberStyles.Any, CultureInfo.InvariantCulture),
                    Speed = double.Parse(n.Element("Speed").Value, NumberStyles.Any, CultureInfo.InvariantCulture)/3.6,
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