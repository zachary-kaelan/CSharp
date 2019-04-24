using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNALib.GeneticAlgorithm
{
    // one individual in a population
    // a single potential solution - in whole - to a problem
    public class Chromosome
    {
        private const float TWIN_RATE = 0.003f;
        private static readonly Random GEN = new Random();

        // components of the solution
        // order does not matter
        private Gene[] Genes { get; set; }
        private Func<GeneParameter[]> GetGeneParameters;
        private int[] MutableGeneParameters { get; set; }
        private int[] NonmutableGeneParameters { get; set; }

        public float Fertility { get; private set; }

        public Chromosome(Gene[] genes, Func<GeneParameter[]> getGeneParams, int[] mutableGeneParams, int[] nonmutableGeneParams)
        {
            Genes = genes;
            GetGeneParameters = getGeneParams;
            MutableGeneParameters = mutableGeneParams;
            NonmutableGeneParameters = nonmutableGeneParams;
        }

        public Chromosome[] CrossoverWith(Chromosome other, int maxOffspring)
        {
            var geneParams = GetGeneParameters();
            int numOffspring = (int)(maxOffspring * Fertility * other.Fertility) + 1;
            if (numOffspring % 2 == 1)
                ++numOffspring;
            int numGenes = Genes.Length;

            Chromosome[] offspring = new Chromosome[numOffspring];
            for (int i = 0; i < numOffspring; i += 2)
            {
                int twins = 0;
                int twinsIndex = i;
                while (GEN.NextDouble() < TWIN_RATE && ++twinsIndex < numOffspring)
                {
                    ++twins;
                }

                if (twins > 0)
                {
                    byte[][] child = new byte[geneParams.Length][];

                    // partial crossover
                    foreach(var paramIndex in NonmutableGeneParameters)
                    {
                        var point1 = GEN.Next(Genes.Length);
                        var point2 = GEN.Next(Genes.Length);
                        if (point2 > point1)
                        {
                            var temp = point2;
                            point2 = point1;
                            point1 = temp;
                        }

                        SortedDictionary<byte, byte> swapper = new SortedDictionary<byte, byte>();
                        for (int p = point1; p <= point2; ++p)
                        {
                            byte parent1 = Genes[p][paramIndex];
                            byte parent2 = other.Genes[p][paramIndex];
                            swapper.Add(parent1, parent2);
                            swapper.Add(parent2, parent1);
                        }

                        byte[] childGenes = new byte[Genes.Length];
                        for (int p = 0; p < Genes.Length; ++p)
                        {
                            // gets this parameter from each gene

                            byte value1 = Genes[p][paramIndex];
                            childGenes[p] = swapper.TryGetValue(value1, out byte value) ? value : value1;
                        }

                        child[paramIndex] = childGenes;
                    }

                    // simple crossover
                    foreach(var paramIndex in MutableGeneParameters)
                    {
                        var point1 = GEN.Next(Genes.Length);
                        var point2 = GEN.Next(Genes.Length);
                        if (point2 > point1)
                        {
                            var temp = point2;
                            point2 = point1;
                            point1 = temp;
                        }

                        byte[] childGenes = new byte[Genes.Length];
                        for (int p = point1; p < point2; ++p)
                        {
                            childGenes[p] = Genes[p][paramIndex];
                        }

                        for (int p = point2; p < Genes.Length; ++p)
                        {
                            childGenes[p] = other.Genes[p][paramIndex];
                        }
                        for (int p = 0; p < point1; ++p)
                        {
                            childGenes[p] = other.Genes[p][paramIndex];
                        }

                        child[paramIndex] = childGenes;
                    }

                    Gene[] genes = new Gene[numGenes];
                    for (int g = 0; g < numGenes; ++g)
                    {
                        byte[] gene = new byte[geneParams.Length];
                        for (int p = 0; p < geneParams.Length; ++p)
                        {
                            gene[p] = child[p][g];
                        }
                        genes[g] = new Gene(gene);
                    }


                    if (twins % 2 == 0)
                        ++twinsIndex;
                    offspring[i] = new Chromosome(genes, GetGeneParameters, MutableGeneParameters, NonmutableGeneParameters);
                    for (int k = i + 1; k <= twinsIndex; ++k)
                    {
                        offspring[k] = new Chromosome(genes, GetGeneParameters, MutableGeneParameters, NonmutableGeneParameters);
                    }

                    i = twinsIndex - 1;
                }
                else
                {
                    byte[][] child1 = new byte[geneParams.Length][];
                    byte[][] child2 = new byte[geneParams.Length][];

                    // partial crossover
                    foreach (var paramIndex in NonmutableGeneParameters)
                    {
                        //var geneParam = geneParams[index];
                        (child1[paramIndex], child2[paramIndex]) = PartialCrossover(other, paramIndex);
                    }

                    // simple crossover
                    foreach (var paramIndex in MutableGeneParameters)
                    {
                        (child1[paramIndex], child2[paramIndex]) = SimpleCrossover(other, paramIndex);
                    }

                    Gene[] genes1 = new Gene[numGenes];
                    Gene[] genes2 = new Gene[numGenes];
                    for (int g = 0; g < numGenes; ++g)
                    {
                        byte[] gene1 = new byte[geneParams.Length];
                        byte[] gene2 = new byte[geneParams.Length];
                        for (int p = 0; p < geneParams.Length; ++p)
                        {
                            gene1[p] = child1[p][g];
                            gene2[p] = child2[p][g];
                        }
                        genes1[g] = new Gene(gene1);
                        genes2[g] = new Gene(gene2);
                    }

                    offspring[i] = new Chromosome(genes1, GetGeneParameters, MutableGeneParameters, NonmutableGeneParameters);
                    offspring[i + 1] = new Chromosome(genes2, GetGeneParameters, MutableGeneParameters, NonmutableGeneParameters);
                }
            }

            return offspring;
        }

        #region Crossover Algorithms
        public (byte[], byte[]) PartialCrossover(Chromosome other, int paramIndex)
        {
            var point1 = GEN.Next(Genes.Length);
            var point2 = GEN.Next(Genes.Length);
            if (point2 > point1)
            {
                var temp = point2;
                point2 = point1;
                point1 = temp;
            }

            SortedDictionary<byte, byte> swapper = new SortedDictionary<byte, byte>();
            for (int p = point1; p <= point2; ++p)
            {
                byte parent1 = Genes[p][paramIndex];
                byte parent2 = other.Genes[p][paramIndex];
                swapper.Add(parent1, parent2);
                swapper.Add(parent2, parent1);
            }

            byte[] child1Genes = new byte[Genes.Length];
            byte[] child2Genes = new byte[Genes.Length];
            for (int p = 0; p < Genes.Length; ++p)
            {
                // gets this parameter from each gene

                byte value1 = Genes[p][paramIndex];
                child1Genes[p] = swapper.TryGetValue(value1, out byte value) ? value : value1;

                byte value2 = other.Genes[p][paramIndex];
                child2Genes[p] = swapper.TryGetValue(value2, out value) ? value : value2;
            }
            return (child1Genes, child2Genes);
        }

        public (byte[], byte[]) SimpleCrossover(Chromosome other, int paramIndex)
        {
            var point1 = GEN.Next(Genes.Length);
            var point2 = GEN.Next(Genes.Length);
            if (point2 > point1)
            {
                var temp = point2;
                point2 = point1;
                point1 = temp;
            }

            byte[] child1Genes = new byte[Genes.Length];
            byte[] child2Genes = new byte[Genes.Length];
            for (int p = point1; p < point2; ++p)
            {
                child1Genes[p] = Genes[p][paramIndex];
                child2Genes[p] = other.Genes[p][paramIndex];
            }

            for (int p = point2; p < Genes.Length; ++p)
            {
                child1Genes[p] = other.Genes[p][paramIndex];
                child2Genes[p] = Genes[p][paramIndex];
            }
            for (int p = 0; p < point1; ++p)
            {
                child1Genes[p] = other.Genes[p][paramIndex];
                child2Genes[p] = Genes[p][paramIndex];
            }

            return (child1Genes, child2Genes);
        }
        #endregion

        public void Mutate(KeyValuePair<float, Mutation>[] mutationChances)
        {
            var geneParams = GetGeneParameters();

            for (int g = 0; g < Genes.Length; ++g)
            {
                Gene gene = Genes[g];
                int multiplier = 1;
                foreach (var paramIndex in NonmutableGeneParameters)
                {
                    if (GEN.NextDouble() < mutationChances[0].Key)
                    {
                        if (!Swap(gene, g, paramIndex))
                            ++multiplier;
                    }
                }

                foreach(var paramIndex in MutableGeneParameters)
                {
                    if (GEN.NextDouble() * multiplier < mutationChances[0].Key)
                    {
                        if (GEN.Next(2) == 1)
                        {
                            if (!Swap(gene, g, paramIndex))
                                ++multiplier;
                        }
                        else
                        {

                        }
                    }
                }
            }
        }

        #region Mutation Algorithms
        private bool Swap(Gene gene, int geneIndex, int paramIndex)
        {
            int otherIndex = geneIndex;
            do
            {
                otherIndex = GEN.Next(Genes.Length);
            } while (otherIndex == geneIndex);

            Gene otherGene = Genes[otherIndex];
            return gene.Swap(otherGene, paramIndex);
        }
        #endregion
    }
}
