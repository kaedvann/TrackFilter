using Analysis;
using Domain;
using NUnit.Framework;

namespace DomainTests
{
    [TestFixture]
    public class UtilTests
    {
        [TestCase(0, 0, 0,1, 0.5, 1, 0.5)]
        [TestCase(0, 0, 2, 2, 2, 0, 1.4142135623730950488016887242097)]
        [TestCase(0,0,1,0,0.5,1,1)]
        [TestCase(0, 0, 1, 0, 2, 1, 1.4142135623730950488016887242097)]
        public void TestDistance(double x1, double y1, double x2, double y2, double x0, double y0, double expected)
        {
            var start = new Point {X=x1,Y=y1};
            var end = new Point {X = x2, Y = y2};
            var point = new Point {X = x0, Y = y0};
            var distance = Utils.PointLineDistance(point, start, end);
            Assert.AreEqual(expected, distance, 1e-3);
        }
    }
}
