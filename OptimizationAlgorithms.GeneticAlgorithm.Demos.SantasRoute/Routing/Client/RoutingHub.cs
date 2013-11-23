using System;
using System.Reflection;
using Microsoft.AspNet.SignalR;
using log4net;

namespace OptimizationAlgorithms.GeneticAlgorithm.Demos.SantasRoute.Routing.Client
{
    // Acts as the endpoint for clients to use in starting/stopping a routing process on the server
    public class RoutingHub : Hub
    {
	    // Relying on a separate class to handle the sending of updates
	    // out to clients, as we cant have this class listening for updates since
	    // a new instance of it is created per request
		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		// Method called from SignalR client indicating to start an optimization
		public void Optimize(RouteOptimizationRequest request, DateTime requestDateTime)
		{
			var connId = Context.ConnectionId;
			Log.DebugFormat("Received optimization request for {0} at {1}", connId, requestDateTime);			
			RoutingGateway.Optimize(connId, request, requestDateTime);
		}

        // Method called from SignalR client indicating to end a previously started optimization
        public void CancelOptimization(DateTime requestDateTime)
        {
            var connId = Context.ConnectionId;
            Log.DebugFormat("Received optimization cancellation request for {0} at {1}", connId, requestDateTime);
            RoutingGateway.CancelOptimization(connId, requestDateTime);            
        }

		// Called when client explicitly disconnects or connection is lost
		public override System.Threading.Tasks.Task OnDisconnected()
		{
			var connId = Context.ConnectionId;
			Log.DebugFormat("Conn {0} disconnected", connId);
			RoutingGateway.CancelOptimization(connId, new DateTime());            

			return base.OnDisconnected();
		}
    }
}