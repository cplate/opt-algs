using System.Collections.Generic;
using OptimizationAlgorithms.GeneticAlgorithm.Models;

namespace OptimizationAlgorithms.GeneticAlgorithm.Operations
{
    public interface ICandidateEvaluator<TCandidate> where TCandidate : class
    {
        // Provide a "score" or "fitness" for this candidate based on its makeup.
        EvaluatedCandidate<TCandidate> Evaluate(TCandidate candidate);

        // Sort candidates by their score, allows implementor to decide whether
        // higher or lower scores are more desireable
        List<EvaluatedCandidate<TCandidate>> Sort(List<EvaluatedCandidate<TCandidate>> evaluatedCandidates);

        // Return true if candidate1 has a better score than candidate2
        bool IsBetterThan(EvaluatedCandidate<TCandidate> candidate1, EvaluatedCandidate<TCandidate> candidate2);
    }
}
