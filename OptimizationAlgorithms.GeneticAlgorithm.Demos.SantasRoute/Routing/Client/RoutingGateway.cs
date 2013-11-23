using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using OptimizationAlgorithms.GeneticAlgorithm.Demos.SantasRoute.Routing.Models;
using log4net;

namespace OptimizationAlgorithms.GeneticAlgorithm.Demos.SantasRoute.Routing.Client
{
    // Acts as the proxy to the optimization process on the server.  This class forwards 
    // updates relating to the routing to the requesting client
    // A better architecture would probably have the actual optimization being done
    // in a separate process than the web app, but this is a demo....
    public class RoutingGateway
    {
        // Keep track of the state of an optimization 
        private class RoutingState
        {
            public RouteGenerator Generator { get; set; }
            public Task<Route> Task { get; set; }
            public CancellationTokenSource Canceller { get; set; }
        }

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // Bunch of static stuff to keep track of the ongoing routing processes and enable us
        // to clean them up when complete
        private static readonly Dictionary<RouteGenerator, string> _routingsByGenerator;
        private static readonly Dictionary<string, RoutingState> _routingsBySubscriber;
        private static readonly RouteGeneratorFactory _routeGeneratorFactory;
        private static readonly object _cleanupLock = new object();

        static RoutingGateway()
		{
            _routingsByGenerator = new Dictionary<RouteGenerator, string>();
            _routingsBySubscriber = new Dictionary<string, RoutingState>();
            _routeGeneratorFactory = new RouteGeneratorFactory();
		}

        // Kick off routing for given subscriber, called from hub
		public static void Optimize(string subscriberId, RouteOptimizationRequest request , DateTime requestDateTime)
		{
            // Take a momemt to cleanup anything that's done processing
            lock (_cleanupLock)
            {
               CleanupCompletedOptimizations();
            }
            
            if (_routingsBySubscriber.ContainsKey(subscriberId))
            {
                Log.WarnFormat("Subscriber {0} already has route request working, not starting another one",subscriberId);
                return;
            }

			Log.DebugFormat("Subscriber {0} to optimize at {1}", subscriberId, requestDateTime);

            // Build the class to do the work, and hook up events
		    var routeGenerator = _routeGeneratorFactory.BuildGenerator(request);
		    routeGenerator.RouteUpdated += NotifyOfRouteUpdate;
		    routeGenerator.RoutingComplete += NotifyOfRouteCompletion;
		    routeGenerator.ProgressMade += NotifyOfProgressMade;

            // Keep track of a cancellation token source in can user wants to cancel
		    var cancellationSource = new CancellationTokenSource();

            // Kick it off
		    var task = routeGenerator.GenerateRouteAsync(request.Start, request.Locations, cancellationSource.Token);

            // Add to some lookups so we can get back to things later
            _routingsByGenerator.Add(routeGenerator, subscriberId);
            _routingsBySubscriber.Add(subscriberId, new RoutingState { Canceller = cancellationSource, Generator = routeGenerator, Task = task });
		}

        // Cancel routing process for given subscriber, called from hub
        public static void CancelOptimization(string subscriberId, DateTime requestDateTime)
        {
            lock (_cleanupLock)
            {
                RoutingState state;
                _routingsBySubscriber.TryGetValue(subscriberId, out state);
                if (state != null)
                {
                    // Cancel it
                    state.Canceller.Cancel();
                }

                CleanupCompletedOptimizations();
            }
        }

        // Look for tasks that are complete and unhook event handlers
        private static void CleanupCompletedOptimizations()
        {
            var keys = _routingsByGenerator.Keys.ToList();
            foreach (var g in keys)
            {
                if (g.IsComplete)
                {                    
                    var subscriber = _routingsByGenerator[g];

                    g.RouteUpdated -= NotifyOfRouteUpdate;
                    g.RoutingComplete -= NotifyOfRouteCompletion;
                    g.ProgressMade -= NotifyOfProgressMade;
                    _routingsByGenerator.Remove(g);
                    _routingsBySubscriber.Remove(subscriber);
                }
            }
        }

		// This gets called when we get an improved route from an route generator instance
		private static void NotifyOfRouteUpdate(object generator, RouteUpdateArgs args)
		{
            // Find the subscriber for this generator
		    string subscriberId;
		    _routingsByGenerator.TryGetValue((RouteGenerator)generator, out subscriberId);

		    if (subscriberId != null)
		    {
		        //Log.DebugFormat("Notifying subscriber {0} of routing update",subscriberId);
		        var hubContext = GlobalHost.ConnectionManager.GetHubContext<RoutingHub>();

		        // updateRouteDetails is the method that the client has designated to receive the data
		        hubContext.Clients.Client(subscriberId).updateRouteDetails(new RouteUpdateResponse
		            {
		                LocationSequence = args.Sequence,
                        Miles = args.Miles
		            });
		    }
		}

        // This gets called when the routing process is complete (cancelled or not)
        private static void NotifyOfRouteCompletion(object generator, RouteUpdateArgs args)
        {
            // Find the subscriber for this generator
            string subscriberId;
            _routingsByGenerator.TryGetValue((RouteGenerator)generator, out subscriberId);

            if (subscriberId != null)
            {
                //Log.DebugFormat("Notifying subscriber {0} of routing completion",subscriberId);
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<RoutingHub>();

                // updateFinalRoute is the method that the client has designated to receive the data
                hubContext.Clients.Client(subscriberId).updateFinalRoute(new RouteUpdateResponse
                {
                    LocationSequence = args.Sequence,
                    Miles = args.Miles
                });
            }
        }

        // This gets called when the routing process informs us of its progress
        private static void NotifyOfProgressMade(object generator, RoutingProgressArgs args)
        {
            // Find the subscriber for this generator
            string subscriberId;
            _routingsByGenerator.TryGetValue((RouteGenerator)generator, out subscriberId);

            if (subscriberId != null)
            {
                //Log.DebugFormat("Notifying subscriber {0} of routing progress",subscriberId);
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<RoutingHub>();

                // updateProgress is the method that the client has designated to receive the data
                hubContext.Clients.Client(subscriberId).updateProgress(args);
            }
        }

    }
}