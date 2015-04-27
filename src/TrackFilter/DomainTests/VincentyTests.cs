using System;
using System.Diagnostics;
using Domain;
using NUnit.Framework;

namespace DomainTests
{
    [TestFixture]
    public class VincentyTests
    {
        [Test]
        public void TestConvertation()
        {
            //тестирование проводилось методом тщательного всматривания в яндекс-карты
            var coordinate = new Coordinate() { Longitude = 37.512353, Latitude = 55.769474 };
            var point = VincentyEllipsoid.GetPointFromDistance(Math.PI/2, 200, coordinate.Longitude, coordinate.Latitude);
            Debugger.Break();
        }
    }
}
