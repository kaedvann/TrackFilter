using System;
using Domain;

namespace Analysis
{
    public static class Utils
    {
        public static double PointDistance(Point start, Point end)
        {
            return Math.Sqrt(Math.Pow(start.X - end.X, 2) + Math.Pow(start.Y - end.Y, 2));
        }

        public static double TriangleCosAlpha(double a, double b, double c)
        {
            return (b*b + c*c - a*a)/(2*b*c);
        }

        public static double PointLineDistance(Point point, Point start, Point end)
        {
            var h =  Math.Abs(((end.X - start.X)*(point.Y-start.Y) - (end.Y-start.Y)*(point.X-start.X))/Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2)));
            var ab = PointDistance(start, point);
            var bc = PointDistance(point, end);
            var ac = PointDistance(start, end);
            var alpha = TriangleCosAlpha(bc, ab, ac);
            var beta = TriangleCosAlpha(ac, ab, bc);
            var gamma = TriangleCosAlpha(ab, bc, ac);
            if (alpha < 0 || gamma < 0)
                return ab > bc ? bc : ab;
            return h;
        }
    }
}
