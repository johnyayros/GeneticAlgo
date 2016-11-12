
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
        private readonly Func<Individual<T>,double> fitnessFunction;

        private Individual<T> fittest;
        public Individual<T> Fittest
        {
            get 
            {
                return this.fittest;
            }
        }

        public Population(IList<T> initialSolution, 
                                    Func<Individual<T>,double> fitnessFunction, 
                                    int populationSize, 
                                    double mutationRate, 
                                    bool elitist,
                                    int crossoverPoint = -1)
        {
            if (mutationRate < 0 || mutationRate > 1)
                throw new ArgumentException("Mutation rate must be between 0 and 1");

            if (fitnessFunction == null)
                throw new ArgumentException("Fitness function cannot be null.");
            
            this.initialSolution = initialSolution;
            this.fitnessFunction = fitnessFunction;
            this.populationSize = populationSize;
            this.mutationRate = mutationRate;
            this.elitist = elitist;
            this.crossoverPoint = crossoverPoint;
        }

        private double totalFitness = 0.0, avgFitness = 0.0;

        public void AddIndividual(Individual<T> individual)
        {
            if (individual.Fitness == 0)
                individual.CalcFitness();
            individuals.Add(individual);
            totalFitness += individual.Fitness;
            avgFitness = totalFitness / individuals.Count;
        }

        public void RemoveIndividual(Individual<T> individual)
        {
            individuals.Remove(individual);
            totalFitness -= individual.Fitness;
            avgFitness = totalFitness / individuals.Count;
        }

        public Individual<T> RunGeneticAlgorithm(int maxIterations = 100, Action<string> output = null)
        {
            individuals = new List<Individual<T>>();

            //Initialize the population
            for (var i = 0; i < populationSize; ++i)
            {
                var copy = initialSolution.Clone();
                copy.Shuffle();
                var solution = new Individual<T>(initialSolution,
                                                    mutationRate,
                                                    fitnessFunction,
                                                    crossoverPoint);
                AddIndividual(solution);
            }

            fittest = individuals[random.Next(0, populationSize-1)];

            for (var i = 0; i<maxIterations; ++i)
            {
                Evolve();
                if (output != null)
                    output(string.Format("Iteration: {0}, Average fitness: {1}, Fittest: {2}", i, avgFitness, fittest.Fitness));
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

                father = individuals[random.Next(0, individuals.Count - 1)]; //todo: what if father == mother?
                offspring.Add(mother.Mate(father));
            }

            offspring.ForEach(t => AddIndividual(t));

            //prune the population
            var toRemove = new List<Individual<T>>();

            foreach (Individual<T> t in individuals) 
            {
                if (t.Fitness > avgFitness && individuals.Count > 2) 
                {
                    toRemove.Add(t);
                } 
                else 
                {
                    if (fittest == null || fittest.Fitness > t.Fitness) 
                    {
                        fittest = new Individual<T>(t);
                    }
                }
            }

            toRemove.ForEach(t => RemoveIndividual(t));
        }
    }
}
