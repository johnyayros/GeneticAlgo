
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
        private double avgFitness;

        private Individual<T> fittest;
        public Individual<T> Fittest
        {
            get {
                return this.fittest;
            }
        }

        public Population(int populationSize, double mutationRate, bool elitist)
        {
            this.populationSize = populationSize;
            this.mutationRate = mutationRate;
            this.elitist = elitist;
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
                if (t.GetFitness() < avgFitness && individuals.Count > 2) {
                    individuals.Remove(t);
                } else {
                    totalFitness += t.GetFitness();
                    if (fittest == null || fittest.GetFitness() < t.GetFitness()) {
                        fittest = new Individual<T>(t);
                    }
                }
                avgFitness = totalFitness / individuals.Count;
            }
        }
    }
}
