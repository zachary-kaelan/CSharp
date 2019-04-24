using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SapphoLib;

namespace CampSchedules_v3.GeneticAlgorithm
{
    // one individual in a population
    // a single potential solution - in whole - to a problem
    public class Chromosome
    {
        private const float TWIN_RATE = 0.003f;
        internal static readonly Random GEN = new Random(324793728);
        internal static ByteGenerator CROSSOVER_GEN { get; set; }

        // components of the solution
        // order does not matter
        private Gene[] Genes { get; set; }
        private Func<GeneParameter[]> GetGeneParameters;

        public EvoStats Stats { get; private set; }

        public Chromosome(Gene[] genes, Func<GeneParameter[]> getGeneParams)
        {
            Genes = genes;
            GetGeneParameters = getGeneParams;
        }

        private const byte INDEX_DAY = 0;
        private const byte INDEX_TIME = 1;
        private const byte INDEX_ACTIVITY = 2;
        private const byte INDEX_DORM = 3;
        private const byte INDEX_OTHERDORM = 4;

        private const byte MAX_DAY_INDEX = 4;

        public IEnumerable<byte> GetGenesByImmutables(params byte[] immutables)
        {
            var immutor = ((IEnumerable<byte>)immutables).GetEnumerator();

            byte min = 0;
            byte max = GenePool.NUM_GENES;

            if (immutor.MoveNext())
            {
                // genes are ordered by day and time first
                var day = immutor.Current;
                //var dayInfo = Constants.SURVIVAL_DAYINFO[day];

                if (day > 3)
                {
                    for (byte d = MAX_DAY_INDEX; d > day; --d)
                    {
                        //max -= SimpleGenePool.GENES_PER_DAY;
                        throw new NotImplementedException();
                    }
                    //min = (byte)(max - dayInfo.NumGenes);
                }
                else
                {
                    for (byte d = 0; d < day; ++d)
                    {
                        //min += SimpleGenePool.GENES_PER_DAY;
                        throw new NotImplementedException();
                    }
                    //max = (byte)(min + SimpleGenePool.GENES_PER_DAY);
                }

                if (immutor.MoveNext())
                {
                    var time = immutor.Current;
                    var numGenes = time % 2 == 0 ? 17 : 14;
                    //var times = dayInfo.GenesPerTime;

                    if (time > 3)
                    {
                        for (byte t = 3; t > time; --t)
                        {
                            max = (byte)(max - numGenes); 
                        }
                        min = (byte)(max - numGenes);
                    }
                    else
                    {
                        for (byte t = 0; t < time; ++t)
                        {
                            min = (byte)(min + numGenes);
                        }
                        max = (byte)(min + numGenes);
                    }

                    if (immutor.MoveNext())
                    {
                        var activity = immutor.Current;
                        byte currIndex = (byte)((max + min) / 2);

                        bool failed = false;
                        while (Genes[currIndex][INDEX_ACTIVITY] != activity)
                        {
                            if (currIndex > activity)
                            {
                                if (max == currIndex)
                                {
                                    failed = true;
                                    break;
                                }
                                max = currIndex;
                            }
                            else if (currIndex < activity)
                            {
                                if (min == currIndex)
                                {
                                    failed = true;
                                    break;
                                }
                                min = currIndex;
                            }
                            currIndex = (byte)((max + min) / 2);
                        }

                        byte indexTemp = currIndex;
                        while (Genes[indexTemp][INDEX_ACTIVITY] == activity && currIndex > min)
                            --indexTemp;
                        ++indexTemp;
                        min = indexTemp;

                        indexTemp = currIndex;
                        while (Genes[indexTemp][INDEX_ACTIVITY] == activity && currIndex < max)
                            ++indexTemp;
                        max = indexTemp;
                    }
                }
            }

            immutor.Dispose();

            for (byte g = min; g < max; ++g)
            {
                yield return g;
            }

            yield break;
        }

        public EvoStats Select()
        {
            EvoStats stats = new EvoStats();
            if (!Survives(out float fertility))
                stats.Fertility = fertility;
            else
            {
                (float fitness, float minFitness) = Fitness();
                stats.Fitness = fitness;
                stats.MinGeneFitness = minFitness;
                stats.Fertility = (fitness + minFitness) / 2;
            }
            Stats = stats;
            return stats;
        }

