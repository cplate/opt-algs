using System.Collections.Generic;
using OptimizationAlgorithms.GeneticAlgorithm.Demos.SantasRoute.Routing.Models;

namespace OptimizationAlgorithms.GeneticAlgorithm.Demos.SantasRoute.Routing.Client
{
    // Response given to clients indicating the best route found thus far
    public class RouteUpdateResponse
    {
        public List<Location> LocationSequence { get; set; }
        public double Miles { get; set; }
    }
}