using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ObjectTrackerService.Model
{
    class VincentyEllipsoid
    {
        private const double a = 6378137;
        private const double b = 6356752.3142;
        private const double f = 1 / 298.257223563;

        public VincentyEllipsoid()
        {

        }

        private Point GetPointFromDistance(double direction, double distance, double CurrLongtitude, double CurrLatitude)
        {
            //throw new NotImplementedException();

            double alpha1 = direction;
            double s = distance;
            double lo1 = CurrLongtitude;
            double la1 = CurrLatitude;

            double lat1 = la1 * Math.PI / 180;
            double lon1 = lo1 * Math.PI / 180;

            if (alpha1 < 0)
            {
                alpha1 = alpha1 + 2 * Math.PI;
            }
            if (alpha1 > 2 * Math.PI)
            {
                alpha1 = alpha1 - 2 * Math.PI;
            }

            double tanU1, cosU1, sinU1, sigma1, cosAlpha1, sinAlpha1, sinAlpha,
                cosSqAlpha, uSq, A, B, sigma, cos2SigmaM, sinSigma, cosSigma, deltaSigma,
                sigmaP, tmp, lat2, lambda, C, L, lon2;

            cosAlpha1 = Math.Cos(alpha1);
            sinAlpha1 = Math.Sin(alpha1);

            tanU1 = (1 - f) * Math.Tan(lat1);
            cosU1 = 1 / Math.Sqrt((1 + tanU1 * tanU1));
            sinU1 = tanU1 * cosU1;
            sigma1 = Math.Atan2(tanU1, cosAlpha1);
            sinAlpha = cosU1 * sinAlpha1;
            cosSqAlpha = 1 - sinAlpha * sinAlpha;
            uSq = cosSqAlpha * (a * a - b * b) / (b * b);
            A = 1 + uSq / 16384 * (4096 + uSq * (-768 + uSq * (320 - 175 * uSq)));
            B = uSq / 1024 * (256 + uSq * (-128 + uSq * (74 - 47 * uSq)));
            sigma = s / (b * A); sigmaP = 2 * Math.PI;
            sinSigma = 1;
            cosSigma = 1;
            cos2SigmaM = 1;
            while (Math.Abs(sigma - sigmaP) > 1e-12)
            {
                cos2SigmaM = Math.Cos(2 * sigma1 + sigma);
                sinSigma = Math.Sin(sigma);
                cosSigma = Math.Cos(sigma);
                deltaSigma = B * sinSigma * 
                    (cos2SigmaM + B / 4 * 
                        (cosSigma * (-1 + 2 * cos2SigmaM * cos2SigmaM) -
                            B / 6 * cos2SigmaM * 
                            (-3 + 4 * sinSigma * sinSigma) * 
                            (-3 + 4 * cos2SigmaM * cos2SigmaM)
                        )
                     );

                sigmaP = sigma;
                sigma = s / (b * A) + deltaSigma;
            }
            tmp = sinU1 * sinSigma - cosU1 * cosSigma * cosAlpha1;
            lat2 = Math.Atan2(
                sinU1 * cosSigma + cosU1 * sinSigma * cosAlpha1,
                (1 - f) * Math.Sqrt(sinAlpha * sinAlpha + tmp * tmp)
                );

            lambda = Math.Atan2(
                sinSigma * sinAlpha1, cosU1 * cosSigma - sinU1 * sinSigma * cosAlpha1
                );

            C = f / 16 * cosSqAlpha * (4 + f * (4 - 3 * cosSqAlpha));
            L = lambda - (1 - C) * f * sinAlpha *
                (sigma + C * sinSigma * (cos2SigmaM + C * cosSigma * (-1 + 2 * cos2SigmaM * cos2SigmaM)));
            lon2 = (lon1 + L + 3 * Math.PI) % (2 * Math.PI) - Math.PI;

            double object_Latitude = lat2 * 180 / Math.PI;
            double object_Longitude = lon2 * 180 / Math.PI;

            //Решение первой геодезической задачи (см. Морозов В.П. Курс сфероидической геодезии) Конец Алгоритма.

            Point Answer = new Point();
            Answer.Y = object_Latitude;
            Answer.X = object_Longitude;
            return Answer;
        }

    }
}