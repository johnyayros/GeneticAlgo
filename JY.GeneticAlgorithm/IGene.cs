using System;

namespace  JY.GeneticAlgorithm
{
    public interface IGene : IEquatable<IGene>
    {
        double DistanceFrom(IGene other);
        IGene Clone();
    }
}