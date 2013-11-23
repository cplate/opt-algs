using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using OptimizationAlgorithms.GeneticAlgorithm.Demos.SantasRoute.Routing.Models;
using OptimizationAlgorithms.GeneticAlgorithm.Operations;
using log4net;

namespace OptimizationAlgorithms.GeneticAlgorithm.Demos.SantasRoute.Routing.Operations
{
    public class RouteMutationOperation : IMutationOperation<Route>
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ICandidateEvaluator<Route> _evaluator;
        private readonly ThreadLocal<Random> _randomGenerator;

        public RouteMutationOperation(ICandidateEvaluator<Route> evaluator)
        {
            _evaluator = evaluator;
            _randomGenerator = new ThreadLocal<Random>(() => new Random());
        }

        public Route Mutate(Route candidateToMutate)
        {
            // Using two different mutation approaches, swaps and moves
            // Randomly decide which to attempt
            var next = _randomGenerator.Value.NextDouble();
            return (next < 0.5 ? doSwapMutation(candidateToMutate) : doMoveMutation(candidateToMutate));
        }

        private Route doSwapMutation(Route candidateToMutate)
        {
            // Attempt swap of each position with each other position,
            // keeping track of which swap had the best result
            var testSeq = new List<int>(candidateToMutate.LocationSequence);
            var testRoute = new Route {LocationSequence = testSeq};
            var bestSeq = new List<int>(candidateToMutate.LocationSequence);
            var bestResult = _evaluator.Evaluate(testRoute).Score;
            
            for (int idx = 0; idx < bestSeq.Count; idx++)
            {
                for (int swapIdx = idx + 1; swapIdx < bestSeq.Count; swapIdx++)
                {
                    // swap
                    var tmp = testSeq[swapIdx];
                    testSeq[swapIdx] = testSeq[idx];
                    testSeq[idx] = tmp;
                    // eval
                    var result = _evaluator.Evaluate(testRoute);
                    if (result.Score < bestResult)
                    {
                        bestSeq = new List<int>(testSeq);
                        bestResult = result.Score;
                    }
                    // swap back
                    tmp = testSeq[swapIdx];
                    testSeq[swapIdx] = testSeq[idx];
                    testSeq[idx] = tmp;
                }
            }            

            // Generate a new route with best sequence we found, leaving parameter unchanged
            var mutated = new Route {LocationSequence = bestSeq};
            return mutated;
        }

        private Route doMoveMutation(Route candidateToMutate)
        {
            // For each index, pull out the current value and attempt to reinsert at each position in the route
            // keeping track of which move had the best result
            // This approach is pretty bad if the routes are really big, as its n-squared, but
            // for this demo its ok
            var testSeq = new List<int>(candidateToMutate.LocationSequence);
            var testRoute = new Route {LocationSequence = testSeq};
            var bestSeq = new List<int>(candidateToMutate.LocationSequence);
            var bestResult = _evaluator.Evaluate(testRoute).Score;

            for (int idx = 0; idx < bestSeq.Count; idx++)
            {
                var removedVal = testSeq[idx];
                testSeq.RemoveAt(idx);
                for (int moveToIdx = 0; moveToIdx < bestSeq.Count; moveToIdx++)
                {
                    // add the removed value to this position
                    testSeq.Insert(moveToIdx, removedVal);
                    // eval
                    var result = _evaluator.Evaluate(testRoute);
                    if (result.Score < bestResult)
                    {
                        bestSeq = new List<int>(testSeq);
                        bestResult = result.Score;
                    }
                    // remove again
                    testSeq.RemoveAt(moveToIdx);
                }
                // put back the way it was to prep for next round
                testSeq.Insert(idx, removedVal);
            }            

            // Generate a new route with best sequence we found, leaving parameter unchanged
            var mutated = new Route {LocationSequence = bestSeq};
            return mutated;
        }
    }
}
