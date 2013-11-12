using System;
using System.Collections.Generic;
using OptimizationAlgorithms.GeneticAlgorithm.Models;

namespace OptimizationAlgorithms.GeneticAlgorithm.Operations
{
    public interface INaturalSelectionOperation<TCandidate> where TCandidate : class
    {
        // Select the specified number of candidates from the list provided
        // (e.g., top 10, top 1 and randomly select others, etc)
        // Note that the "best" solution should be the first item in the returned list
        List<TCandidate> Select(List<EvaluatedCandidate<TCandidate>> evaluatedCandidates, int numberOfSurvivors);
    }
}
