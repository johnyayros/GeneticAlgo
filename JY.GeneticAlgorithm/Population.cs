
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

            //Initialize the population with a population that is 1/10th the populationSize
            var tempPopulation = (int)Math.Floor(populationSize / 10.0); 
            for (var i = 0; i < tempPopulation; ++i)
            {
                var copy = initialSolution.Clone();
                copy.Shuffle();
                var solution = new Individual<T>(copy,
                                                    mutationRate,
                                                    fitnessFunction,
                                                    crossoverPoint);
                AddIndividual(solution);
            }

            if (output != null)
            {
                output(string.Format(
                    "Starting optimization. Initial population has been seeded with {0} individuals.", 
                    individuals.Count));
                
                individuals.ForEach(t => {
                    if (fittest == null || t.Fitness < fittest.Fitness)
                            fittest = t;
                    
                    output((string.Format("Fitness = {0}", t.Fitness)));
                });
            }
                    

            fittest = individuals[random.Next(0, tempPopulation-1)];

            for (var i = 0; i<maxIterations; ++i)
            {
                if (output != null)
                    output(string.Format("Starting iteration: {0}", i));

                var pre = fittest.Fitness;

                Evolve(output);

                if (output != null)
                    output(string.Format("Iteration: {0}, Average fitness: {1}, Fittest: {2}, Difference: {3}", 
                                i, avgFitness, fittest.Fitness, fittest.Fitness - pre));
            }
            return fittest;
        }

        private static Random random = new Random();

        public void Evolve(Action<string> output = null)
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
                if ((t.Fitness > avgFitness) && ((individuals.Count - toRemove.Count) > 2)) 
                {
                    toRemove.Add(t);
                } 
                else 
                {
                    if (fittest == null || fittest.Fitness > t.Fitness) 
                    {
                        fittest = new Individual<T>(t);
                        fittest.CalcFitness();
                    }
                }
            }

            toRemove.ForEach(t => RemoveIndividual(t));

            if (output != null)
                output(string.Format("Completed interation and pruning; {0} items removed.", toRemove.Count));
        }
    }
}
