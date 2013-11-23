using Owin;

namespace OptimizationAlgorithms.GeneticAlgorithm.Demos.SantasRoute
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Map all hubs to "/signalr"
            app.MapSignalR();
        }

    }
}