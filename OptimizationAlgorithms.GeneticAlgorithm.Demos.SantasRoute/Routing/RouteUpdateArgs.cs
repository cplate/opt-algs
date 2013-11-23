using System.Collections.Generic;
using OptimizationAlgorithms.GeneticAlgorithm.Demos.SantasRoute.Routing.Models;

namespace OptimizationAlgorithms.GeneticAlgorithm.Demos.SantasRoute.Routing
{
    public class RouteUpdateArgs
    {
        public List<Location> Sequence { get; set; }
        public double Miles { get; set; }
    }
}