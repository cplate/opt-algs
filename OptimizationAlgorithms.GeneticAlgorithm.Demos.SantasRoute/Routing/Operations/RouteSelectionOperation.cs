using System.Collections.Generic;
using System.Linq;
using OptimizationAlgorithms.GeneticAlgorithm.Demos.SantasRoute.Routing.Models;
using OptimizationAlgorithms.GeneticAlgorithm.Models;
using OptimizationAlgorithms.GeneticAlgorithm.Operations;

namespace OptimizationAlgorithms.GeneticAlgorithm.Demos.SantasRoute.Routing.Operations
{
    public class RouteSelectionOperation : INaturalSelectionOperation<Route>
    {
        public List<Route> Select(List<EvaluatedCandidate<Route>> evaluatedCandidates, int numberOfSurvivors)
        {
            // Just keep X distinct top scorers
            return evaluatedCandidates.Select(x => x.Candidate).Distinct().Take(numberOfSurvivors).ToList();
        }
    }
}
