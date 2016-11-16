using System;
using System.Collections.Generic;
using System.Linq;

namespace JY.GeneticAlgorithm
{
    public class Individual<T> where T : IGene
    {
        private static Random random = new Random();

        private IList<T> genes;
        private double mutationRate;
        private int crossoverPoint;
        private Func<Individual<T>, double> fitnessFunction;
        private double fitness = 0;

        public IList<T> Genes
        {
            get
            {
                return genes;
            }
        }

        public double Fitness
        {
            get
            {
                return fitness;
            }
        }

        public Individual(IList<T> genes,
                            double mutationRate,
                            Func<Individual<T>, double> fitnessFunction,
                            int crossoverPoint = -1)
        {
            if (mutationRate < 0 || mutationRate > 1)
                throw new ArgumentException("Mutation rate must be between 0 and 1");

            if (fitnessFunction == null)
                throw new ArgumentException("Fitness function cannot be null.");

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
            this.fitnessFunction = other.fitnessFunction;
            this.crossoverPoint = other.crossoverPoint;
        }

        internal Individual<T> Mate(Individual<T> father)
        {
            var resultGenes = new List<T>(genes.Count);
            resultGenes.AddRange(genes.Take(crossoverPoint));
            var diff = genes.Except(resultGenes.ToArray());
            resultGenes.AddRange(diff);

            var result = new Individual<T>(resultGenes,
                                    this.mutationRate,
                                    this.fitnessFunction,
                                    this.crossoverPoint);

            result.fitness = result.fitnessFunction(result);
            result.Mutate();
            return result;
        }

        internal void Mutate()
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

        internal void CalcFitness()
        {
            fitness = fitnessFunction(this);
        }
    }
}