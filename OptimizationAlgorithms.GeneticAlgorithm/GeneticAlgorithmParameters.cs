namespace OptimizationAlgorithms.GeneticAlgorithm
{
    // General settings used by the algorithm
    public class GeneticAlgorithmParameters
    {        
        public int MaxIterations { get; set; } // How many times to apply the select/crossover/mutate loop
        public int PopulationSize { get; set; } // How big of a population to maintain
        public bool AllowParallelization { get; set; } // Whether to perform operation in parallel
        public double MutationPercentage { get; set; } // 0..1, percentage of population to mutate in each iteration
        public double SurvivalPercentage { get; set; } // 0..1, percentage of population that survives each iteration
        public double NewBloodPercentage { get; set; } // 0..1, percentage of population that should be reinitialized after each iteration, to prevent staleness

        public GeneticAlgorithmParameters()
        {
            // Provide some reasonable defaults
            MaxIterations = 100;
            PopulationSize = 100;
            AllowParallelization = false;
            MutationPercentage = 0.20;
            SurvivalPercentage = 0.30;
            NewBloodPercentage = 0.10;
        }
    }
}
