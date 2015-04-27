using System;

namespace Domain
{
    public static class VincentyEllipsoid
    {
        public class Point
        {
            public double X;
            public double Y;
        }
        private const double A = 6378137;
        private const double B = 6356752.3142;
        private const double F = 1 / 298.257223563;


        /// <summary>
        /// Calculates new geographical point by supplied distance, direction and start point using Vincenty's formula 
        /// </summary>
        /// <param name="direction">Direction in degrees</param>
        /// <param name="distance">Distance in meters</param>
        /// <param name="currentLongtitude">Starting longitude</param>
        /// <param name="currentLatitude">Starting latitude</param>
        /// <returns>Calculated Poin</returns>
        public static Point GetPointFromDistance(double direction, double distance, double currentLongtitude, double currentLatitude)
        {
            double alpha1 = direction * Math.PI / 180.0;
            double s = distance;
            double lo1 = currentLongtitude;
            double la1 = currentLatitude;

            double lat1 = la1 * Math.PI / 180.0;
            double lon1 = lo1 * Math.PI / 180.0;

            if (alpha1 < 0)
            {
                alpha1 = alpha1 + 2 * Math.PI;
            }
            if (alpha1 > 2 * Math.PI)
            {
                alpha1 = alpha1 - 2 * Math.PI;
            }

            double tanU1, cosU1, sinU1, sigma1, cosAlpha1, sinAlpha1, sinAlpha,
                cosSqAlpha, uSq, a, b, sigma, cos2SigmaM, sinSigma, cosSigma, deltaSigma,
                sigmaP, tmp, lat2, lambda, C, L, lon2;

            cosAlpha1 = Math.Cos(alpha1);
            sinAlpha1 = Math.Sin(alpha1);

            tanU1 = (1 - F) * Math.Tan(lat1);
            cosU1 = 1 / Math.Sqrt((1 + tanU1 * tanU1));
            sinU1 = tanU1 * cosU1;
            sigma1 = Math.Atan2(tanU1, cosAlpha1);
            sinAlpha = cosU1 * sinAlpha1;
            cosSqAlpha = 1 - sinAlpha * sinAlpha;
            uSq = cosSqAlpha * (A * A - B * B) / (B * B);
            a = 1 + uSq / 16384 * (4096 + uSq * (-768 + uSq * (320 - 175 * uSq)));
            b = uSq / 1024 * (256 + uSq * (-128 + uSq * (74 - 47 * uSq)));
            sigma = s / (B * a); sigmaP = 2 * Math.PI;
            sinSigma = 1;
            cosSigma = 1;
            cos2SigmaM = 1;
            while (Math.Abs(sigma - sigmaP) > 1e-12)
            {
                cos2SigmaM = Math.Cos(2 * sigma1 + sigma);
                sinSigma = Math.Sin(sigma);
                cosSigma = Math.Cos(sigma);
                deltaSigma = b * sinSigma * 
                    (cos2SigmaM + b / 4 * 
                        (cosSigma * (-1 + 2 * cos2SigmaM * cos2SigmaM) -
                            b / 6 * cos2SigmaM * 
                            (-3 + 4 * sinSigma * sinSigma) * 
                            (-3 + 4 * cos2SigmaM * cos2SigmaM)
                        )
                     );

                sigmaP = sigma;
                sigma = s / (B * a) + deltaSigma;
            }
            tmp = sinU1 * sinSigma - cosU1 * cosSigma * cosAlpha1;
            lat2 = Math.Atan2(
                sinU1 * cosSigma + cosU1 * sinSigma * cosAlpha1,
                (1 - F) * Math.Sqrt(sinAlpha * sinAlpha + tmp * tmp)
                );

            lambda = Math.Atan2(
                sinSigma * sinAlpha1, cosU1 * cosSigma - sinU1 * sinSigma * cosAlpha1
                );

            C = F / 16 * cosSqAlpha * (4 + F * (4 - 3 * cosSqAlpha));
            L = lambda - (1 - C) * F * sinAlpha *
                (sigma + C * sinSigma * (cos2SigmaM + C * cosSigma * (-1 + 2 * cos2SigmaM * cos2SigmaM)));
            lon2 = (lon1 + L + 3 * Math.PI) % (2 * Math.PI) - Math.PI;

            double objectLatitude = lat2 * 180 / Math.PI;
            double objectLongitude = lon2 * 180 / Math.PI;


            Point answer = new Point
            {
                Y = objectLatitude,
                X = objectLongitude
            };
            return answer;
        }

    }
}