        // exclusive activities
        private bool Survives(out float fertility)
        {
            // Dorm, Day, Activity
            var repeats = new List<Tuple<byte, byte, byte>>();
            byte[] failures = new byte[7];

            foreach (var gene in Genes)
            {
                var dormInfo = Constants.SURVIVAL_DORMINFO[gene[INDEX_DORM]];
                var activity = gene[INDEX_ACTIVITY];
                var activityInfo = Constants.SURVIVAL_ACTIVITYINFO[activity];

                var otherDorm = gene[INDEX_OTHERDORM];
                if (otherDorm != 255)
                {
                    if (
                        !dormInfo.AllowedOtherDorms.Contains(otherDorm) ||
                        !activityInfo.MultiDorm.HasFlag(ActivityMultiDorm.Multi)
                    )
                        ++failures[0];

                    var otherDormInfo = Constants.SURVIVAL_DORMINFO[otherDorm];

                    if (
                        activityInfo.IsExclusive && (
                            !dormInfo.AllowedExclusives.Contains(activity) ||
                            !otherDormInfo.AllowedExclusives.Contains(activity)
                        )
                    )
                        ++failures[1];
                }
                else
                {
                    if (!activityInfo.MultiDorm.HasFlag(ActivityMultiDorm.Single))
                        ++failures[2];

                    if (activityInfo.IsExclusive && !dormInfo.AllowedExclusives.Contains(activity))
                        ++failures[3];
                }

                var foundRepeats = repeats.FindAll(r => r.Item1 == gene[INDEX_DORM] && r.Item3 == gene[INDEX_ACTIVITY]);
                if (foundRepeats.Any())
                {
                    if (!activityInfo.IsRepeatable)
                        failures[4] += (byte)foundRepeats.Count;
                    failures[5] += (byte)foundRepeats.Count(r => r.Item2 == gene[INDEX_DAY]);
                }

                if (activityInfo.LongerDuration)//&& isOddTime Constants.SURVIVAL_DAYINFO[gene[INDEX_DAY]].TailTimes.Contains(gene[INDEX_TIME]))
                    ++failures[6];
            }

            bool survives = !failures.Any(f => f != 0);
            if (!survives)
            {
                var boundedFailures = failures.Select(f => BoundedNumber.FromUnboundedNumber(f)).ToArray();
                fertility = (
                    BoundedHelpers.Blend(
                        BoundedHelpers.Blend(
                            boundedFailures[0] + boundedFailures[2],
                            boundedFailures[1] + boundedFailures[3],
                            0.667f
                        ),
                        boundedFailures[4] + boundedFailures[5],
                        0.667f
                    ) + boundedFailures[6]
                ).Suppress();
            }
            else
                fertility = -1f;
            
            return survives;
        }

        // Fitness, unlike Fertility, reduces mutation rate
        private (float, float) Fitness()
        {
            float[] geneFitnesses = new float[Genes.Length];
            int index = 0;
            foreach(var gene in Genes)
            {
                var dormInfo = Constants.FITNESS_DORMINFO[gene[INDEX_DORM]];
                var activity = gene[INDEX_ACTIVITY];
                var activityInfo = Constants.FITNESS_ACTIVITYINFO[activity];

                var otherDorm = gene[INDEX_OTHERDORM];
                byte multiplicativePriority = Math.Max((byte)1, activityInfo);
                byte additivePriority = activityInfo;

                if (dormInfo.ActivityPriorities.TryGetValue(activity, out byte priority))
                {
                    multiplicativePriority *= priority;
                    additivePriority += priority;
                }

                if (otherDorm != 255)
                {
                    byte dormPriority = dormInfo.DormPriorities[otherDorm];
                    multiplicativePriority *= dormPriority;
                    additivePriority += dormPriority;

                    if (Constants.FITNESS_DORMINFO[otherDorm].ActivityPriorities.TryGetValue(activity, out priority))
                    {
                        multiplicativePriority *= priority;
                        additivePriority += priority;
                    }

                    // multiply by the number of variables
                    multiplicativePriority *= 4;
                }
                else
                {
                    multiplicativePriority *= 2;
                    /*multiplicativePriority *= ;
                    additivePriority += 1;*/
                }

                float combined = 1 - (additivePriority / ((float)multiplicativePriority));
                float dayScore = 4 - gene[INDEX_DAY];
                var diff = 1 - ( 
                    dayScore < combined ? 
                        combined / dayScore : 
                        dayScore / combined
                );

                geneFitnesses[index] = combined - diff;
                ++index;
            }

            return (geneFitnesses.Average(), geneFitnesses.Min());
        }

