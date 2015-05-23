using System;
using Domain;

namespace Analysis
{
    public static class Utils
    {
        public static double PointLineDistance(Point point, Point start, Point end)
        {
            return Math.Abs(((end.X - start.X)*(point.Y-start.Y) - (end.Y-start.Y)*(point.X-start.X))/Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2)));
        }
    }
}
