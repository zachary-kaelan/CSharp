using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SapphoLib;
using ZachLib;
using ZachLib.Logging;

namespace CampSchedules_v3.SimpleGeneticAlgorithm
{
    public class SimpleChromosome : IComparable<SimpleChromosome>
    {
        private const float TWIN_RATE = 0.003f;
        internal static readonly Random GEN = new Random(324793728);
        internal static ByteGenerator CROSSOVER_GEN { get; set; }
        private static ByteGenerator SWAP_GEN = new ByteGenerator(2);
        private static ushort NUM_GENES => SimpleGenePool.NUM_GENES;

        // 0 is reserved as parents for initial chromosomes
        private static long CHROMOSOME_ID_COUNTER = 1;

        // components of the solution
        // order does not matter
        internal SimpleGene[] Genes { get; set; }
        private bool _logBufferFlushed = false;
        private List<LogUpdate> LogBuffer = new List<LogUpdate>();

        public EvoStats Stats { get; private set; }
        public long ChromosomeID { get; private set; }
        public KeyValuePair<long, long> ParentIDs { get; private set; }

        public float MutationRate { get; private set; }
        public float PreservationRate { get; private set; }

        public byte Age = 0;

        // generate random
        public SimpleChromosome()
        {
            double chance = GEN.NextDouble();

        }

        private static SimpleChromosome Random_Full()
        {
            SimpleGene[] chromosome = new SimpleGene[NUM_GENES];
            for (ushort g = 0; g < NUM_GENES; ++g)
            {
                chromosome[g] = new SimpleGene(g);
            }
            return new SimpleChromosome(chromosome);
        }

        public SimpleChromosome(SimpleGene[] genes)
        {
            Genes = genes;
            MutationRate = SimpleGenePool.MutationChance;
            ParentIDs = new KeyValuePair<long, long>(0, 0);
            ChromosomeID = CHROMOSOME_ID_COUNTER;
            ++CHROMOSOME_ID_COUNTER;
            _logBufferFlushed = true;
        }

        public SimpleChromosome(SimpleGene[] genes, float mutationRate, float preservationRate, KeyValuePair<long, long> parents)
        {
            Genes = genes;
            MutationRate = mutationRate;
            PreservationRate = preservationRate;
            ParentIDs = parents;
            ChromosomeID = CHROMOSOME_ID_COUNTER;
            ++CHROMOSOME_ID_COUNTER;

            LogBuffer.Add(
                new LogUpdate(
                    "GeneticScheduling",
                    EntryType.DEBUG,
                    String.Format(
                        "Chromosome {0} has been born from {1} and {2}",
                        ChromosomeID,
                        parents.Key,
                        parents.Value
                    ),
                    ""
                )
            );

            Debug.Assert(!Genes.Skip(1).Any(g => g.ID == 0));
        }

        private const byte INDEX_DAY = 0;
        private const byte INDEX_TIME = 1;
        private const byte INDEX_ACTIVITY = 2;
        private const byte INDEX_DORM = 3;
        private const byte INDEX_OTHERDORM = 4;

        private const byte MAX_DAY_INDEX = 4;

        public IEnumerable<ushort> GetGenesByImmutables(params byte[] immutables)
        {
            var immutor = ((IEnumerable<byte>)immutables).GetEnumerator();

            ushort min = 0;
            ushort max = (ushort)SimpleGenePool.NUM_GENES;

            if (immutor.MoveNext())
            {
                // genes are ordered by day and time first
                var day = immutor.Current;
                //var dayInfo = Constants.SURVIVAL_DAYINFO[day];

                if (day > 3)
                {
                    for (byte d = MAX_DAY_INDEX; d > day; --d)
                    {
                        max -= SimpleGenePool.GENES_PER_DAY;
                    }
                    min = (byte)(max - SimpleGenePool.GENES_PER_DAY);
                }
                else
                {
                    for (byte d = 0; d < day; ++d)
                    {
                        min += SimpleGenePool.GENES_PER_DAY;
                    }
                    max = (byte)(min + SimpleGenePool.GENES_PER_DAY);
                }

                if (immutor.MoveNext())
                {
                    var time = immutor.Current;
                    ushort numGenes = (ushort)(time % 2 == 0 ? 17 : 14);

                    if (time > 3)
                    {
                        for (byte t = 3; t > time; --t)
                        {
                            max -= numGenes;
                        }
                        min = (ushort)(max - numGenes);
                    }
                    else
                    {
                        for (byte t = 0; t < time; ++t)
                        {
                            min += numGenes;
                        }
                        max = (ushort)(min + numGenes);
                    }

                    if (immutor.MoveNext())
                    {
                        var activity = immutor.Current;
                        ushort currIndex = (ushort)((max + min) / 2);

                        bool failed = false;
                        while (SimpleGenePool.ImmutableValues[currIndex].Activity != activity)
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
                            currIndex = (ushort)((max + min) / 2);
                        }

                        ushort indexTemp = currIndex;
                        while (SimpleGenePool.ImmutableValues[currIndex].Activity == activity && currIndex > min)
                            --indexTemp;
                        ++indexTemp;
                        min = indexTemp;

                        indexTemp = currIndex;
                        while (SimpleGenePool.ImmutableValues[currIndex].Activity == activity && currIndex < max)
                            ++indexTemp;
                        max = indexTemp;
                    }
                }
            }

            immutor.Dispose();

            for (ushort g = min; g < max; ++g)
            {
                yield return g;
            }

            yield break;
        }

        #region EvolutionFunctions

        public EvoStats Select()
        {
            EvoStats stats = new EvoStats();
            if (!Survives(out SurvivalStats survivalStats))
            {
                stats.Fertility = survivalStats.Fertility;
                PreservationRate *= survivalStats.TimesBounded;
                MutationRate = (
                    BoundedHelpers.Blend(
                        SimpleGenePool.MinMutationChance,
                        SimpleGenePool.MaxMutationChance,
                        survivalStats.TimesBounded
                    ) + MutationRate
                ) / 2;
            }
            else
            {
                (float fitness, float minFitness) = Fitness(survivalStats);
                stats.Fitness = fitness;
                stats.MinGeneFitness = minFitness;

                if (survivalStats.Fertility == -1f)
                    stats.Fertility = (fitness + minFitness);
                else
                {
                    stats.Fertility = BoundedHelpers.Blend(
                        new BoundedNumber(fitness),
                        survivalStats.Fertility,
                        0.25f
                    );
                }

                MutationRate = (
                    BoundedHelpers.Blend(
                        SimpleGenePool.MinMutationChance,
                        SimpleGenePool.MutationChance,
                        survivalStats.TimesBounded
                    ) + MutationRate
                ) / 2;
            }

            stats.TotalTimes = survivalStats.TimesTotal;
            Stats = stats;
            return stats;
        }