        public Chromosome[] CrossoverWith(Chromosome other, int maxOffspring)
        {
            throw new NotImplementedException();

            var geneParams = GetGeneParameters();
            int numOffspring = (int)(maxOffspring * Stats.Fertility * other.Stats.Fertility) + 1;
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
                    for (byte paramIndex = 0; paramIndex < GenePool.NUM_IMMUTABLE; ++paramIndex)
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
                        SortedDictionary<byte, byte> swapper = new SortedDictionary<byte, byte>();

                        for (int p = point1; p <= point2; ++p)
                        {
                            byte parent1 = Genes[p][paramIndex];
                            byte parent2 = other.Genes[p][paramIndex];
                            if (parent1 == parent2) // shared compatibility/incompatibility
                            {
                                if (!Stats.Survives || !other.Stats.Survives) // shared incompatibility
                                {
                                    //child[p] = GenePool.GetGeneParamRandomValue(paramIndex);
                                }
                                else
                                {
                                    //child1Genes[p] = parent1;
                                    //child2Genes[p] = parent2;
                                }
                            }
                            else
                            {
                                swapper.Add(parent1, parent2);
                                swapper.Add(parent2, parent1);
                            }
                        }

                        for (int p = 0; p < Genes.Length; ++p)
                        {
                            // gets this parameter from each gene

                            byte value1 = Genes[p][paramIndex];
                            childGenes[p] = swapper.TryGetValue(value1, out byte value) ? value : value1;
                        }

                        child[paramIndex] = childGenes;
                    }

                    // simple crossover
                    for (byte paramIndex = GenePool.NUM_IMMUTABLE; paramIndex < GenePool.NUM_PARAMS; ++paramIndex)
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
                    for (ushort g = 0; g < numGenes; ++g)
                    {
                        byte[] gene = new byte[geneParams.Length];
                        for (int p = 0; p < geneParams.Length; ++p)
                        {
                            gene[p] = child[p][g];
                        }
                        genes[g] = new Gene(g, gene);
                    }


                    if (twins % 2 == 0)
                        ++twinsIndex;
                    offspring[i] = new Chromosome(genes, GetGeneParameters);
                    for (int k = i + 1; k <= twinsIndex; ++k)
                    {
                        offspring[k] = new Chromosome(genes, GetGeneParameters);
                    }

