using System.Collections.Generic;
using System.Linq;
using Domain;
using MoreLinq;

namespace Analysis
{
    public class Analyzer
    {
        public IList<AnalysisResult> Analyze(IList<Coordinate> source, IList<Coordinate> result,
            IList<Coordinate> actual)
        {
            return Enumerable.Zip(source, result, (sourceCoordinate, resultCoordinate) => new AnalysisResult
            {
                Source = sourceCoordinate,
                Result = resultCoordinate,
                SourceDerivation =
                    actual.Pairwise(
                        (coordinate, coordinate1) =>
                            Utils.PointLineDistance(
                                new Point {X = sourceCoordinate.Longitude, Y = sourceCoordinate.Latitude},
                                new Point {X = coordinate.Longitude, Y = coordinate.Latitude},
                                new Point {X = coordinate1.Longitude, Y = coordinate1.Latitude})).Min(),
                ResultDerivation =
                    actual.Pairwise(
                        (coordinate, coordinate1) =>
                            Utils.PointLineDistance(
                                new Point {X = resultCoordinate.Longitude, Y = resultCoordinate.Latitude},
                                new Point {X = coordinate.Longitude, Y = coordinate.Latitude},
                                new Point {X = coordinate1.Longitude, Y = coordinate1.Latitude})).Min()
            }).ToList();
        }

        public class AnalysisResult
        {
            public double SourceDerivation { get; set; }
            public double ResultDerivation { get; set; }
            public Coordinate Source { get; set; }
            public Coordinate Result { get; set; }
        }
    }
}