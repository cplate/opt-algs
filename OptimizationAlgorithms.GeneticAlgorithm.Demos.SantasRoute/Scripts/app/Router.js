var SantasRoute = SantasRoute || {};

// Class that interfaces with server in dealing with the generation of the route
SantasRoute.Router = (function () {
    // User provides the callbacks we should call when we get responses from the server
    function Router(routeUpdatedCallback, routeFinishedCallback, routingProgressCallback) {
        this.isConnected = false;
        this.routerProxy = $.connection.routingHub;
        this.routerProxy.client.updateRouteDetails = function (msg) {
            routeUpdatedCallback(msg);
        };
        this.routerProxy.client.updateFinalRoute = function (msg) {
            routeFinishedCallback(msg);
        };
        this.routerProxy.client.updateProgress = function (msg) {
            routingProgressCallback(msg);
        };
    }
    Router.prototype.connect = function (onConnectCallback) {
        if (!this.isConnected) {
            var self = this;
            $.connection.hub.start()
                .done(function () { self.isConnected = true; onConnectCallback(); })
                .fail(function () { self.isConnected = false; alert("Unable to contact routing process!"); }); // error handling should be a bit better...
        } else {
            onConnectCallback();
        }
    };
    Router.prototype.disconnect = function () {
        if (this.isConnected) {
            this.routerProxy.stop();
            this.isConnected = false;
        }
    };
    Router.prototype.route = function (start, locations) {
        var self = this;
        // calling connect just in case...
        this.connect(function() {
            var rqt = {
                Start: start,
                Locations: locations
            };
            self.routerProxy.server.optimize(rqt, new Date());
        });
    };
    Router.prototype.cancel = function () {
        this.routerProxy.server.cancelOptimization(new Date());
    };
    return Router;
})();
