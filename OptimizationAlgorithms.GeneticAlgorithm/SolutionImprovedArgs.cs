namespace OptimizationAlgorithms.GeneticAlgorithm
{
    public class SolutionImprovedArgs<TCandidate> where TCandidate:class
    {
        public int Iteration { get; set; }
        public TCandidate Candidate { get; set; }
        public double Score { get; set; }
    }
}
