
using System;
using System.Collections.Generic;

namespace  JY.GeneticAlgorithm
{
    public class Population<T> where T : IGene
    {
        private List<Individual<T>> individuals;
        private readonly int populationSize;
        private readonly double mutationRate;
        private readonly bool elitist;
        private readonly IList<T> initialSolution;
        private readonly int crossoverPoint;
        private readonly Func<Individual<IGene>,double> fitnessFunction;
        private double avgFitness;

        private Individual<T> fittest;
        public Individual<T> Fittest
        {
            get 
            {
                return this.fittest;
            }
        }

        public Population(IList<T> initialSolution, 
                                    Func<Individual<IGene>,double> fitnessFunction, 
                                    int populationSize, 
                                    double mutationRate, 
                                    bool elitist,
                                    int crossoverPoint = -1)
        {
            if (mutationRate < 0 || mutationRate > 1)
                throw new ArgumentException("Mutation rate must be between 0 and 1");

            this.populationSize = populationSize;
            this.mutationRate = mutationRate;
            this.elitist = elitist;
            this.initialSolution = initialSolution;
            this.crossoverPoint = crossoverPoint;
            this.fitnessFunction = fitnessFunction;
        }

        private void Initialize(IList<Individual<T>> individuals)
        {
            this.individuals = new List<Individual<T>>();
            individuals.Shuffle();
            foreach (var i in individuals) 
            {
                this.individuals.Add(i);
            }
        }

        public Individual<T> RunGeneticAlgorithm(int maxIterations = 100, Action<string> output = null)
        {
            this.individuals = new List<Individual<T>>();

            //Initialize the population
            for (var i = 0; i < populationSize; ++i)
            {
                var copy = initialSolution.Clone();
                copy.Shuffle();
                var solution = new Individual<T>(initialSolution,
                                                    mutationRate,
                                                    fitnessFunction,
                                                    crossoverPoint);
                individuals.Add(solution);
            }

            for (var i = 0; i<maxIterations; ++i)
            {
                Evolve();
                if (output != null)
                    output(string.Format("Iteration: {0}, Average fitness: {1}", i, avgFitness));
            }
            return fittest;
        }

        private static Random random = new Random();

        public void Evolve()
        {
            var offspring = new List<Individual<T>>();
            Individual<T> mother, father;
            
            while ((offspring.Count + individuals.Count) < populationSize)
            {
                if (elitist)
                    mother = fittest;
                else
                    mother = individuals[random.Next(0, individuals.Count - 1)];

                father = individuals[random.Next(0, individuals.Count - 1)];
                offspring.Add(mother.Mate(father));
            }

            offspring.ForEach(t => individuals.Add(t));

            //prune the entire population
            double totalFitness = 0;

            foreach (Individual<T> t in individuals) {
                if (t.Fitness < avgFitness && individuals.Count > 2) 
                {
                    individuals.Remove(t);
                } 
                else 
                {
                    totalFitness += t.Fitness;
                    if (fittest == null || fittest.Fitness < t.Fitness) 
                    {
                        fittest = new Individual<T>(t);
                    }
                }
                avgFitness = totalFitness / individuals.Count;
            }
        }
    }
}
