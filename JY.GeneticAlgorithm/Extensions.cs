using System;
using System.Collections.Generic;
using System.Linq;

namespace  JY.GeneticAlgorithm
{
    internal static class Extensions {
        private static Random rng = new Random();
        //http://stackoverflow.com/questions/273313/randomize-a-listt
        public static void Shuffle<T>(this IList<T> list)  
        {  
            int n = list.Count;  
            while (n > 1) 
            {  
                n--;  
                int k = rng.Next(n + 1);  
                T value = list[k];  
                list[k] = list[n];  
                list[n] = value;  
            }  
        }

        public static IList<T> Clone<T>(this IList<T> listToClone) where T: IGene
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }       
    }
}