                    i = twinsIndex - 1;
                }
                else
                {
                    byte[][] child1 = new byte[geneParams.Length][];
                    byte[][] child2 = new byte[geneParams.Length][];

                    // partial crossover
                    for (byte paramIndex = 0; paramIndex < GenePool.NUM_IMMUTABLE; ++paramIndex)
                    {
                        //var geneParam = geneParams[index];
                        (child1[paramIndex], child2[paramIndex]) = PartialCrossover(other, paramIndex);
                    }

                    // simple crossover
                    for (byte paramIndex = GenePool.NUM_IMMUTABLE; paramIndex < GenePool.NUM_PARAMS; ++paramIndex)
                    {
                        (child1[paramIndex], child2[paramIndex]) = SimpleCrossover(other, paramIndex);
                    }

                    Gene[] genes1 = new Gene[numGenes];
                    Gene[] genes2 = new Gene[numGenes];
                    for (ushort g = 0; g < numGenes; ++g)
                    {
                        byte[] gene1 = new byte[geneParams.Length];
                        byte[] gene2 = new byte[geneParams.Length];
                        for (int p = 0; p < geneParams.Length; ++p)
                        {
                            gene1[p] = child1[p][g];
                            gene2[p] = child2[p][g];
                        }
                        genes1[g] = new Gene(g, gene1);
                        genes2[g] = new Gene(g, gene2);
                    }

                    offspring[i] = new Chromosome(genes1, GetGeneParameters);
                    offspring[i + 1] = new Chromosome(genes2, GetGeneParameters);
                }
            }

            return offspring;
        }

        #region Crossover Algorithms
        /*private Chromosome[] GetChildren(Chromosome other, int numOffspring)
        {
            int numCrossovers = numOffspring / 2;
            int numGenes = Genes.Length;

            byte[][] parent1Points = new byte[GenePool.NUM_PARAMS][];
            byte[][] parent2Points = new byte[GenePool.NUM_PARAMS][];

            for (int p = 0; p < GenePool.NUM_PARAMS; ++p)
            {
                byte[] points1 = new byte[numCrossovers];
                byte[] points2 = new byte[numCrossovers];
                for (int o = 0; o < numCrossovers; ++o)
                {
                    var point1 = CROSSOVER_GEN.GetNext();
                    var point2 = CROSSOVER_GEN.GetNext();
                    if (point2 > point1)
                    {
                        var temp = point2;
                        point2 = point1;
                        point1 = temp;
                    }

                    points1[o] = point1;
                    points2[o] = point2;
                }
                parent1Points[p] = points1;
                parent2Points[p] = points2;
            }

            bool oneUnfit = !Stats.Survives || !other.Stats.Survives;
            
            for (int i = 0; i < numCrossovers; ++i)
            {

                for (int paramIndex = 0; paramIndex < GenePool.NUM_IMMUTABLE; ++paramIndex)
                {

                }
                SortedDictionary<byte, byte> swapper = new SortedDictionary<byte, byte>();
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
                for (int p = point1; p <= point2; ++p)
                {
                    byte parent1 = Genes[p][paramIndex];
                    byte parent2 = other.Genes[p][paramIndex];
                    if (parent1 == parent2) // shared compatibility/incompatibility
                    {
                        if (!Stats.Survives || !other.Stats.Survives) // shared incompatibility
                        {
                            child1Genes[p] = GenePool.GetGeneParamRandomValue(paramIndex);
                            child2Genes[p] = GenePool.GetGeneParamRandomValue(paramIndex);
                        }
                        else
                        {
                            child1Genes[p] = parent1;
                            child2Genes[p] = parent2;
                        }
                    }
                    else
                    {
                        swapper.Add(parent1, parent2);
                        swapper.Add(parent2, parent1);
                    }
                }

                for (int p = 0; p < Genes.Length; ++p)
                {
                    if (child1Genes[p] == 0)
                    {
                        // gets this parameter from each gene

                        byte value1 = Genes[p][paramIndex];
                        child1Genes[p] = swapper.TryGetValue(value1, out byte value) ? value : value1;

                        byte value2 = other.Genes[p][paramIndex];
                        child2Genes[p] = swapper.TryGetValue(value2, out value) ? value : value2;
                    }
                }

                offspring[i] = child1Genes;
                ++i;
                offspring[i] = child2Genes;
            }
        }*/

        public byte[][] PartialCrossover(Chromosome other, byte paramIndex, int numOffspring)
        {
            byte[][] offspring = new byte[numOffspring][];

            for (int i = 0; i < numOffspring; ++i)
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
                SortedDictionary<byte, byte> swapper = new SortedDictionary<byte, byte>();
                for (int p = point1; p <= point2; ++p)
                {
                    byte parent1 = Genes[p][paramIndex];
                    byte parent2 = other.Genes[p][paramIndex];
                    if (parent1 == parent2) // shared compatibility/incompatibility
                    {
                        if (!Stats.Survives || !other.Stats.Survives) // shared incompatibility
                        {
                            child1Genes[p] = GenePool.GetGeneParamRandomValue(paramIndex);
                            child2Genes[p] = GenePool.GetGeneParamRandomValue(paramIndex);
                        }
                        else
                        {
                            child1Genes[p] = parent1;
                            child2Genes[p] = parent2;
                        }
                    }
                    else
                    {
                        swapper.Add(parent1, parent2);
                        swapper.Add(parent2, parent1);
                    }
                }

                for (int p = 0; p < Genes.Length; ++p)
                {
                    if (child1Genes[p] == 0)
                    {
                        // gets this parameter from each gene

                        byte value1 = Genes[p][paramIndex];
                        child1Genes[p] = swapper.TryGetValue(value1, out byte value) ? value : value1;

                        byte value2 = other.Genes[p][paramIndex];
                        child2Genes[p] = swapper.TryGetValue(value2, out value) ? value : value2;
                    }
                }

                offspring[i] = child1Genes;
                ++i;
                offspring[i] = child2Genes;
            }

            return offspring;
        }

        public (byte[], byte[]) PartialCrossover(Chromosome other, byte paramIndex)
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
            SortedDictionary<byte, byte> swapper = new SortedDictionary<byte, byte>();
            for (int p = point1; p <= point2; ++p)
            {
                byte parent1 = Genes[p][paramIndex];
                byte parent2 = other.Genes[p][paramIndex];
                if (parent1 == parent2) // shared compatibility/incompatibility
                {
                    if (!Stats.Survives || !other.Stats.Survives) // shared incompatibility
                    {
                        child1Genes[p] = GenePool.GetGeneParamRandomValue(paramIndex);
                        child2Genes[p] = GenePool.GetGeneParamRandomValue(paramIndex);
                    }
                    else
                    {
                        child1Genes[p] = parent1;
                        child2Genes[p] = parent2;
                    }
                }
                else
                {
                    swapper.Add(parent1, parent2);
                    swapper.Add(parent2, parent1);
                }
            }

            for (int p = 0; p < Genes.Length; ++p)
            {
                if (child1Genes[p] == 0)
                {
                    // gets this parameter from each gene

                    byte value1 = Genes[p][paramIndex];
                    child1Genes[p] = swapper.TryGetValue(value1, out byte value) ? value : value1;

                    byte value2 = other.Genes[p][paramIndex];
                    child2Genes[p] = swapper.TryGetValue(value2, out value) ? value : value2;
                }
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

        public void Mutate(float baseMutationChance)
        {
            var mutationChance = Stats.GetMutationMultiplier() * baseMutationChance;
            var geneParams = GetGeneParameters();

            for (ushort g = 0; g < Genes.Length; ++g)
            {
                for (byte paramIndex = 0; paramIndex < GenePool.NUM_IMMUTABLE; ++paramIndex)
                {
                    if (GEN.NextDouble() < mutationChance)
                        Swap(Genes[g], g, paramIndex);
                }
            }

            for (byte paramIndex = GenePool.NUM_IMMUTABLE; paramIndex < GenePool.NUM_PARAMS; ++paramIndex)
            {
                var param = geneParams[paramIndex];
                List<ushort> mutations = new List<ushort>();
                var mutationValues = GenePool.ParamsPossibleValues[paramIndex];

                // figuring out which genes mutate so we know what values are available
                for (ushort g = 0; g < Genes.Length; ++g)
                {
                    int multiplier = 1;
                    if (GEN.NextDouble() < mutationChance)
                    {
                        if (GEN.Next(2) == 1)
                        {
                            var gene = Genes[g];
                            throw new NotImplementedException();
                            //if (!Swap(gene, g, paramIndex))
                            //    ++multiplier;
                            //mutationValues.Remove(gene[paramIndex]);
                        }
                        else
                            mutations.Add(g);
                    }
                    /*else
                        mutationValues.Remove(Genes[g][paramIndex]);*/
                }

                int valuesLeft = mutationValues.Length;
                // stops above 0 because upper bound for generator is exclusive
                for (int mutationsLeft = mutations.Count; mutationsLeft > 0; --mutationsLeft)
                {
                    // it is impossible for the number of mutations to be more than the number of values

                    // generates the index randomly to avoid bias towards genes earlier in the sequence
                    var mutationIndex = GEN.Next(0, mutationsLeft);
                    var valueIndex = GEN.Next(0, valuesLeft);
                    Genes[mutations[mutationIndex]][paramIndex] = mutationValues[valueIndex];

                    --mutationsLeft;
                    mutations.RemoveAt(mutationIndex);

                    //--valuesLeft;
                    //mutationValues.RemoveAt(valueIndex);
                }
            }
        }

        #region Mutation Algorithms
        private byte Swap(Gene gene, ushort geneIndex, byte paramIndex)
        {
            ushort otherIndex = geneIndex;
            do
            {
                otherIndex = CROSSOVER_GEN.GetNext();
            } while (otherIndex == geneIndex);

            Gene otherGene = Genes[otherIndex];
            // this function handles the swap for both genes
            return gene.Swap(otherGene, paramIndex);
        }

        private byte Swap(Gene gene, ushort geneIndex)
        {
            ushort otherIndex = geneIndex;
            do
            {
                otherIndex = CROSSOVER_GEN.GetNext();
            } while (otherIndex == geneIndex);

            Gene otherGene = Genes[otherIndex];
            // this function handles the swap for both genes
            return gene.FullSwap(otherGene);
        }
        #endregion

        public void SerializeTo(string folder, string filename)
        {
            var geneParams = GetGeneParameters();

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            using (FileStream file = new FileStream(filename + ".csm", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                file.Write(BitConverter.GetBytes(Genes.Length), 0, 4);
                file.WriteByte((byte)geneParams.Length);
                file.Write(BitConverter.GetBytes(Stats.Survives), 0, 1);
                file.Write(BitConverter.GetBytes(Stats.Fertility), 0, 4);
                file.Write(BitConverter.GetBytes(Stats.Fitness), 0, 4);
                file.Write(BitConverter.GetBytes(Stats.MinGeneFitness), 0, 4);

                foreach (var gene in Genes)
                {
                    gene.SerializeWith(file);
                }
            }
        }
    }
}
