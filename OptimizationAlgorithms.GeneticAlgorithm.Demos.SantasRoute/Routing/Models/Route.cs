using System;
using System.Collections.Generic;
using System.Linq;

namespace OptimizationAlgorithms.GeneticAlgorithm.Demos.SantasRoute.Routing.Models
{
    // Class that represents a candidate solution for our genetic algorithm
    public class Route : IEquatable<Route>
    {        
        // Only thing we care about is sequence of locations
        public List<int> LocationSequence { get; set; }

        public override int GetHashCode()
        {
            return (LocationSequence != null ? LocationSequence.Sum() : 0);
        }

        public bool Equals(Route other)
        {
            return LocationSequence.SequenceEqual(other.LocationSequence);
        }

        public override string ToString()
        {
            return String.Join(",",LocationSequence);
        }
    }
}
