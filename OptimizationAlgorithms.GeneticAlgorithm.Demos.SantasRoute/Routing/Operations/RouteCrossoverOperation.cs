using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OptimizationAlgorithms.GeneticAlgorithm.Demos.SantasRoute.Routing.Models;
using OptimizationAlgorithms.GeneticAlgorithm.Operations;

namespace OptimizationAlgorithms.GeneticAlgorithm.Demos.SantasRoute.Routing.Operations
{
    public class RouteCrossoverOperation : ICrossoverOperation<Route>
    {
        private readonly ThreadLocal<Random> _randomGenerator;

        public RouteCrossoverOperation()
        {
            _randomGenerator = new ThreadLocal<Random>(() => new Random());
        }

        public Route Crossover(Route parent1, Route parent2)
        {
            // Build a new route by taking a subseqence of parent one and inserting
            // it in the same position in parent2
            var subseqStartIdx = _randomGenerator.Value.Next(0, parent1.LocationSequence.Count - 2);
            var subseqEndIdx = _randomGenerator.Value.Next(subseqStartIdx + 1, parent1.LocationSequence.Count - 1);

            var parent1SubSequence = parent1.LocationSequence.Skip(subseqStartIdx).Take(subseqEndIdx - subseqStartIdx).ToList();

            // Start building the new sequence by walking down parent2 and adding locations
            // that arent in the targeted subsequence of parent1
            var newSequence = new List<int>(parent1.LocationSequence.Count);
            var parent2Idx = 0; 
            while (newSequence.Count < subseqStartIdx)
            {
                var locIdx = parent2.LocationSequence[parent2Idx];
                if (!parent1SubSequence.Contains(locIdx))
                {
                    newSequence.Add(locIdx);
                }
                parent2Idx++;
            }      

            // Add the subsequence
            newSequence.AddRange(parent1SubSequence);

            // Add the remaining locations in parent2
            while (parent2Idx < parent2.LocationSequence.Count)
            {
                var locIdx = parent2.LocationSequence[parent2Idx];
                if (!parent1SubSequence.Contains(locIdx))
                {
                    newSequence.Add(locIdx);
                }
                parent2Idx++;
            }

            return new Route {LocationSequence = newSequence};
        }
    }
}
