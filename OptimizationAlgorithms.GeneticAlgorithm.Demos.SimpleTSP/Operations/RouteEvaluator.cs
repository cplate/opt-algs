using System.Collections.Generic;
using System.Linq;
using OptimizationAlgorithms.GeneticAlgorithm.Demos.SimpleTSP.Models;
using OptimizationAlgorithms.GeneticAlgorithm.Models;
using OptimizationAlgorithms.GeneticAlgorithm.Operations;

namespace OptimizationAlgorithms.GeneticAlgorithm.Demos.SimpleTSP.Operations
{
    public class RouteEvaluator : ICandidateEvaluator<Route>
    {
        private readonly double[,] _distances;

        public RouteEvaluator(double[,] distances)
        {
            _distances = distances;
        }

        public EvaluatedCandidate<Route> Evaluate(Route candidate)
        {
            // In a "real" implementation, there might be a lot more going on here.
            // For our purposes, score is sum of distances between locations on route
            // Note that the distance matrix includes distances from the start location as well,
            // so we need to offset the route location sequence indicies by 1
            var dist = _distances[0,candidate.LocationSequence[0]+1];
            for (var idx = 0; idx < candidate.LocationSequence.Count - 1; idx++)
            {
                dist += _distances[candidate.LocationSequence[idx]+1, candidate.LocationSequence[idx + 1]+1];
            }

            return new EvaluatedCandidate<Route>
                {
                    Candidate = candidate,
                    Score = dist 
                };
        }

        public List<EvaluatedCandidate<Route>> Sort(List<EvaluatedCandidate<Route>> evaluatedCandidates)
        {
            // smaller scores are considered better in our impl, as shorter routes are better 
            return evaluatedCandidates.OrderBy(x => x.Score).ToList();
        }

        public bool IsBetterThan(EvaluatedCandidate<Route> candidate1, EvaluatedCandidate<Route> candidate2)
        {
            return (candidate1.Score < candidate2.Score);
        }
    }
}
