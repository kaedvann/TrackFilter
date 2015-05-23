using System.Linq;
using Analysis;
using Domain;
using Filter;
using NUnit.Framework;

namespace DomainTests
{
    [TestFixture]
   public  class AnalyzerTests
    {
        [Test]
        public void TestMeasuring()
        {
            var worker = new TrackXmlWorker();
            var precise = worker.ReadTracks("../../../../../data/analysis/precise.xml");
            var actual = worker.ReadTracks("../../../../../data/analysis/actual.xml");
            var filter = new TrackProcessor(){StopsDetection = false, SpikeDetection = false};
            var result = filter.ProcessTracks(actual);
            var analyzer = new Analyzer();
            var analysis = analyzer.Analyze(actual.First().Coordinates, result.Coordinates, precise.First().Coordinates);
            var sourceAverage = analysis.Average(a => a.SourceDerivation);
            var resultAverage = analysis.Average(a => a.ResultDerivation);
            var sourceMin = analysis.Min(a => a.SourceDerivation);
            var resultMin = analysis.Min(a => a.ResultDerivation);

            var sourceMax = analysis.Max(a => a.SourceDerivation);
            var resultMax = analysis.Max(a => a.ResultDerivation);
        }
    }
}
