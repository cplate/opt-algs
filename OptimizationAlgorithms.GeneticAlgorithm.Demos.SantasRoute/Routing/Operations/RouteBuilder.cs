using System;
using System.Collections.Generic;
using System.Linq;
using OptimizationAlgorithms.GeneticAlgorithm.Demos.SantasRoute.Routing.Models;
using OptimizationAlgorithms.GeneticAlgorithm.Operations;

namespace OptimizationAlgorithms.GeneticAlgorithm.Demos.SantasRoute.Routing.Operations
{
    // Generate some potential routes for the algorithm to work with
    public class RouteBuilder : ICandidateFactory<Route>
    {
        private readonly List<Location> _locations;
        private readonly Random _randomGenerator;

        public RouteBuilder(List<Location> locations)
        {
            _locations = locations;
            _randomGenerator = new Random();
        }

        public List<Route> GeneratePool(int poolSize)
        {
            var routes = new List<Route>(poolSize);
            for (var idx = 0; idx < poolSize; idx++)
                routes.Add(GenerateOne());
            return routes;
        }

        public Route GenerateOne()
        {
            // Just generating a random sequence...
            var sequence = Enumerable.Range(0, _locations.Count).OrderBy(n => _randomGenerator.Next());
            return new Route {LocationSequence = sequence.ToList()};
        }
    }
}
