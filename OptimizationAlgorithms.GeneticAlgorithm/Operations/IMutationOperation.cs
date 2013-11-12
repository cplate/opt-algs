namespace OptimizationAlgorithms.GeneticAlgorithm.Operations
{
    public interface IMutationOperation<TCandidate> where TCandidate : class
    {
        // Takes an existing candidate and performs mutation procedure, leaving 
        // existing candiate unchanged and returning a new candidate that is
        // similar to the existing candidate
        TCandidate Mutate(TCandidate candidateToMutate);
    }
}
