using JY.GeneticAlgorithm;
using System;
using System.Linq.Expressions;

namespace JY.GeneticAlgorithm.Example
{
    public class Location : IGene
    {
        public double XCoordinate { get; set; }
        public double YCoordinate { get; set; }

        private Func<Location, Location, double> distanceFunction;

        public Location(double x, double y, Func<Location,Location,double> distanceFunction)
        {
            this.XCoordinate = x;
            this.YCoordinate = y;
            this.distanceFunction = distanceFunction;
        }

        public double DistanceFrom(IGene other) 
        {
            return distanceFunction(this, (Location)other);
        }

        public IGene Clone()
        {
            return (IGene)this.MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format("X : {0}, Y: {1}", XCoordinate, YCoordinate);
        }
    }
}