        // exclusive activities
        private bool Survives(out SurvivalStats survivalStats)
        {
            // Dorm, Day, Activity
            var nonrepeatableHistory = new SortedSet<HistoryEvent>();
            var repeatableHistory = new SortedDictionary<HistoryEvent, byte>();
            //ushort repeatedNonRepeatable = 0;
            //ushort repeatedSameDay = 0;
            //byte otherDormsRerolled = 0;
            byte[] failures = new byte[5];
            var times = new sbyte[SimpleGenePool.BASE_TIMES.Length];
            Array.Copy(SimpleGenePool.BASE_TIMES, times, SimpleGenePool.BASE_TIMES.Length);

            ushort index = 0;
            var currentImmutable = SimpleGenePool.ImmutableValues[0];
            var pairingCounts = new SortedDictionary<DormPairing, byte>();

            for (byte day = 0; day < MAX_DAY_INDEX; ++day)
            {
                SortedSet<HistoryEvent> dayHistory = new SortedSet<HistoryEvent>();
                //var currentDay = Constants.SURVIVAL_DAYINFO[day];

                bool isOddTime = false;
                bool checkLastHistory = false;
                SortedSet<byte> lastTimeReserved = new SortedSet<byte>();
                //List<KeyValuePair<byte, byte>> tailTimeHistory = new List<KeyValuePair<byte, byte>>();

                for (byte time = 0; time < 4/*= currentDay.MaxTime*/; ++time)
                {
                    bool logLastHistory = false;
                    SortedSet<byte> timeHistory = new SortedSet<byte>();

                    var max = isOddTime ? SimpleGenePool.NUM_ODD_ACTIVITIES : SimpleGenePool.NUM_ACTIVITIES;

                    if (isOddTime)
                        isOddTime = false;
                    else
                    {
                        //doTailTime = true;
                        isOddTime = true;
                        if (lastTimeReserved == null)
                        {
                            checkLastHistory = false;
                            logLastHistory = true;
                        }
                    }

                    for (short geneIndex = 0; geneIndex < max; ++geneIndex)
                    {
                        var gene = Genes[index];
                        bool tryAgain = false;

                        // error-checking
                        if (gene.Dorm != 255)
                        {
                            var immutable = SimpleGenePool.ImmutableValues[gene.ID];

                            byte result = SimpleGenePool.ImmutableGenes.ContainsMatch(immutable.Day, immutable.Time, gene.Dorm, gene.OtherDorm);
                            tryAgain = result > 0;
                            if (result == 3)
                            {
                                Genes[index].MutateBothExcl();
                                --geneIndex;
                                continue;
                            }

                            var activityInfo = Constants.SURVIVAL_ACTIVITYINFO[immutable.Activity];
                            var dormInfo = Constants.SURVIVAL_DORMINFO[gene.Dorm];

                            var otherDorm = gene.OtherDorm != 255;

                            if (otherDorm)
                            {
                                if (!activityInfo.MultiDorm.HasFlag(ActivityMultiDorm.Multi))
                                {
                                    Genes[index].SetOtherNull();
                                    otherDorm = false;

                                    if (activityInfo.IsExclusive && !dormInfo.AllowedExclusives.Contains(immutable.Activity))
                                    {
                                        result = 1;
                                        tryAgain = true;
                                    }
                                }
                                else if (result != 2)
                                {
                                    if (
                                        !dormInfo.AllowedOtherDorms.Contains(gene.OtherDorm) || (
                                            (gene.Dorm == 5 || gene.OtherDorm == 6) &&
                                            Constants.FITNESS_DORMINFO[gene.Dorm].DormPriorities.ContainsKey(gene.OtherDorm)
                                        )
                                    ) {
                                        result = (byte)(1 + DormSurvivalInfo.GEN.GetNext(2));
                                        tryAgain = true;
                                    }
                                    /*else
                                    {
                                        var otherDormInfo = Constants.SURVIVAL_DORMINFO[gene.OtherDorm];

                                        if (activityInfo.IsExclusive)
                                        {
                                            if (result != 1 && !dormInfo.AllowedExclusives.Contains(immutable.Activity))
                                                ++result;
                                            if (!otherDormInfo.AllowedExclusives.Contains(immutable.Activity))
                                            {
                                                result += 2;
                                                tryAgain = true;
                                                otherDorm = false;
                                                //++otherDormsRerolled;
                                            }
                                        }
                                    }*/
                                }
                            }
                            else
                            {
                                if (result == 1)
                                {
                                    if (!activityInfo.MultiDorm.HasFlag(ActivityMultiDorm.Single))
                                        result = 3;
                                }
                                else
                                {
                                    if (!activityInfo.MultiDorm.HasFlag(ActivityMultiDorm.Single))
                                        result += 2;
                                    else if (activityInfo.IsExclusive && !dormInfo.AllowedExclusives.Contains(immutable.Activity))
                                        ++result;
                                }
                            }

                            if (tryAgain)
                            {
                                switch (result)
                                {
                                    case 1:
                                        Genes[index].MutatePrimaryExcl();
                                        break;

                                    case 2:
                                        Genes[index].OtherDorm = dormInfo.GetNewOtherDorm();
                                        break;

                                    case 3:
                                        Genes[index].MutateBothExcl();
                                        break;
                                }

                                --geneIndex;
                                continue;
                            }

                            // these are problems that conflict with other genes, so they cannot be fixed immediately 

                            var history = new HistoryEvent(immutable.Activity, 255);

                            if (otherDorm)
                            {
                                --times[gene.OtherDorm];
                                if (activityInfo.LongerDuration)
                                    --times[gene.OtherDorm];

                                history.Dorm = gene.OtherDorm;
                                pairingCounts.Increment(new DormPairing(gene.Dorm, gene.OtherDorm));

                                if (logLastHistory && activityInfo.LongerDuration)
                                {
                                    lastTimeReserved.Add(gene.Dorm);
                                    lastTimeReserved.Add(gene.OtherDorm);
                                }
                                
                                if (!timeHistory.Add(gene.OtherDorm))
                                    ++failures[0];
                                else if (!dayHistory.Add(history))
                                    ++failures[2];

                                if (checkLastHistory && lastTimeReserved.Contains(gene.OtherDorm))
                                    ++failures[1];

                                if (!activityInfo.IsRepeatable)
                                {
                                    if (!nonrepeatableHistory.Add(history))
                                        ++failures[3];
                                }
                                else
                                    repeatableHistory.Increment(history);
                            }
                            else if (logLastHistory && activityInfo.LongerDuration)
                                lastTimeReserved.Add(gene.Dorm);

                            history.Dorm = gene.Dorm;

                            if (!timeHistory.Add(gene.Dorm))
                                ++failures[0];
                            else if (checkLastHistory && lastTimeReserved.Contains(gene.Dorm))
                                ++failures[1];
                            else if (!dayHistory.Add(history))
                                ++failures[2];
                            if (!activityInfo.IsRepeatable)
                            {
                                if (!nonrepeatableHistory.Add(history))
                                    ++failures[3];
                            }
                            else
                                repeatableHistory.Increment(history);

                            if (activityInfo.LongerDuration)
                                --times[gene.Dorm];
                            --times[gene.Dorm];
                        }

                        ++index;
                    }

                    if (logLastHistory && lastTimeReserved.Any())
                        checkLastHistory = true;
                    else if (checkLastHistory)
                        lastTimeReserved = null;
                }
            }

            //Debug.Assert(!Genes.Skip(1).Any(g => g.ID == 0));

            // blank genes are technically fit, so need to be careful or it'll start cutting blocks
            var timesAbsSum = times.Sum(t => Math.Abs(t));
            var timesBounded = BoundedNumber.FromUnboundedNumber(timesAbsSum);

            float fertility = -1f;

            bool survives = !failures.Any(f => f != 0);
            failures[4] = (byte)repeatableHistory.Values.Where(n => n > 2).Sum(n => n - 2);
            float altFertility = 1f / (failures.Sum(f => f) + timesAbsSum);
            if (!survives)
            {
                var boundedFailures = failures.Select(f => BoundedNumber.FromUnboundedNumber(f)).ToArray();
                fertility = 1 - BoundedHelpers.Blend(
                    (
                        BoundedHelpers.Blend(
                            BoundedHelpers.Blend(
                                BoundedHelpers.Blend(
                                    boundedFailures[0],
                                    boundedFailures[1],
                                    0.333f
                                ),
                                boundedFailures[2],
                                0.333f
                            ),
                            boundedFailures[3],
                            0.333f
                        )
                    ),
                    timesBounded + boundedFailures[4],
                    0.250f
                );
            }
            else if (!times.All(t => t != 0))
                fertility = 1 - BoundedHelpers.Blend(0, timesBounded + BoundedNumber.FromUnboundedNumber(failures[4]), 0.667f);

            fertility = altFertility;
            survivalStats = new SurvivalStats(
                fertility,
                timesBounded,
                (short)times.Sum(t => t),
                times,
                repeatableHistory.Where(kv => kv.Value > 2).GroupBy(
                    kv => kv.Key.Activity, kv => kv.Value
                ).ToDictionary(g => g.Key, g => (byte)g.Sum(v => v))
            );
            return survives;
        }

