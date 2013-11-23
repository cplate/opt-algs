var SantasRoute = SantasRoute || {};

// View model used to bind to markup when displaying the location sequence
SantasRoute.LocationViewModel = (function () {
    function LocationViewModel(name, lat, lng) {
        this.Name = name;
        this.Latitude = lat;
        this.Longitude = lng;
    }
    return LocationViewModel;
})();

// Main view model that is bound to the page
SantasRoute.RoutingViewModel = (function () {

    function RoutingViewModel() {
        // Ordinally, this data would come from the server, but this is a demo....
        this.startLocation = new SantasRoute.LocationViewModel("North Pole", 85, 0);
        this.usLocations = [
            new SantasRoute.LocationViewModel("Montgomery AL", 32.30, -86.40),
            new SantasRoute.LocationViewModel("Phoenix AZ", 33.43, -112.02),
            new SantasRoute.LocationViewModel("Little Rock AR", 34.92, -92.15),
            new SantasRoute.LocationViewModel("Sacramento CA", 38.52, -121.50),
            new SantasRoute.LocationViewModel("Denver CO", 39.75, -104.87),
            new SantasRoute.LocationViewModel("Hartford CT", 41.73, -72.65),
            new SantasRoute.LocationViewModel("Dover DE", 39.13, -75.47),
            new SantasRoute.LocationViewModel("Washington DC", 38.95, -77.46),
            new SantasRoute.LocationViewModel("Tallahassee FL", 30.38, -84.37),
            new SantasRoute.LocationViewModel("Atlanta GA", 33.65, -84.42),
            new SantasRoute.LocationViewModel("Boise ID", 43.57, -116.22),
            new SantasRoute.LocationViewModel("Springfield IL", 39.85, -89.67),
            new SantasRoute.LocationViewModel("Indianapolis IN", 39.73, -86.27),
            new SantasRoute.LocationViewModel("Des Moines IA", 41.88, -91.70),
            new SantasRoute.LocationViewModel("Topeka KS", 39.07, -95.62),
            new SantasRoute.LocationViewModel("Frankfort KY", 38.20, -84.86),
            new SantasRoute.LocationViewModel("Baton Rouge LA", 30.53, -91.15),
            new SantasRoute.LocationViewModel("Augusta ME", 44.32, -69.80),
            new SantasRoute.LocationViewModel("Annapolis MD", 38.97, -76.50),
            new SantasRoute.LocationViewModel("Boston, MA", 42.37, -71.03),
            new SantasRoute.LocationViewModel("Lansing MI", 42.77, -84.60),
            new SantasRoute.LocationViewModel("St Paul MN", 44.93, -93.05),
            new SantasRoute.LocationViewModel("Jackson MS", 32.32, -90.08),
            new SantasRoute.LocationViewModel("Jefferson City MO", 38.60, -92.17),
            new SantasRoute.LocationViewModel("Helena MT", 46.60, -112.00),
            new SantasRoute.LocationViewModel("Lincoln NE", 40.85, -96.75),
            new SantasRoute.LocationViewModel("Carson City NV", 39.16, -119.75),
            new SantasRoute.LocationViewModel("Concord NH", 43.20, -71.50),
            new SantasRoute.LocationViewModel("Trenton NJ", 40.28, -74.82),
            new SantasRoute.LocationViewModel("Santa Fe NM", 35.62, -106.08),
            new SantasRoute.LocationViewModel("Albany NY", 42.75, -73.80),
            new SantasRoute.LocationViewModel("Raleigh NC", 35.87, -78.78),
            new SantasRoute.LocationViewModel("Bismarck ND", 46.77, -100.75),
            new SantasRoute.LocationViewModel("Columbus OH", 40.00, -82.88),
            new SantasRoute.LocationViewModel("Oklahoma City OK", 35.40, -97.60),
            new SantasRoute.LocationViewModel("Salem OR", 44.92, -123.00),
            new SantasRoute.LocationViewModel("Harrisburg PA", 40.22, -76.85),
            new SantasRoute.LocationViewModel("Providence RI", 41.73, -71.43),
            new SantasRoute.LocationViewModel("Columbia SC", 33.95, -81.12),
            new SantasRoute.LocationViewModel("Pierre SD", 44.38, -100.28),
            new SantasRoute.LocationViewModel("Nashville TN", 36.12, -86.68),
            new SantasRoute.LocationViewModel("Austin TX", 30.30, -97.70),
            new SantasRoute.LocationViewModel("Salt Lake City UT", 40.78, -111.97),
            new SantasRoute.LocationViewModel("Montpelier VT", 44.20, -72.57),
            new SantasRoute.LocationViewModel("Richmond VA", 37.50, -77.33),
            new SantasRoute.LocationViewModel("Olympia WA", 46.97, -122.90),
            new SantasRoute.LocationViewModel("Charleston, WV", 38.37, -81.60),
            new SantasRoute.LocationViewModel("Madison WI", 43.13, -89.33),
            new SantasRoute.LocationViewModel("Cheyenne WY", 41.15, -104.82)
        ];
        this.worldLocations = [
            new SantasRoute.LocationViewModel("Tokyo, Japan", 35.6895, 139.6917),
            new SantasRoute.LocationViewModel("Jakarta, Indonesia", -6.2000, 106.8000),
            new SantasRoute.LocationViewModel("Seoul, South Korea", 37.5665, 126.9780),
            new SantasRoute.LocationViewModel("Delhi, India", 28.6100, 77.2300),
            new SantasRoute.LocationViewModel("Shanghai, China", 31.2000, 121.5000),
            new SantasRoute.LocationViewModel("Manila, Phillipines", 11.3333, 123.0167),
            new SantasRoute.LocationViewModel("Karachi, Pakistan", 24.8600, 67.0100),
            new SantasRoute.LocationViewModel("New York, USA", 40.6700, -73.9400),
            new SantasRoute.LocationViewModel("Sao Paulo, Brazil", -23.5500, -46.6333),
            new SantasRoute.LocationViewModel("Mexico City, Mexico", 19.4328, -99.1333),
            new SantasRoute.LocationViewModel("Cairo, Egypt", 30.0500, 31.2333),
            new SantasRoute.LocationViewModel("Moscow, Russia", 55.7500, 37.6167),
            new SantasRoute.LocationViewModel("Dhaka, Bangladesh", 23.7000, 90.3750),
            new SantasRoute.LocationViewModel("Buenos Aires, Argentina", -34.6033, -58.3817),
            new SantasRoute.LocationViewModel("Istanbul, Turkey", 41.0136, 28.9550),
            new SantasRoute.LocationViewModel("Lagos, Nigeria", 6.4531, 3.3958),
            new SantasRoute.LocationViewModel("Paris, France", 48.8567, 2.3508),
            new SantasRoute.LocationViewModel("Lima, Peru", -12.0433, -77.0283),
            new SantasRoute.LocationViewModel("Kinshasa, Congo", -4.3250, 15.3222),
            new SantasRoute.LocationViewModel("Bogota, Colombia", 4.5981, -74.0758),
            new SantasRoute.LocationViewModel("London, United Kingdom", 51.5072, -0.1275),
            new SantasRoute.LocationViewModel("Taipei, Taiwan", 22.9500, 120.2000),
            new SantasRoute.LocationViewModel("Ho Chi Minh City, Vietnam", 10.7500, 106.6667),
            new SantasRoute.LocationViewModel("Johannesburg, South Africa", -26.2044, 28.0456),
            new SantasRoute.LocationViewModel("Tehran, Iran", 35.6961, 51.4231),
            new SantasRoute.LocationViewModel("Essen, Germany", 51.4508, 7.0131),
            new SantasRoute.LocationViewModel("Bangkok, Thailand", 13.7500, 100.4667),
            new SantasRoute.LocationViewModel("Hong Kong, Hong Kong", 22.2783, 114.1589),
            new SantasRoute.LocationViewModel("Baghdad, Iraq", 33.3250, 44.4220),
            new SantasRoute.LocationViewModel("Toronto, Canada", 43.7000, -79.4000),
            new SantasRoute.LocationViewModel("Kuala Lumpur, Malaysia", 3.1357, 101.6880),
            new SantasRoute.LocationViewModel("Santiago, Chile", -33.4500, -70.6667),
            new SantasRoute.LocationViewModel("Madrid, Spain", 40.4000, -3.6833),
            new SantasRoute.LocationViewModel("Milan, Italy", 45.4667, 9.1833),
            new SantasRoute.LocationViewModel("Luanda, Angola", -8.8383, 13.2344),
            new SantasRoute.LocationViewModel("Singapore, Singapore", 1.3000, 103.8000),
            new SantasRoute.LocationViewModel("Riyadh, Saudi Arabia", 24.6333, 46.7167),
            new SantasRoute.LocationViewModel("Khartoum, Sudan", 15.6333, 32.5333),
            new SantasRoute.LocationViewModel("Yangoon, Myanmar", 16.8000, 96.1500),
            new SantasRoute.LocationViewModel("Abidjan, Cote d'Ivoire", 5.3167, -4.0333),
            new SantasRoute.LocationViewModel("Accra, Ghana", 5.5500, -0.2000),
            new SantasRoute.LocationViewModel("Sydney, Australia", -33.8600, 151.2111),
            new SantasRoute.LocationViewModel("Athens, Greece", 37.9667, 23.7167),
            new SantasRoute.LocationViewModel("Tel Aviv, Israel", 32.0833, 34.8000),
            new SantasRoute.LocationViewModel("Lisbon, Portugal", 38.7138, -9.1394),
            new SantasRoute.LocationViewModel("Katowice, Poland", 50.2667, 19.0167),
            new SantasRoute.LocationViewModel("Tashkent, Uzbekistan", 41.2667, 69.2167),
            new SantasRoute.LocationViewModel("Baku, Azerbaijan", 40.3953, 49.8822),
            new SantasRoute.LocationViewModel("Budapest, Hungary", 47.4719, 19.0503),
            new SantasRoute.LocationViewModel("Beirut, Lebanon", 33.8869, 35.5131),
        ];
        
        // best route found thus far
        this.currentRoute = ko.observableArray([]);
        
        // class to use to display the route on a map
        this.mapper = new SantasRoute.Mapper("map", 15.00, 0.00, 2);
        
        // whether we're currently routing
        this.isRouting = ko.observable(false);
        
        // last percentage update we got from server
        this.percentComplete = ko.observable(0);
        
        // set of locations to be routed
        this.selectedLocations = this.usLocations;
        
        // which radio button is selected
        this.selectedLocationGroup = ko.observable("US");
        
        // last mileage update we got from server
        this.miles = ko.observable("");
        
        // message to display to user
        this.message = ko.observable("");

        // initialize the locations to route
        this.locationGroupChanged();
        this.displayLocationsToRoute();

        // setup the routing class with callbacks from this class
        var ctx = this;
        this.router = new SantasRoute.Router(
            function (data) { ctx.routeUpdated(data); }, 
            function (data) { ctx.routingComplete(data); }, 
            function (data) { ctx.routingProgress(data); }
        );
    }

    // called when start button clicked
    RoutingViewModel.prototype.route = function () {
        // set the current route to be the chosen list of locations
        var curRoute = [this.startLocation];
        curRoute = curRoute.concat(this.selectedLocations);
        this.currentRoute(curRoute);
        this.isRouting(true);
        this.percentComplete(0);

        // reset the map
        this.displayLocationsToRoute();

        // kick off routing 
        var ctx = this;
        this.router.connect(function () {
            ctx.router.route(ctx.startLocation, ctx.selectedLocations);
        });
        this.message("Routing...");
    };
    
    // called when radio button clicked
    RoutingViewModel.prototype.locationGroupChanged = function () {
        if (this.selectedLocationGroup() == "US") {
            this.selectedLocations = this.usLocations;
            this.mapper.setCenterAndZoom(39.07, -95.62, 4);
        } else if (this.selectedLocationGroup() == "World") {
            this.selectedLocations = this.worldLocations;
            this.mapper.setCenterAndZoom(15, 0, 2);
        }
        this.displayLocationsToRoute();
        return true; // otherwise radio button doesnt update itself
    };
    
    // update display given selected locations
    RoutingViewModel.prototype.displayLocationsToRoute = function () {
        
        // update map
        this.mapper.clearShapes();
        var mapPoints = [this.mapper.createPoint(this.startLocation.Latitude, this.startLocation.Longitude, this.startLocation.Name, this.startLocation.Name)];
        for (var i = 0; i < this.selectedLocations.length; i++) {
            var loc = this.selectedLocations[i];
            mapPoints.push(this.mapper.createPoint(loc.Latitude, loc.Longitude, loc.Name, loc.Name));
        }
        this.mapper.addShapes('points', mapPoints);
        
        // set current route info for list
        var curRoute = [this.startLocation];
        curRoute = curRoute.concat(this.selectedLocations);
        this.currentRoute(curRoute);
        this.miles("A LOT OF");
    };
    
    // called when cancel button clicked
    RoutingViewModel.prototype.cancelRouting = function () {
        this.router.cancel();
    };
    
    // update display based on new route info
    RoutingViewModel.prototype.updateCurrentRoute = function (newRouteInfo) {
        
        // update list 
        this.miles(newRouteInfo.Miles.toFixed(0));
        this.currentRoute(newRouteInfo.LocationSequence);

        // refresh the route displayed on the map, leave points alone
        this.mapper.removeShapes('route');
        var route = [];
        for (var idx = 0; idx < this.currentRoute().length; idx++) {
            route.push({ lat: this.currentRoute()[idx].Latitude, lng: this.currentRoute()[idx].Longitude });
        }
        var routeOverlay = this.mapper.createOverlay(route, 5, '#9900ff', 0.4);
        this.mapper.addShapes('route', [routeOverlay]);
    };
    
    // callback for when updated route is received from server
    RoutingViewModel.prototype.routeUpdated = function (newRouteInfo) {
        this.updateCurrentRoute(newRouteInfo);
    };
    
    // callback for when server indicates routing is complete
    RoutingViewModel.prototype.routingComplete = function (finalRouteInfo) {
        this.percentComplete(100);
        this.updateCurrentRoute(finalRouteInfo);
        this.isRouting(false);
        this.message("Routing Complete");
    };
    
    // callback for when server provides new percentage complete
    RoutingViewModel.prototype.routingProgress = function (progressInfo) {
        this.percentComplete(progressInfo.PercentComplete.toFixed(0));
        this.message("Routing..." + this.percentComplete() + "% of processing complete.");
    };
    
    // resize map given updated size of its container.  Limiting max to avoid MQ display quirks 
    RoutingViewModel.prototype.resizeMap = function () {
        var mapContainer = $('#routeInfo');
        var width = mapContainer.width() > 900 ? 900 : mapContainer.width();
        var height = .60 * width;
        this.mapper.setSize(width, height);
        var mapDiv = $('#map');
        mapDiv.width = width;
        mapDiv.height = height;
    };

    return RoutingViewModel;
})();
