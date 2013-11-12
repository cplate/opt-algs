namespace OptimizationAlgorithms.GeneticAlgorithm.Operations
{
    public interface IDecisionMaker
    {
        // Provide a true/false value based on specification of
        // the percentage of the time we want the decision to be true
        bool DecideBool(double truePercentage);

        // Provide a number between the specified min (inclusive) and the specified max (inclusive)
        int DecideIntBetween(int min, int max);
    }
}
