using System.Collections.Generic;

namespace OptimizationAlgorithms.GeneticAlgorithm.Operations
{
    public interface ICandidateFactory<TCandidate> where TCandidate : class
    {
        // Generate a pool of random candidates for use in the algorithm
        List<TCandidate> GeneratePool(int poolSize);

        // Generate a single random candidate for use in the algorithm
        TCandidate GenerateOne();
    }
}
