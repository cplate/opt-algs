using System;
using System.Collections.Generic;
using OptimizationAlgorithms.GeneticAlgorithm.Demos.SantasRoute.Routing.Models;

namespace OptimizationAlgorithms.GeneticAlgorithm.Demos.SantasRoute.Routing
{
    public class DistanceCalculator
    {
        // Generate distances between each pair of locations
        public double[,] GetDistanceMatrix(List<Location> locations)
        {
            var matrix = new double[locations.Count,locations.Count];

            for (var oIdx = 0; oIdx < locations.Count; oIdx++)
            {
                for (var dIdx = 0; dIdx < locations.Count; dIdx++)
                {
                    matrix[oIdx, dIdx] = GetDistance(locations[oIdx].Latitude, locations[oIdx].Longitude,
                                                     locations[dIdx].Latitude, locations[dIdx].Longitude);
                }
            }

            return matrix;
        }

        public double GetDistance(double originLatitude, double originLongitude, double destinationLatitude,
                                  double destinationLongitude)
        {
            // Haversine formula
			double radianOriginLatitude = degreesToRadians(originLatitude);
			double radianOriginLongitude = degreesToRadians(originLongitude);
			double radianDestinationLatitude = degreesToRadians(destinationLatitude);
			double radianDestinationLongitude = degreesToRadians(destinationLongitude);
			double latitudeDifference = radianOriginLatitude - radianDestinationLatitude;
			double longitudeDifference = radianOriginLongitude - radianDestinationLongitude;
			double sinLatitude = Math.Sin(latitudeDifference / 2.0);
			double sinLongitude = Math.Sin(longitudeDifference / 2.0);
			double a = Math.Pow(sinLatitude, 2.0) + Math.Cos(radianOriginLatitude) * Math.Cos(radianDestinationLatitude) * Math.Pow(sinLongitude, 2.0);

			const double earthRadiusMiles = 3959.0;
			double distance = earthRadiusMiles * 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));

			return distance;
        }

        private double degreesToRadians(double degreeVal)
		{
			return (degreeVal * (Math.PI / 180));
		}
    }
}
