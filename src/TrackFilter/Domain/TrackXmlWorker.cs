using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain
{
    public class TrackXmlWorker
    {
        public Track ReadTrack(string filename)
        {
            try
            {
                var doc = XDocument.Load(filename);
                var coordinates = doc.Element("Coordinates");
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
                    Coordinates = result.ToList()
                };
            }
            catch
            {
                throw new ArgumentException("Bad xml format");
            }
        }
}
}
