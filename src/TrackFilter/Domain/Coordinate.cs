using System;

namespace Domain
{
    public class Coordinate
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public DateTimeOffset Time { get; set; }
    }
}
