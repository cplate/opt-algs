using System;
using System.Collections.Generic;
using OptimizationAlgorithms.GeneticAlgorithm.Demos.SimpleTSP.Models;

namespace OptimizationAlgorithms.GeneticAlgorithm.Demos.SimpleTSP
{
    class Program
    {
        static void Main(string[] args)
        {
            var sequencer = new RouteLocationSequencer();
            var startLocation = new Location { City = "Milwaukee", State = "WI", Latitude = 43.038903, Longitude = -87.906474 };
            var locations = new List<Location>
                {                    
                    new Location {City = "Madison", State = "WI", Latitude = 43.073052, Longitude = -89.401230},
                    new Location {City = "Green Bay", State = "WI", Latitude = 44.519159, Longitude = -88.019826},
                    new Location {City = "Kenosha", State = "WI", Latitude = 42.584743, Longitude = -87.821185},
                    new Location {City = "Racine", State = "WI", Latitude = 42.726131, Longitude = -87.782852},
                    new Location {City = "Appleton", State = "WI", Latitude = 44.261931, Longitude = -88.415385},
                    new Location {City = "Waukesha", State = "WI", Latitude = 43.011678, Longitude = -88.231481},
                    new Location {City = "Oshkosh", State = "WI", Latitude = 44.024706, Longitude = -88.542614},
                    new Location {City = "Eau Claire", State = "WI", Latitude = 44.811349, Longitude = -91.498494},
                    new Location {City = "West Allis", State = "WI", Latitude = 43.016681, Longitude = -88.007031},
                    new Location {City = "Janesville", State = "WI", Latitude = 42.682789, Longitude = -89.018722},
                    new Location {City = "La Crosse", State = "WI", Latitude = 43.801356, Longitude = -91.239581},
                    new Location {City = "Sheboygan", State = "WI", Latitude = 43.750828, Longitude = -87.714530},
                    new Location {City = "Wauwatosa", State = "WI", Latitude = 43.049457, Longitude = -88.007588},
                    new Location {City = "Fond du Lac", State = "WI", Latitude = 43.773045, Longitude = -88.447051},
                    new Location {City = "Brookfield", State = "WI", Latitude = 43.060567, Longitude = -88.106479},
                    new Location {City = "Wausau", State = "WI", Latitude = 44.959135, Longitude = -89.630122},
                    new Location {City = "New Berlin", State = "WI", Latitude = 42.976403, Longitude = -88.108422},
                    new Location {City = "Beloit", State = "WI", Latitude = 42.508348, Longitude = -89.031776},
                    new Location {City = "Greenfield", State = "WI", Latitude = 42.961404, Longitude = -88.012587},
                    new Location {City = "Manitowoc", State = "WI", Latitude = 44.088606, Longitude = -87.657584},
                };

            var route = sequencer.Sequence(startLocation, locations);

            Console.WriteLine("BEST ROUTE");
            Console.WriteLine("0: {0}",startLocation.City);
            for (var idx = 0; idx < route.LocationSequence.Count; idx++)
            {
                Console.WriteLine("{0}: {1},{2}",idx+1,locations[route.LocationSequence[idx]].City,locations[route.LocationSequence[idx]].State);
            }
            Console.ReadKey();
        }
    }
}
