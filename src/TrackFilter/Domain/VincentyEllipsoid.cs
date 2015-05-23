using System;

namespace Domain
{
    public static class VincentyEllipsoid
    {
        private const double A = 6378137;
        private const double B = 6356752.3142;
        private const double F = 1/298.257223563;

        /// <summary>
        ///     Calculates new geographical point by supplied distance, direction and start point using Vincenty's formula
        /// </summary>
        /// <param name="direction">Direction in degrees</param>
        /// <param name="distance">Distance in meters</param>
        /// <param name="currentLongtitude">Starting longitude</param>
        /// <param name="currentLatitude">Starting latitude</param>
        /// <returns>Calculated Poin</returns>
        public static Point GetPointFromDistance(double direction, double distance, double currentLongtitude,
            double currentLatitude)
        {
            var alpha1 = direction*Math.PI/180.0;
            var s = distance;
            var lo1 = currentLongtitude;
            var la1 = currentLatitude;

            var lat1 = la1*Math.PI/180.0;
            var lon1 = lo1*Math.PI/180.0;

            if (alpha1 < 0)
            {
                alpha1 = alpha1 + 2*Math.PI;
            }
            if (alpha1 > 2*Math.PI)
            {
                alpha1 = alpha1 - 2*Math.PI;
            }

            var cosAlpha1 = Math.Cos(alpha1);
            var sinAlpha1 = Math.Sin(alpha1);

            var tanU1 = (1 - F)*Math.Tan(lat1);
            var cosU1 = 1/Math.Sqrt((1 + tanU1*tanU1));
            var sinU1 = tanU1*cosU1;
            var sigma1 = Math.Atan2(tanU1, cosAlpha1);
            var sinAlpha = cosU1*sinAlpha1;
            var cosSqAlpha = 1 - sinAlpha*sinAlpha;
            var uSq = cosSqAlpha*(A*A - B*B)/(B*B);
            var a = 1 + uSq/16384*(4096 + uSq*(-768 + uSq*(320 - 175*uSq)));
            var b = uSq/1024*(256 + uSq*(-128 + uSq*(74 - 47*uSq)));
            var sigma = s/(B*a);
            var sigmaP = 2*Math.PI;
            double sinSigma = 1;
            double cosSigma = 1;
            double cos2SigmaM = 1;
            while (Math.Abs(sigma - sigmaP) > 1e-12)
            {
                cos2SigmaM = Math.Cos(2*sigma1 + sigma);
                sinSigma = Math.Sin(sigma);
                cosSigma = Math.Cos(sigma);
                var deltaSigma = b*sinSigma*
                                 (cos2SigmaM + b/4*
                                  (cosSigma*(-1 + 2*cos2SigmaM*cos2SigmaM) -
                                   b/6*cos2SigmaM*
                                   (-3 + 4*sinSigma*sinSigma)*
                                   (-3 + 4*cos2SigmaM*cos2SigmaM)
                                      )
                                     );

                sigmaP = sigma;
                sigma = s/(B*a) + deltaSigma;
            }
            var tmp = sinU1*sinSigma - cosU1*cosSigma*cosAlpha1;
            var lat2 = Math.Atan2(
                sinU1*cosSigma + cosU1*sinSigma*cosAlpha1,
                (1 - F)*Math.Sqrt(sinAlpha*sinAlpha + tmp*tmp)
                );

            var lambda = Math.Atan2(
                sinSigma*sinAlpha1, cosU1*cosSigma - sinU1*sinSigma*cosAlpha1
                );

            var C = F/16*cosSqAlpha*(4 + F*(4 - 3*cosSqAlpha));
            var L = lambda - (1 - C)*F*sinAlpha*
                       (sigma + C*sinSigma*(cos2SigmaM + C*cosSigma*(-1 + 2*cos2SigmaM*cos2SigmaM)));
            var lon2 = (lon1 + L + 3*Math.PI)%(2*Math.PI) - Math.PI;

            var objectLatitude = lat2*180/Math.PI;
            var objectLongitude = lon2*180/Math.PI;


            var answer = new Point
            {
                Y = objectLatitude,
                X = objectLongitude
            };
            return answer;
        }


    }
}