using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNALib.GeneticAlgorithm
{
    internal static class CrossoverAlgorithms
    {
        private static readonly Random GEN = new Random();

        public static (TGene[], TGene[]) SimpleCrossover<TGene>(TGene[] parent1, TGene[] parent2)
        {
            var length = parent1.Length;
            TGene[] child1 = new TGene[length];
            TGene[] child2 = new TGene[length];

            var point1 = GEN.Next(length);
            var point2 = GEN.Next(length);
            if (point2 > point1)
            {
                var temp = point2;
                point2 = point1;
                point1 = temp;
            }

            for (int i = point1; i < point2; ++i)
            {
                child1[i] = parent1[i];
                child2[i] = parent2[i];
            }

            for (int i = point2; i < length; ++i)
            {
                child1[i] = parent2[i];
                child2[i] = parent1[i];
            }
            for (int i = 0; i < point1; ++i)
            {
                child1[i] = parent2[i];
                child2[i] = parent1[i];
            }

            return (child1, child2);
        }

        public static (TGene[], TGene[]) PartialCrossover<TGene>(TGene[] parent1, TGene[] parent2)
        {
            var length = parent1.Length;
            TGene[] child1 = new TGene[length];
            TGene[] child2 = new TGene[length];

            var point1 = GEN.Next(length);
            var point2 = GEN.Next(length);
            if (point2 > point1)
            {
                var temp = point2;
                point2 = point1;
                point1 = temp;
            }

            SortedDictionary<TGene, TGene> swapper = new SortedDictionary<TGene, TGene>();
            for (int i = point1; i <= point2; ++i)
            {
                swapper.Add(parent1[i], parent2[i]);
                swapper.Add(parent2[i], parent1[i]);
            }

            for (int i = 0; i < length; ++i)
            {
                TGene gene1 = parent1[i];
                child1[i] = swapper.TryGetValue(gene1, out TGene gene) ? gene : gene1;

                TGene gene2 = parent2[i];
                child2[i] = swapper.TryGetValue(gene2, out gene) ? gene : gene2;
            }

            return (child1, child2);
        }
    }
}
