using System.Collections.Generic;
using OptimizationAlgorithms.GeneticAlgorithm.Demos.SantasRoute.Routing.Models;

namespace OptimizationAlgorithms.GeneticAlgorithm.Demos.SantasRoute.Routing.Client
{
    // Request received from the client to begin an optimization
    // This could include more stuff, like how long to process, etc.
    public class RouteOptimizationRequest
    {
        public Location Start { get; set; }
        public List<Location> Locations { get; set; }
    }
}