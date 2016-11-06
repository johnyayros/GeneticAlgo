using JY.GeneticAlgorithm;
using System;
using System.Linq.Expressions;

namespace JY.GeneticAlgorithm.Example
{
    public class Location : IGene
    {
        public double XCoordinate { get; set; }
        public double YCoordinate { get; set; }

        private Func<IGene,double> distanceFunction;

        public Location(double x, double y, Func<IGene,double> distanceFunction)
        {
            XCoordinate = x;
            YCoordinate = y;
            this.distanceFunction = distanceFunction;
        }

        public double DistanceFrom(IGene other) 
        {
            return distanceFunction(other);
        }
    }
}