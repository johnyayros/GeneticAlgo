using JY.GeneticAlgorithm;
using System;
using System.Linq.Expressions;

namespace JY.GeneticAlgorithm.Example
{
    public class Location : IGene
    {
        public int Id { get; set; }
        public double XCoordinate { get; set; }
        public double YCoordinate { get; set; }

        private Func<Location, Location, double> distanceFunction;

        public Location(int id, double x, double y, Func<Location,Location,double> distanceFunction)
        {
            this.Id = id;
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

        // override object.Equals
        public bool Equals (IGene obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            return ((Location)obj).Id == this.Id;
        }
        
        // override object.GetHashCode
        public override int GetHashCode()
        {
            return this.Id;
        }
    }
}