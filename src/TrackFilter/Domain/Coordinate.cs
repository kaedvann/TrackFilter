using System;

namespace Domain
{
    public class Coordinate
    {
        /// <summary>
        /// Longitude of the point in degrees
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Latitude of the point in degrees
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// The moment in time when the coordinate was received
        /// </summary>
        public DateTimeOffset Time { get; set; }

        /// <summary>
        /// Accuracy of the coordinate in meteres
        /// </summary>
        public double Accuracy { get; set; }

        /// <summary>
        /// Azimuth
        /// </summary>
        public double Azimuth { get; set; }

        /// <summary>
        /// Speed in metres per second
        /// </summary>
        public double Speed { get; set; }

        /// <summary>
        /// Accuracy in degrees
        /// </summary>
        public double SphereAccuracy { get; set; }
    }
}
