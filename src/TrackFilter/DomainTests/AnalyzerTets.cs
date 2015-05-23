using Domain;
using NUnit.Framework;

namespace DomainTests
{
    [TestFixture]
   public  class AnalyzerTets
    {
        [Test]
        public void TestMeasuring()
        {
            var worker = new TrackXmlWorker();
            var precise = worker.ReadTracks("../../../../../data/analysis/precise.xml");
            var actual = worker.ReadTracks("../../../../../data/analysis/actual.xml");

        }
    }
}
