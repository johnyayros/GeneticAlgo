namespace  JY.GeneticAlgorithm
{
    public interface IGene
    {
        double DistanceFrom(IGene other);
        IGene Clone();
    }
}