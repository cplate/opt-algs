namespace OptimizationAlgorithms.GeneticAlgorithm.Operations
{
    public interface ICrossoverOperation<TCandidate> where TCandidate : class
    {
        // Takes two existing "parent" candidates and performs crossover procedure, leaving 
        // the existing candiates unchanged and returning a new "child" candidate that 
        // has some characteristics of both parents
        TCandidate Crossover(TCandidate parent1, TCandidate parent2);
    }
}
