using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Domain;

namespace GPRMCconverter
{
    public class GPRMCparser
    {
        private double _currentAccuracy = 500;
        public List<Coordinate> Parse(TextReader reader)
        {
            var result = new List<Coordinate>();
            string currentLine = null;
            while ((currentLine = reader.ReadLine()) != null)
            {
                if (currentLine.StartsWith(@"$GPRMC"))
                {
                    var res = Parse(currentLine);
                    if (res != null)
                        result.Add(res);
                }
                if (currentLine.StartsWith(@"$GPGGA"))
                {
                    _currentAccuracy = ParseAccuracy(currentLine);
                }
            }
           
            return result;
        }

        double ParseAccuracy(string line)
        {
            try
            {
                var pieces = line.Split(',');
                double result;
                var p = double.TryParse(pieces[8], NumberStyles.Any, CultureInfo.InvariantCulture, out result);
                return p ? result*2 : 500;
            }
            catch
            {
                return 500;
            }
        }
        double? ParseSpeed(string speed)
        {
            double result;
            var p = double.TryParse(speed, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
            return p ? result*0.514 : (double?)null;
        }

        double? ParseAzimuth(string azimuth)
        {
            double result;
            var p = double.TryParse(azimuth, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
            return p ? result : (double?)null;
        }

        DateTimeOffset ParseTime(string time, string date)
        {
            return new DateTimeOffset(int.Parse(date.Substring(4,2)), int.Parse(date.Substring(2, 2)), int.Parse(date.Substring(0,2)), int.Parse(time.Substring(0,2)), int.Parse(time.Substring(2,2)), int.Parse(time.Substring(4,2)), TimeSpan.FromHours(3));
        }

        private Coordinate Parse(string currentLine)
        {
            try
            {
                var pieces = currentLine.Split(',');
                if (pieces[2] != "A")
                    return null;
                return new Coordinate
                {
                    Latitude = (double.Parse(pieces[3].Substring(0,2)) + double.Parse(pieces[3].Substring(2),CultureInfo.InvariantCulture)/60.0) * (pieces[4]=="S"? -1:1),
                    Longitude = (double.Parse(pieces[5].Substring(0, 3)) + double.Parse(pieces[5].Substring(3), CultureInfo.InvariantCulture)/60.0) * (pieces[6] == "W" ? -1 : 1),
                    Speed = ParseSpeed(pieces[7])??0.0,
                    Azimuth = ParseAzimuth(pieces[8])??0.0,
                    Time = ParseTime(pieces[1], pieces[9]),
                    Accuracy = _currentAccuracy

                };
            }
            catch
            {
                return null;
            }
        }
    }
}
