using System;
using System.Collections.Generic;
using System.Linq;
using OptimizationAlgorithms.GeneticAlgorithm.Demos.SimpleTSP.Models;
using OptimizationAlgorithms.GeneticAlgorithm.Demos.SimpleTSP.Operations;

namespace OptimizationAlgorithms.GeneticAlgorithm.Demos.SimpleTSP
{
    // Responsible for optimizing the sequence of locations on a route
    // This is a demo, so not worrying about abstraction here and just
    // directly wiring things up.  A real impl might do this more flexiblity
    // to allow for use of different operations depending on the problem characteristics
    public class RouteLocationSequencer
    {
        private readonly DistanceCalculator _distanceCalculator;
        
        public RouteLocationSequencer()
        {
            _distanceCalculator = new DistanceCalculator();
        }

        public Route Sequence(Location startLocation, List<Location> routeLocations)
        {
            // To do sequencing, need to know distances between each pair of locations, including starting location
            var distancesInput = new [] { startLocation }.Concat(routeLocations).ToList();
            var distMatrix = _distanceCalculator.GetDistanceMatrix(distancesInput);

            var algParms = new GeneticAlgorithmParameters { MaxIterations = 5000, PopulationSize = 250, SurvivalPercentage = 0.2, NewBloodPercentage = 0.1, AllowParallelization = false};
            var algOps = new GeneticAlgorithmOperations<Route>
                {
                    CandidateEvaluator = new RouteEvaluator(distMatrix),
                    CandidateFactory = new RouteBuilder(routeLocations),
                    CrossoverOperation = new RouteCrossoverOperation(),
                    MutationOperation = new RouteMutationOperation(),
                    NaturalSelectionOperation = new RouteSelectionOperation()
                };
            var alg = new GeneticAlgorithm<Route>(algParms, algOps);
            alg.SolutionImproved += solutionImproved;
            var route = alg.GetBestCandidate(null);
            alg.SolutionImproved -= solutionImproved;
            return route;
        }

        private void solutionImproved(object sender, SolutionImprovedArgs<Route> args)
        {
            Console.WriteLine("Improved solution at iteration {0} with {1} miles",args.Iteration,args.Score);
        }
    }
}