        // Fitness, unlike Fertility, reduces mutation rate
        private (float, float) Fitness(SurvivalStats survivalStats)
        {
            float[] geneFitnesses = new float[Genes.Length];
            //var currentDay = Constants.SURVIVAL_DAYINFO[0];
            byte day = 0;
            ushort dayIndex = 0;
            ushort index = 0;

            var emptyTimeLeft = survivalStats.TimesTotal;

            foreach (var gene in Genes)
            {
                if (gene.Dorm != 255)
                {
                    var immutable = SimpleGenePool.ImmutableValues[index];
                    var dormInfo = Constants.FITNESS_DORMINFO[gene.Dorm];
                    var activityInfo = Constants.FITNESS_ACTIVITYINFO[immutable.Activity];

                    byte multiplicativePriority = Math.Max((byte)1, activityInfo);
                    byte additivePriority = activityInfo;

                    if (dormInfo.ActivityPriorities.TryGetValue(immutable.Activity, out byte priority))
                    {
                        multiplicativePriority *= priority;
                        additivePriority += priority;
                    }

                    if (gene.OtherDorm != 255)
                    {
                        byte dormPriority = dormInfo.DormPriorities[gene.OtherDorm];
                        multiplicativePriority *= dormPriority;
                        additivePriority += dormPriority;

                        if (Constants.FITNESS_DORMINFO[gene.OtherDorm].ActivityPriorities.TryGetValue(immutable.Activity, out priority))
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

                    if (survivalStats.RepeatsExcess.TryGetValue(immutable.Activity, out byte excess))
                        additivePriority += excess;
                    float combined = 1 - (additivePriority / ((float)multiplicativePriority));
                    float dayScore = 4 - day;
                    var diff = 1 - (
                        dayScore < combined ?
                            combined / dayScore :
                            dayScore / combined
                    );
                    geneFitnesses[index] = combined - diff;
                }
                else
                {
                    if (--emptyTimeLeft >= 0)
                        geneFitnesses[index] = 0f;
                    else
                        geneFitnesses[index] = 1f;
                }

                ++dayIndex;
                if (dayIndex == SimpleGenePool.GENES_PER_DAY && day != MAX_DAY_INDEX)
                {
                    dayIndex = 0;
                    ++day;
                }

                ++index;
            }

            return (geneFitnesses.Average(), geneFitnesses.Min());
        }

        internal static ushort MAX_DISTANCE = (ushort)(NUM_GENES * 2);
        public float DistanceFrom(SimpleChromosome other)
        {
            ushort distance = MAX_DISTANCE;
            for (ushort gene = 0; gene < NUM_GENES; ++gene)
            {
                var gene1 = Genes[gene];
                var gene2 = other.Genes[gene];
                if (gene1.Dorm == gene2.Dorm)
                    --distance;
                if (gene1.OtherDorm == gene2.OtherDorm)
                    --distance;
            }
            return (float)Math.Sqrt(distance);
        }

        public SimpleChromosome[] CrossoverWith(SimpleChromosome other, int maxOffspring)
        {
            var fertilityMultiplier = (Stats.Fertility + other.Stats.Fertility) / 2;
            fertilityMultiplier = BoundedHelpers.Blend(1, fertilityMultiplier, 0.75f);
            int numOffspring = Math.Max(2, (int)Math.Round(fertilityMultiplier * (maxOffspring / 2)) * 2);

            KeyValuePair<long, long> parents = new KeyValuePair<long, long>(ChromosomeID, other.ChromosomeID);
            SimpleChromosome[] offspring = new SimpleChromosome[numOffspring];
            int numCrossovers = numOffspring / 2;
            ushort numGenes = (ushort)Genes.Length;

            // instead of asserting a "first parent" and "second parent", the other Chromosome is assumed to be second

            bool oneUnfit = !Stats.Survives || !other.Stats.Survives;

            IEnumerator<SimpleGene> thisEnum = ((IEnumerable<SimpleGene>)Genes).GetEnumerator();
            IEnumerator<SimpleGene> otherEnum = ((IEnumerable<SimpleGene>)other.Genes).GetEnumerator();

            ushort genePointer = 0;
            ushort pointerStop = 0;
            ushort similarityScore = 0; // incest has more mutation to make up with similarities
            // max similarityScore is numGenes * 2

            SimpleGene[] genes1 = null;
            SimpleGene[] genes2 = null;

            //SortedSet<ushort> indices = new SortedSet<ushort>(Enumerable.Range(0, numGenes).Select(i => (ushort)i));

            ushort numEqual = 0;
            //SortedSet<ushort> equal = new SortedSet<ushort>();
            // constants are not included in the chromosomes

            for (ushort gene = 0; gene < numGenes; ++gene)
            {
                var gene1 = Genes[gene];
                var gene2 = other.Genes[gene];
                if (gene1.Dorm == gene2.Dorm)
                {
                    ++similarityScore;
                    if (gene1.OtherDorm == gene2.OtherDorm)
                    {
                        ++similarityScore;
                        /*if (gene1.Dorm == gene2.Dorm)
                        {
                            genes1[gene] = oneUnfit ? new SimpleGene(gene) : gene1;
                            genes2[gene] = new SimpleGene(gene);
                        }
                        else
                        {
                            genes1[genePointer] = gene1;
                            genes2[genePointer] = gene1;
                        }
                        //indices.Remove(gene);
                        equal.Add(gene);*/
                        ++numEqual;
                    }
                }
                else
                {
                    if (gene1.OtherDorm == gene2.OtherDorm)
                        ++similarityScore;
                }
            }
            //Console.Write(numEqual.ToString().PadLeft(3, '0') + ", ");

            float fitness = 0;
            if (oneUnfit)
            {
                fitness = (float)Math.Sqrt(Stats.GetStat() * other.Stats.GetStat());
                fitness *= fitness;
            }
            else
                fitness = (float)Math.Sqrt(Stats.Fitness * other.Stats.Fitness);

            float mutationRate = (MutationRate + other.MutationRate) / 2;
            similarityScore /= 2;
            float incestMutationMultiplier = 0;
            if (similarityScore >= numGenes * 0.5f)
            {
                var x = (2f * similarityScore) / 3f;
                incestMutationMultiplier = (x / (numGenes - x)) + 1;
            }
            else
                incestMutationMultiplier = (similarityScore / (numGenes * 0.5f)) + 1;

            float splitRate = 0;

            if (fitness >= 0.5)
            {
                var distance = 0.5f / (0.5f - fitness);
                var minMutationRate = SimpleGenePool.MinMutationChance * incestMutationMultiplier;
                mutationRate = ((mutationRate - minMutationRate) * distance) + minMutationRate;
                splitRate = minMutationRate / incestMutationMultiplier;
            }
            else
            {
                var distance = (0.5f - fitness) / 0.5f;
                mutationRate += ((SimpleGenePool.MaxMutationChance * incestMutationMultiplier) - mutationRate) * distance;
                splitRate = SimpleGenePool.MutationChance / incestMutationMultiplier;
            }

            float fitnessMean = 0;
            if (oneUnfit)
            {
                fitnessMean = (fitness * 2) / 3;
                fitnessMean *= fitnessMean;
            }
            else
                fitnessMean = (fitness + ((Stats.MinGeneFitness + other.Stats.MinGeneFitness) / 2)) / 2;

            float orderedCrossoverChance = Math.Min((fitnessMean + mutationRate) / incestMutationMultiplier, 1f);
            float OXReduction = fitness <= 0.5 ? fitness : fitnessMean;
            byte offspringIndex = 0;
            float preservationRate = Math.Min(1f, ((float)Math.Sqrt(fitnessMean * (1 - mutationRate))) / incestMutationMultiplier);

            #region Crossover Functions
            // ~~~ SIMPLE CROSSOVER ~~~
            void ToNextPoint1122()
            {
                int length = (pointerStop - genePointer);// - 1;
                Array.Copy(Genes, genePointer, genes1, genePointer, length);
                Array.Copy(other.Genes, genePointer, genes2, genePointer, length);
                while (genePointer < pointerStop)
                {
                    thisEnum.MoveNext();
                    otherEnum.MoveNext();
                    ++genePointer;
                }
            }

            void ToNextPoint2211() // both dorm and other are flipped
            {
                int length = (pointerStop - genePointer);// - 1;
                Array.Copy(Genes, genePointer, genes2, genePointer, length);
                Array.Copy(other.Genes, genePointer, genes1, genePointer, length);
                while (genePointer < pointerStop)
                {
                    thisEnum.MoveNext();
                    otherEnum.MoveNext();
                    ++genePointer;
                }
            }

            void ToNextPoint1221() // first gene: this dorm, other otherDorm; second gene: other dorm, this otherDorm
            {
                while (genePointer < pointerStop)
                {
                    thisEnum.MoveNext();
                    otherEnum.MoveNext();
                    //if (!equal.Contains(genePointer)) { }
                    genes1[genePointer] = new SimpleGene(genePointer, otherEnum.Current.Dorm, thisEnum.Current.OtherDorm);
                    genes2[genePointer] = new SimpleGene(genePointer, thisEnum.Current.Dorm, otherEnum.Current.OtherDorm);
                    ++genePointer;
                }
            }

            void ToNextPoint2112() // first gene: other dorm, this otherDorm; second gene: this dorm, other otherDorm
            {
                while (genePointer < pointerStop)
                {
                    thisEnum.MoveNext();
                    otherEnum.MoveNext();
                    //if (!equal.Contains(genePointer)) { }
                    genes1[genePointer] = new SimpleGene(genePointer, otherEnum.Current.Dorm, thisEnum.Current.OtherDorm);
                    genes2[genePointer] = new SimpleGene(genePointer, thisEnum.Current.Dorm, otherEnum.Current.OtherDorm);
                    ++genePointer;
                }
            }

            // ~~~ SPLIT CROSSOVER ~~~
            void ToNextPoint1212() // dorms are taken from this, otherDorms are taken from other
            {
                while (genePointer < pointerStop)
                {
                    thisEnum.MoveNext();
                    otherEnum.MoveNext();
                    //if (!equal.Contains(genePointer)) { }
                    var gene = new SimpleGene(genePointer, thisEnum.Current.Dorm, otherEnum.Current.OtherDorm);
                    genes1[genePointer] = gene;
                    genes2[genePointer] = gene;
                    ++genePointer;
                }
            }

            void ToNextPoint2121() // dorms are taken from other, otherDorms are taken from this
            {
                while (genePointer < pointerStop)
                {
                    thisEnum.MoveNext();
                    otherEnum.MoveNext();
                    //if (!equal.Contains(genePointer)) { }
                    var gene = new SimpleGene(genePointer, otherEnum.Current.Dorm, thisEnum.Current.OtherDorm);
                    genes1[genePointer] = gene;
                    genes2[genePointer] = gene;
                    ++genePointer;
                }
            }
            #endregion

            for (int c = 0; c < numCrossovers; ++c)
            {
                float thisMutationRate = mutationRate;

                byte pointsUsed = 0;

                bool flipForOther = GEN.NextDouble() < 0.5;
                bool flipForDorm = GEN.NextDouble() < 0.5;
                if (flipForDorm && flipForDorm == flipForOther)
                {
                    flipForDorm = false;
                    flipForOther = false;
                }
                (byte dormPoint1, byte dormPoint2) = GetPoints();
                (byte otherPoint1, byte otherPoint2) = GetPoints();

                genes1 = new SimpleGene[SimpleGenePool.NUM_GENES];
                genes2 = new SimpleGene[SimpleGenePool.NUM_GENES];

                genePointer = 0;
                // doesn't take into account flipping, handled in the ToNextPoint set of functions
                pointerStop = otherPoint1;
                bool secondFirst = false; // carried throughout
                bool dormFirst = false; // by child
                /*if (dormPoint1 < pointerStop)
                {
                    pointerStop = dormPoint1;
                    dormFirst = true;
                }*/
                ushort max = numGenes;

                void GetNextStop()
                {
                    switch(pointsUsed)
                    {
                        case 0:
                            dormFirst = false;
                            if (dormPoint1 < pointerStop)
                            {
                                pointerStop = dormPoint1;
                                dormFirst = true;
                            }
                            break;

                        case 1:
                            if (dormFirst)
                            {
                                pointerStop = otherPoint1;
                                if (dormPoint2 < pointerStop)
                                {
                                    secondFirst = true;
                                    pointerStop = dormPoint2;
                                    //flipForOther = !flipForOther;
                                }
                                //else
                                    flipForDorm = !flipForDorm;
                            }
                            else
                            {
                                pointerStop = dormPoint1;
                                if (otherPoint2 < pointerStop)
                                {
                                    secondFirst = true;
                                    pointerStop = otherPoint2;
                                    //flipForDorm = !flipForDorm;
                                }
                                //else
                                    flipForOther = !flipForOther;
                            }
                            break;

                        case 2:
                            // setting the other flag that wasn't set before
                            if (secondFirst)
                            {
                                if (dormFirst)
                                    flipForDorm = !flipForDorm;
                                else
                                    flipForOther = !flipForOther;
                            }
                            else
                            {
                                if (dormFirst)
                                    flipForOther = !flipForOther;
                                else
                                    flipForDorm = !flipForDorm;
                            }

                            if (secondFirst)
                            {
                                pointerStop = dormFirst ? otherPoint1 : dormPoint1;
                                dormFirst = !dormFirst;
                            }
                            else
                            {
                                pointerStop = otherPoint2;
                                dormFirst = false;
                                if (dormPoint2 < otherPoint2)
                                {
                                    pointerStop = dormPoint2;
                                    dormFirst = true;
                                }
                            }
                            break;

                        case 3:
                            if (secondFirst)
                            {
                                if (dormFirst)
                                {
                                    // otherDorm points used twice
                                    pointerStop = dormPoint2;
                                    flipForDorm = !flipForDorm;
                                }
                                else
                                {
                                    // dorm points used twice
                                    pointerStop = otherPoint2;
                                    flipForOther = !flipForOther;
                                }
                            }
                            else
                            {
                                if (dormFirst)
                                {
                                    pointerStop = otherPoint2;
                                    flipForDorm = !flipForDorm;
                                }
                                else
                                {
                                    pointerStop = dormPoint2;
                                    flipForOther = !flipForOther;
                                }
                            }
                            break;

                        case 4:
                            if (secondFirst)
                            {
                                if (dormFirst)
                                    flipForDorm = !flipForDorm; 
                                else
                                    flipForOther = !flipForOther;
                            }
                            else
                            {
                                if (dormFirst)
                                    flipForOther = !flipForOther;
                                else
                                    flipForDorm = !flipForDorm;
                            }
                            pointerStop = max;
                            break;

                    }

                    ++pointsUsed;
                }

                // Doesn't work for scheduling
                /*if (orderedCrossoverChance == 1 || GEN.NextDouble() < orderedCrossoverChance)
                {
                    // keep Dorm and OtherDorm together
                    var midsectionLength = dormPoint2 - dormPoint1;
                    Array.Copy(Genes, dormPoint1, genes1, dormPoint1, midsectionLength);
                    Array.Copy(other.Genes, dormPoint1, genes1, 0, dormPoint1);
                    Array.Copy(other.Genes, 0, genes1, dormPoint2, (numGenes - 1) - dormPoint2);

                    Array.Copy(other.Genes, dormPoint1, genes2, dormPoint1, midsectionLength);
                    Array.Copy(Genes, dormPoint1, genes2, 0, dormPoint1);
                    Array.Copy(Genes, 0, genes2, dormPoint2, (numGenes - 1) - dormPoint2);

                    orderedCrossoverChance *= OXReduction;
                }
                else */

                if (GEN.NextDouble() > splitRate) // don't split
                {
                    if (!flipForDorm && !flipForOther)
                    {
                        max = Math.Max(otherPoint2, dormPoint2);

                        //Array.Copy(Genes, genes1, pointerStop);
                        //Array.Copy(other.Genes, genes2, pointerStop);

                        // skips the enumerator cycling after the last section
                        int num = numGenes - max;
                        Array.Copy(Genes, max, genes1, max, num);
                        Array.Copy(other.Genes, max, genes2, max, num);

                        //++pointsUsed;
                    }

                    do
                    {
                        GetNextStop();
                        if (pointerStop != genePointer)
                        {
                            if (!flipForDorm)
                            {
                                if (!flipForOther)
                                    ToNextPoint1122();
                                else
                                    ToNextPoint1221();
                            }
                            else
                            {
                                if (!flipForOther)
                                    ToNextPoint2112();
                                else
                                    ToNextPoint2211();
                            }
                        }
                    } while (pointerStop != max);
                }
                else
                {
                    do
                    {
                        GetNextStop();
                        if (pointerStop != genePointer)
                        {
                            if (!flipForDorm)
                            {
                                if (!flipForOther)
                                    ToNextPoint1212();
                                else
                                    ToNextPoint1221();
                            }
                            else
                            {
                                if (!flipForOther)
                                    ToNextPoint2112();
                                else
                                    ToNextPoint2121();
                            }
                        }
                    } while (pointerStop != max);
                    thisMutationRate -= splitRate;
                }

                Debug.Assert(thisMutationRate > SimpleGenePool.MinMutationChance);
                offspring[offspringIndex] = new SimpleChromosome(genes1, thisMutationRate, preservationRate, parents);
                ++offspringIndex;
                offspring[offspringIndex] = new SimpleChromosome(genes2, thisMutationRate, preservationRate, parents);
                ++offspringIndex;

                thisEnum.Reset();
                otherEnum.Reset();
            }

            return offspring;
        }

        #region Mutation
        public ushort Mutate()
        {
            if (GEN.NextDouble() < PreservationRate)
            {
                if (GEN.NextDouble() < 0.5)
                    Reordering(); // frameshift mutation
                else
                    Shuffling();
                return 0;
            }
            else
            {
                //preservation = (PreservationRate * 2) / (3 + mutationRatio);
                ushort numMutated = 0;
                if (!SwapMutation(out numMutated))
                {
                    for (ushort genePointer = 0; genePointer < SimpleGenePool.NUM_GENES; ++genePointer)
                    {
                        if (GEN.NextDouble() < MutationRate)
                        {
                            Genes[genePointer].Mutate();
                            ++numMutated;
                        }
                    }
                }
                return numMutated;
                //Debug.Assert(!Genes.Skip(1).Any(g => g.ID == 0));
            }
        }

        internal static byte SECONDARY_SHIFT_BLOCK_SIZE;
        #region Frameshift Mutations
        private void Reordering()
        {
            var mutationRatio = MutationRate / SimpleGenePool.MinMutationChance;
            var preservationRatio = 1 / PreservationRate;
            float preservation = (preservationRatio + mutationRatio) / (preservationRatio * mutationRatio * 2);

            if (GEN.NextDouble() < preservation)
            {
                // shift days around
                var shiftCount = (byte)(DormSurvivalInfo.GEN.GetNext(3) + 1);
                ReorderDays(shiftCount);
                return;
            }

            if (GEN.NextDouble() < preservation)
            {
                // shift around by blocks of 2
                var shiftCount = (byte)(DormSurvivalInfo.GEN.GetNext(7) + 1);
                if (shiftCount % 2 == 0)
                    ReorderDays((byte)(shiftCount / 2));
                else
                    ReorderBlocks(shiftCount);
                return;
            }
            else
            {
                // shift around by singular blocks
                var shiftCount = (byte)(DormSurvivalInfo.GEN.GetNext(15) + 1);

                if (shiftCount % 2 == 0)
                {
                    if (shiftCount % 4 == 0)
                        ReorderDays((byte)(shiftCount / 4));
                    else
                        ReorderBlocks((byte)(shiftCount / 2));
                    return;
                }

                SimpleGene[] output = new SimpleGene[SimpleGenePool.NUM_GENES];
                ushort genesShift = (ushort)((SECONDARY_SHIFT_BLOCK_SIZE * (shiftCount / 2)) + SimpleGenePool.NUM_ACTIVITIES);
                ushort geneIndex = 0;
                ushort destIndex = genesShift;

                byte maxBlock = MAX_DAY_INDEX * 4;
                bool oddBlock = false;
                for (byte block = 0; block < maxBlock; ++block)
                {
                    for (byte i = 0; i < SimpleGenePool.NUM_ODD_ACTIVITIES; ++i)
                    {
                        output[destIndex] = Genes[geneIndex].ChangeID(destIndex);
                        ++geneIndex;
                        ++destIndex;
                    }

                    // the oddness of the dstBlock is inverted
                    if (oddBlock)
                        destIndex += ActivitySurvivalInfo.NUM_LONGER;
                    else
                    {
                        for (byte i = 0; i < ActivitySurvivalInfo.NUM_LONGER; ++i)
                        {
                            output[geneIndex] = Genes[geneIndex];
                            ++geneIndex;
                        }

                        if (destIndex == SimpleGenePool.NUM_GENES)
                            destIndex = 0;
                    }
                    oddBlock = !oddBlock;
                }

                Genes = output;
                LogBuffer.Add(
                    new LogUpdate(
                        "GeneticScheduling",
                        EntryType.DEBUG,
                        String.Format(
                            "Chromosome {0} shifted by {1} singular blocks",
                            ChromosomeID,
                            shiftCount
                        )
                    )
                );
            }

            //Debug.Assert(!Genes.Skip(1).Any(g => g.ID == 0));
        }

        private void ReorderDays(byte shiftCount)
        {
            SimpleGene[] output = new SimpleGene[SimpleGenePool.NUM_GENES];

            var genesShift = (ushort)(shiftCount * SimpleGenePool.GENES_PER_DAY);
            ushort geneIndex = 0;
            ushort destIndex = genesShift;

            for (byte day = 0; day < MAX_DAY_INDEX; ++day)
            {
                for (ushort i = 0; i < SimpleGenePool.GENES_PER_DAY; ++i)
                {
                    output[destIndex] = Genes[geneIndex].ChangeID(destIndex);
                    ++geneIndex;
                    ++destIndex;
                }
                if (destIndex == SimpleGenePool.NUM_GENES)
                    destIndex = 0;
            }

            Genes = output;
            LogBuffer.Add(
                new LogUpdate(
                    "GeneticScheduling",
                    EntryType.DEBUG,
                    String.Format(
                        "Chromosome {0} shifted by {1} days",
                        ChromosomeID,
                        shiftCount
                    )
                )
            );
        }

        private void ReorderBlocks(byte shiftCount)
        {
            SimpleGene[] output = new SimpleGene[SimpleGenePool.NUM_GENES];

            var genesShift = (ushort)(shiftCount * SECONDARY_SHIFT_BLOCK_SIZE);
            ushort geneIndex = 0;
            ushort destIndex = genesShift;

            for (byte block = 0; block < MAX_DAY_INDEX * 2; ++block)
            {
                for (ushort i = 0; i < SECONDARY_SHIFT_BLOCK_SIZE; ++i)
                {
                    output[destIndex] = Genes[geneIndex].ChangeID(destIndex);
                    ++geneIndex;
                    ++destIndex;
                }
                if (destIndex == SimpleGenePool.NUM_GENES)
                    destIndex = 0;
            }

            Genes = output;
            LogBuffer.Add(
                new LogUpdate(
                    "GeneticScheduling",
                    EntryType.DEBUG,
                    String.Format(
                        "Chromosome {0} shifted by {1} blocks",
                        ChromosomeID,
                        shiftCount
                    )
                )
            );
        }
        #endregion

        private void Shuffling()
        {
            var mutationRatio = MutationRate / SimpleGenePool.MinMutationChance;
            var preservationRatio = 1 / PreservationRate;
            float preservation = (preservationRatio + mutationRatio) / (preservationRatio * mutationRatio * 2);

            SimpleGene[] output = new SimpleGene[SimpleGenePool.NUM_GENES];
            if (GEN.NextDouble() < preservation)
            {
                // shuffle days around
                byte[] destDays = new byte[4];
                SortedSet<byte> takenDestDays = new SortedSet<byte>();
                ushort geneIndex = 0;

                for (byte day = 0; day < MAX_DAY_INDEX; ++day)
                {
                    byte destDay = day;
                    do
                    {
                        destDay = DormSurvivalInfo.GEN.GetNext(4);
                    } while (takenDestDays.Contains(destDay));
                    takenDestDays.Add(destDay);
                    destDays[day] = destDay;

                    if (destDay == day)
                    {
                        Array.Copy(Genes, geneIndex, output, geneIndex, SimpleGenePool.GENES_PER_DAY);
                        geneIndex += SimpleGenePool.GENES_PER_DAY;
                    }
                    else
                    {
                        ushort destIndex = (ushort)(destDay * SimpleGenePool.GENES_PER_DAY);

                        for (ushort i = 0; i < SimpleGenePool.GENES_PER_DAY; ++i)
                        {
                            output[destIndex] = Genes[geneIndex].ChangeID(destIndex);
                            ++geneIndex;
                            ++destIndex;
                        }
                    }
                }

                Genes = output;
                LogBuffer.Add(
                    new LogUpdate(
                        "GeneticScheduling", 
                        EntryType.DEBUG, 
                        String.Format(
                            "Chromosome {0} shuffled by days", 
                            ChromosomeID
                        ),
                        destDays.ToArrayString()
                    )
                );
                return;
            }

            if (GEN.NextDouble() < preservation)
            {
                // shuffle around by blocks of 2
                ushort geneIndex = 0;
                SortedSet<byte> takenBlocks = new SortedSet<byte>();

                for (byte block = 0; block < MAX_DAY_INDEX * 2; ++block)
                {
                    byte destBlock = block;
                    do
                    {
                        destBlock = DormSurvivalInfo.GEN.GetNext(8);
                    } while (takenBlocks.Contains(destBlock));
                    takenBlocks.Add(destBlock);

                    if (destBlock == block)
                    {
                        Array.Copy(Genes, geneIndex, output, geneIndex, SECONDARY_SHIFT_BLOCK_SIZE);
                        geneIndex += SECONDARY_SHIFT_BLOCK_SIZE;
                    }
                    else
                    {
                        ushort destIndex = (ushort)(SECONDARY_SHIFT_BLOCK_SIZE * destBlock);

                        for (ushort i = 0; i < SECONDARY_SHIFT_BLOCK_SIZE; ++i)
                        {
                            output[destIndex] = Genes[geneIndex].ChangeID(destIndex);
                            ++geneIndex;
                            ++destIndex;
                        }
                    }
                }

                Genes = output;
                LogBuffer.Add(
                    new LogUpdate(
                        "GeneticScheduling",
                        EntryType.DEBUG,
                        String.Format(
                            "Chromosome {0} shuffled by blocks",
                            ChromosomeID
                        )
                    )
                );
                return;
            }
            else
            {
                // shuffle around by singular blocks
                ushort geneIndex = 0;
                SortedSet<byte> takenBlocks = new SortedSet<byte>();

                byte maxBlock = MAX_DAY_INDEX * 4;
                bool oddBlock = false;

                for (byte block = 0; block < maxBlock; ++block)
                {
                    byte destBlock = block;
                    do
                    {
                        destBlock = DormSurvivalInfo.GEN.GetNext(maxBlock);
                    } while (takenBlocks.Contains(destBlock));
                    takenBlocks.Add(destBlock);

                    if (destBlock == block)
                    {
                        byte blocksize = oddBlock ? SimpleGenePool.NUM_ODD_ACTIVITIES : SimpleGenePool.NUM_ACTIVITIES;
                        Array.Copy(Genes, geneIndex, output, geneIndex, blocksize);
                        geneIndex += blocksize;
                    }
                    else
                    {
                        byte diff = (byte)(
                            block > destBlock ?
                                block - destBlock :
                                destBlock - block
                        );
                        bool oddShift = diff % 2 != 0;
                        ushort destIndex = (ushort)((destBlock / 2) * SECONDARY_SHIFT_BLOCK_SIZE);
                        if (destBlock % 2 != 0) // missing the first half of a bigger block
                            destIndex += SimpleGenePool.NUM_ACTIVITIES;

                        if (oddShift)
                        {
                            for (byte i = 0; i < SimpleGenePool.NUM_ODD_ACTIVITIES; ++i)
                            {
                                output[destIndex] = Genes[geneIndex].ChangeID(destIndex);
                                ++geneIndex;
                                ++destIndex;
                            }

                            if (oddBlock)
                            {
                                // destBlock has longer duration activities
                                for (byte i = 0; i < ActivitySurvivalInfo.NUM_LONGER; ++i)
                                {
                                    output[destIndex] = Genes[destIndex];
                                    ++destIndex;
                                }
                            }
                            else
                            {
                                // srcBlock has longer duration activities
                                for (byte i = 0; i < ActivitySurvivalInfo.NUM_LONGER; ++i)
                                {
                                    output[geneIndex] = Genes[geneIndex];
                                    ++geneIndex;
                                }
                            }
                            // unlike the reordering, this check is done for both src and dest
                            // oddShift can differ from block to block, so none, one, or both can be overwritten
                        }
                        else
                        {
                            byte max = oddBlock ? SimpleGenePool.NUM_ODD_ACTIVITIES : SimpleGenePool.NUM_ACTIVITIES;
                            for (byte i = 0; i < max; ++i)
                            {
                                output[destIndex] = Genes[geneIndex].ChangeID(destIndex);
                                ++geneIndex;
                                ++destIndex;
                            }
                        }
                    }

                    oddBlock = !oddBlock;
                }

                Genes = output;
                LogBuffer.Add(
                    new LogUpdate(
                        "GeneticScheduling",
                        EntryType.DEBUG,
                        String.Format(
                            "Chromosome {0} shuffled by singular blocks",
                            ChromosomeID
                        )
                    )
                );
            }

            //Debug.Assert(!Genes.Skip(1).Any(g => g.ID == 0));
        }

        private void NoiseMutation()
        {

        }

        private bool SwapMutation(out ushort numMutated)
        {
            numMutated = 0;
            if (GEN.NextDouble() < SimpleGenePool.FieldMutationRates[3])
            {
                var dorm = SimpleGenePool.GetGeneParamRandomValue(3);
                while (dorm == 255)
                    dorm = SimpleGenePool.GetGeneParamRandomValue(3);

                for (ushort g = 0; g < SimpleGenePool.NUM_GENES; ++g)
                {
                    if (Genes[g].Dorm == dorm)
                    {
                        Genes[g].MutatePrimaryExcl();
                        ++numMutated;
                    }
                    else if (Genes[g].OtherDorm == dorm)
                    {
                        Genes[g].MutateOtherExcl();
                        ++numMutated;
                    }
                }
                return true;
            }
            return false;
            
        }
        #endregion
        #endregion

        public void FlushLogBuffer()
        {
            if (!_logBufferFlushed)
            {
                LogBuffer[0].LogContent[1] = String.Format(
                    "Stats: {0}, {1}, and {2}",
                    Stats.Fitness.ToString("#.000"),
                    Stats.Fertility.ToString("#.000"),
                    Stats.MinGeneFitness.ToString("#.000")
                );
                foreach (var update in LogBuffer)
                {
                    LogManager.Enqueue(update);
                }
                LogBuffer.Clear();
                _logBufferFlushed = true;
            }
        }

        public void SerializeTo(string folder, string filename)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            using (FileStream file = new FileStream(filename + ".csm", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                //file.Write(BitConverter.GetBytes(Genes.Length), 0, 4);
                //file.WriteByte((byte)geneParams.Length);
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

        public int CompareTo(SimpleChromosome other) =>
            ChromosomeID == other.ChromosomeID ? 0 :
                (ChromosomeID > other.ChromosomeID ? 1 : -1);

        private static (byte, byte) GetPoints()
        {
            byte point1 = 255;
            byte point2 = 255;

            do
            {
                point1 = CROSSOVER_GEN.GetNext();
                point2 = CROSSOVER_GEN.GetNext();
            } while (point1 == point2);

            if (point2 < point1)
            {
                var temp = point2;
                point2 = point1;
                point1 = temp;
            }

            return (point1, point2);
        }

        private struct HistoryEvent : IComparable<HistoryEvent>
        {
            public byte Activity;
            public byte Dorm;

            public HistoryEvent(byte activity, byte dorm)
            {
                Activity = activity;
                Dorm = dorm;
            }

            public int CompareTo(HistoryEvent other)
            {
                if (Activity != other.Activity)
                    return Activity - other.Activity;
                else
                    return Dorm - other.Dorm;
            }
        }

        private struct DormPairing : IComparable<DormPairing>
        {
            public byte OtherDorm;
            public byte Dorm;

            public DormPairing(byte dorm, byte otherDorm)
            {
                OtherDorm = otherDorm;
                Dorm = dorm;
            }

            public int CompareTo(DormPairing other)
            {
                if (Dorm != other.Dorm)
                    return Dorm - other.Dorm;
                else
                    return OtherDorm - other.OtherDorm;
            }
        }
    }
}
