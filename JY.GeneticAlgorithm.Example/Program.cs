using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using JY.GeneticAlgorithm;

namespace JY.GeneticAlgorithm.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var locations = LoadLocations();

            var fitnessFunction = new Func<Individual<Location>,double>(
                (Individual<Location> solution) => {
                    var total = 0.0;
                    for (var i=1; i<solution.Genes.Count; ++i)
                    {
                        var prev = solution.Genes[i-1];
                        total += solution.Genes[i].DistanceFrom(prev);
                    }
                    return total;
                }
            );

            var population = new Population<Location>(locations, 
                                                        fitnessFunction,
                                                        50, 
                                                        .5, 
                                                        true);

            var output = new Action<string>(i => Console.WriteLine(i));
            var final = population.RunGeneticAlgorithm(100, output);
        }

        public static List<Location> LoadLocations()
        {
            var filepath = Path.Combine(
                Directory.GetCurrentDirectory().ToString(), 
                "data.txt");
            
            var locations = new List<Location>();

            var distanceFunction = new Func<Location,Location,double>(
                (Location thisLocation, Location otherLocation) => {
                    var xDistance = Math.Abs(thisLocation.XCoordinate - otherLocation.XCoordinate);
                    var yDistance = Math.Abs(thisLocation.YCoordinate - otherLocation.YCoordinate);
                    return Math.Sqrt( (xDistance*xDistance) + (yDistance*yDistance) );      
                }
            );
            
            foreach (string line in File.ReadLines(filepath).Skip(8)) //skip the header
            {
                if (line == "EOF")
                    break;

                var coords = line.Split(' ');
                locations.Add(new Location(Double.Parse(coords[1]), 
                                            Double.Parse(coords[2]), 
                                            distanceFunction));
            }

            return locations;
        }
    }
}
