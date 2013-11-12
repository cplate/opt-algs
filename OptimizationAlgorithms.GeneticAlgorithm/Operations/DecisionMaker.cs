using System;
using System.Threading;

namespace OptimizationAlgorithms.GeneticAlgorithm.Operations
{
    public class DecisionMaker : IDecisionMaker
    {
        private readonly ThreadLocal<Random> _random;

        public DecisionMaker()
        {
            _random = new ThreadLocal<Random>(() => new Random());
        }

        public bool DecideBool(double truePercentage)
        {            
            if (truePercentage <= 0.00001) return false;
                return _random.Value.NextDouble() <= truePercentage;            
        }

        public int DecideIntBetween(int min, int max)
        {
            // Next is inclusive lower bound and exclusive upper bound
            return _random.Value.Next(min, max + 1);
        }
    }
}
