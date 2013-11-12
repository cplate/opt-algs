using System;
using System.Collections.Generic;
using System.Threading;
using OptimizationAlgorithms.GeneticAlgorithm.Demos.SimpleTSP.Models;
using OptimizationAlgorithms.GeneticAlgorithm.Operations;

namespace OptimizationAlgorithms.GeneticAlgorithm.Demos.SimpleTSP.Operations
{
    public class RouteMutationOperation : IMutationOperation<Route>
    {
        private readonly ThreadLocal<Random> _randomGenerator;

        public RouteMutationOperation()
        {
            _randomGenerator = new ThreadLocal<Random>(() => new Random());
        }

        public Route Mutate(Route candidateToMutate)
        {
            // Simply pick two locations and swap them
            var loc1Idx = _randomGenerator.Value.Next(0, candidateToMutate.LocationSequence.Count - 1);
            var loc2Idx = loc1Idx;
            while (loc2Idx == loc1Idx)
            {
                loc2Idx = _randomGenerator.Value.Next(0, candidateToMutate.LocationSequence.Count - 1);
            }

            // Generate a new route, leaving parameter unchanged
            var mutated = new Route {LocationSequence = new List<int>(candidateToMutate.LocationSequence)};
            mutated.LocationSequence[loc1Idx] = candidateToMutate.LocationSequence[loc2Idx];
            mutated.LocationSequence[loc2Idx] = candidateToMutate.LocationSequence[loc1Idx];

            return mutated;
        }
    }
}
