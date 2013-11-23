using OptimizationAlgorithms.GeneticAlgorithm.Demos.SantasRoute.Routing.Client;

namespace OptimizationAlgorithms.GeneticAlgorithm.Demos.SantasRoute.Routing
{
    // Bit of an abstraction layer to convert a client request into a route generator.
    // Not really needed, but could take more attributes from request (e.g., duration) and initialize things
    public class RouteGeneratorFactory
    {
        public RouteGenerator BuildGenerator(RouteOptimizationRequest request)
        {
            var generator = new RouteGenerator();
            return generator;
        }
    }
}