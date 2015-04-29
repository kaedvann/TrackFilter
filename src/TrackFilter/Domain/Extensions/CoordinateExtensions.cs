namespace Domain.Extensions
{
    public static class CoordinateExtensions
    {
        public static double SpeedOx(this Coordinate coordinate)
        {
            var point = VincentyEllipsoid.GetPointFromDistance(coordinate.Azimuth, coordinate.Speed, coordinate.Longitude,
                coordinate.Latitude);
            return point.X - coordinate.Longitude;
        }

        public static double SpeedOy(this Coordinate coordinate)
        {
            var point = VincentyEllipsoid.GetPointFromDistance(coordinate.Azimuth, coordinate.Speed, coordinate.Longitude,
                coordinate.Latitude);
            return point.Y - coordinate.Latitude;
        }

        public static double AccuracyOx(this Coordinate coordinate)
        {
            var point = VincentyEllipsoid.GetPointFromDistance(90, coordinate.Accuracy, coordinate.Longitude,
                coordinate.Latitude);
            return point.X - coordinate.Longitude;
        }
        
        public static double AccuracyOy(this Coordinate coordinate)
        {
            var point = VincentyEllipsoid.GetPointFromDistance(0, coordinate.Accuracy, coordinate.Longitude,
                coordinate.Latitude);
            return point.Y - coordinate.Latitude;
        }
    }
}
