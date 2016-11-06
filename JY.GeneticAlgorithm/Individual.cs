using System;
using System.Collections.Generic;
using System.Linq;

namespace  JY.GeneticAlgorithm
{
    public class Individual<T> where T : IGene
    {
        private static Random random = new Random();

        private IList<T> genes;
        private double mutationRate;
        private int crossoverPoint;
        private Func<double> fitnessFunction;
        private double fitness  = 0;

        public double GetFitness()
        {
            return fitness;
        }

        public Individual(IList<T> genes, 
                            double mutationRate, 
                            Func<double> fitnessFunction, 
                            int crossoverPoint = -1)
        {
            if (mutationRate < 0 || mutationRate > 1)
                throw new ArgumentException("Mutation rate must be between 0 and 1");

            this.genes = genes;
            this.mutationRate = mutationRate;
            this.fitnessFunction = fitnessFunction;

            if (crossoverPoint < 0 && genes.Count > 1)
                this.crossoverPoint = (int)Math.Floor(genes.Count / 2.0);
        }

        public Individual(Individual<T> other)
        {
            this.genes = other.genes;
            this.mutationRate = other.mutationRate;
        }

        public Individual<T> Mate(Individual<T> father)
        {
            var count = crossoverPoint;
            var result = new T[genes.Count];
            genes.CopyTo(result, count);
            
            foreach (var item in father.genes)
            {
                if (!result.Contains(item))
                        result[count++] = item;
            }

            return new Individual<T>(result, 
                                    this.mutationRate, 
                                    this.fitnessFunction, 
                                    this.crossoverPoint);
        }

        void Copy(Individual<T> other)
        {
            throw new NotImplementedException();
        }

        
        void Mutate()
        {
            var i = random.NextDouble();
            
            if (i >= mutationRate)
                return;
            
            var idx1 = random.Next(0, genes.Count - 1);
            var idx2 = random.Next(0, genes.Count - 1);
            var gene1 = genes[idx1];
            var gene2 = genes[idx2];
            genes[idx1] = gene2;
            genes[idx2] = gene1;
        }
    }
}