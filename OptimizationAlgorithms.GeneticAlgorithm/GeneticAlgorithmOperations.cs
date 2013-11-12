using System;
using OptimizationAlgorithms.GeneticAlgorithm.Operations;

namespace OptimizationAlgorithms.GeneticAlgorithm
{
    // Bundle of user-defined customizable operations to be used by the algorithm
    public class GeneticAlgorithmOperations<TCandidate> where TCandidate:class
    {        
        public ICandidateEvaluator<TCandidate> CandidateEvaluator { get; set; }
        public ICrossoverOperation<TCandidate> CrossoverOperation { get; set; }
        public IMutationOperation<TCandidate> MutationOperation { get; set; }
        public INaturalSelectionOperation<TCandidate> NaturalSelectionOperation { get; set; }
        public ICandidateFactory<TCandidate> CandidateFactory { get; set; }
    }
}
