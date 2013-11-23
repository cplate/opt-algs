using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using OptimizationAlgorithms.GeneticAlgorithm.Demos.SantasRoute.Routing.Models;
using OptimizationAlgorithms.GeneticAlgorithm.Demos.SantasRoute.Routing.Operations;
using log4net;

namespace OptimizationAlgorithms.GeneticAlgorithm.Demos.SantasRoute.Routing
{
    // Responsible for optimizing the sequence of locations on a route
    // This is a demo, so not worrying about abstraction here and just
    // directly wiring things up.  A real impl might do this more flexiblity
    // to allow for use of different operations depending on the problem characteristics
    public class RouteGenerator
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly DistanceCalculator _distanceCalculator;
        private Location _startLocation;
        private List<Location> _routeLocations;
        private GeneticAlgorithm<Route> _algorithm;
        
        public delegate void RouteUpdateEventHandler(object sender, RouteUpdateArgs args);
        public delegate void ProgressUpdateEventHandler(object sender, RoutingProgressArgs args);

        // Events to allow caller to keep track of whats going on
        public event RouteUpdateEventHandler RouteUpdated;
        public event RouteUpdateEventHandler RoutingComplete;
        public event ProgressUpdateEventHandler ProgressMade;

        public bool IsComplete { get; set; }

        public RouteGenerator()
        {
            _distanceCalculator = new DistanceCalculator();
        }

        // Doing this async as it could take a while...
        public Task<Route> GenerateRouteAsync(Location startLocation, List<Location> routeLocations, CancellationToken cancelToken)
        {
            _startLocation = startLocation;
            _routeLocations = routeLocations;

            // To do sequencing, need to know distances between each pair of locations, including starting location
            var distancesInput = new [] { startLocation }.Concat(routeLocations).ToList();
            var distMatrix = _distanceCalculator.GetDistanceMatrix(distancesInput);

            var algParms = new GeneticAlgorithmParameters { MaxIterations = 250, PopulationSize = 500, MutationPercentage = 0.5, SurvivalPercentage = 0.3, NewBloodPercentage = 0.05, AllowParallelization = false};
            var algOps = new GeneticAlgorithmOperations<Route>
                {
                    CandidateEvaluator = new RouteEvaluator(distMatrix),
                    CandidateFactory = new RouteBuilder(routeLocations),
                    CrossoverOperation = new RouteCrossoverOperation(),
                    MutationOperation = new RouteMutationOperation(new RouteEvaluator(distMatrix)),
                    NaturalSelectionOperation = new RouteSelectionOperation()
                };

            _algorithm = new GeneticAlgorithm<Route>(algParms, algOps);
            _algorithm.SolutionImproved += solutionImproved;
            _algorithm.ProgressUpdated += progressUpdated;

            // Added follow-on task to allow us to trigger an event to notify caller that routing is complete
            var task = _algorithm.GetBestCandidateAsync(null, cancelToken).ContinueWith(t =>
                {
                    IsComplete = true;
                    if (t.Exception == null)
                    {
                        if (RoutingComplete != null)
                        {
                            var seq = t.Result.LocationSequence.Select(x => _routeLocations[x]).ToList();
                            seq.Insert(0, _startLocation);
                            RoutingComplete(this, new RouteUpdateArgs { Miles = algOps.CandidateEvaluator.Evaluate(t.Result).Score, Sequence = seq });
                        }
                        return t.Result;
                    }
                    return null; // error handling should be better...
                });
            return task;
        }

        // Handle GA event that provides the updated route, and send our caller an event of our own
        private void solutionImproved(object sender, SolutionImprovedArgs<Route> args)
        {
            if (RouteUpdated != null)
            {
                var seq = args.Candidate.LocationSequence.Select(x => _routeLocations[x]).ToList();
                seq.Insert(0, _startLocation);
                RouteUpdated(this, new RouteUpdateArgs { Miles = args.Score, Sequence = seq });
            }
                
            Log.DebugFormat("Improved solution at iteration {0} with {1} miles",args.Iteration,args.Score);
        }

        // Handle GA event that provides progress update, and send our caller an event of our own
        private void progressUpdated(object sender, GeneticAlgorithmProgressUpdateArgs args)
        {
            if (ProgressMade != null)
            {
                ProgressMade(this, new RoutingProgressArgs { PercentComplete = (double)args.IterationsProcessed / args.IterationsBeingPerformed * 100 });
            }
                
            Log.DebugFormat("Processed iteration {0} of {1}",args.IterationsProcessed,args.IterationsBeingPerformed);
        }
    }
}
