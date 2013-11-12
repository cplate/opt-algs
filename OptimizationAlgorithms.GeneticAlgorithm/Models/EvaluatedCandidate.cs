using System;

namespace OptimizationAlgorithms.GeneticAlgorithm.Models
{
    // Pair a candidate with its evaluation score
    public class EvaluatedCandidate<TCandidate> where TCandidate : class
    {
        public TCandidate Candidate { get; set; }
        public double Score { get; set; }
    }
}
