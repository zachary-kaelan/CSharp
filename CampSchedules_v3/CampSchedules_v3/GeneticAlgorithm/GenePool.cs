using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CampSchedulesLib_v2.Models.Info;
using CampSchedulesLib_v2.Models.CSV;
using SimpleImmutable = CampSchedules_v3.SimpleGeneticAlgorithm.SimpleImmutable;
using Jil;
using ZachLib;
using ZachLib.Logging;

namespace CampSchedules_v3.GeneticAlgorithm
{
    public static class GenePool
    {
        //private static readonly Random GEN = new Random(324793728);

        public static bool Frozen { get; private set; }
        private static SortedDictionary<string, byte> GeneParameterIndices { get; set; }
        private static List<GeneParameter> GeneParameters { get; set; }
        public static GeneParameter[] GeneParams { get; private set; }
        private static byte GeneLength { get; set; }

        private static float MutationChance { get; set; }
        private static float PoolMinFitness { get; set; }
        private static int PoolNumFit { get; set; }
        private static float[] PoolLastStats { get; set; }

        private static readonly object EvoLock = new object();

        internal static List<byte[]> ParamsPossibleValues { get; private set; }
        private static ByteGenerator[] ParamsValueGenerators;

        private static string Path { get; set; }

        private const int POOL_SIZE = 16;
        private const int SAMPLE_POOL_SIZE = POOL_SIZE * POOL_SIZE;
        internal const byte NUM_IMMUTABLE = 3;
        internal const byte NUM_PARAMS = 5;
        internal static byte NUM_GENES { get; private set; }

        private const string PATH = @"E:\Work Programming\Higher Ground Program Files\Genetics\";

        private static byte GeneParamIndex = 0;

        private static Chromosome[] Pool { get; set; }
        private static Chromosome[] SamplePool { get; set; }
        internal static Gene[] ImmutableGenes { get; private set; }
        internal static SimpleImmutable[] ImmutableValues { get; private set; }

        static GenePool()
        {
            LogManager.AddLog("GeneticScheduling", LogType.FolderFilesByDate);

            GeneParameterIndices = new SortedDictionary<string, byte>();
            ParamsPossibleValues = new List<byte[]>();
            GeneParameters = new List<GeneParameter>();
            Frozen = false;
        }

        #region Initialization
        internal static byte GetGeneParamRandomValue(byte param) =>
            ParamsPossibleValues[param][ParamsValueGenerators[param].GetNext()];
            //ParamsPossibleValues[param][GeneParameters[param].GetRandomValue()];

        public static bool AddParameterType(string name, bool mutable, params byte[] ids)
        {
            if (Frozen || GeneParameterIndices.ContainsKey(name))
                return false;
            var gene = new GeneParameter(GeneParamIndex, mutable, ids);
            GeneParameters.Add(gene);
            GeneParameterIndices.Add(name, gene.Index);
            ParamsPossibleValues.Add(ids);
            ++GeneParamIndex;
            return true;
        }

        // switches to read-only
        public static byte FreezeGenePool()
        {
            Frozen = true;
            GeneLength = (byte)GeneParameters.Sum(gp => gp.Bits);
            MutationChance = 1f / GeneLength;

            GeneParams = GeneParameters.ToArray();
            GeneParameters = null;
            ParamsValueGenerators = new ByteGenerator[NUM_PARAMS - NUM_IMMUTABLE];
            for (int p = NUM_IMMUTABLE; p < NUM_PARAMS; ++p)
            {
                ParamsValueGenerators[p] = new ByteGenerator(GeneParams[p].MaxValue);
            }

            return GeneLength;
        }

        // gene pool is a list of every possible event block, most of which will be inactive
        public static int CreatePool(string infoFilesFolder)
        {
            Path = infoFilesFolder;
            AddParameterType("Day", false, 1, 2, 3, 4, 5);
            AddParameterType("Time", false, 0, 1, 2, 3, 4, 5, 6);

            string path = infoFilesFolder.TrimEnd('\\') + "\\";
            var activitiesCSV = Utils.LoadCSV<ActivityCSV>(path + "Activities.csv");
            var activities = activitiesCSV.Select(a => a.ToInfo()).ToList();
            var exclusives = activities.Select((a, i) => new { Index = i, Activity = a }).Where(a => a.Activity.Flags.HasFlag(ActivityFlags.Manual));
            var dorms = File.ReadAllLines(
                    path + "ExclusiveActivities.txt"
                ).Select(
                    l =>
                    {
                        var split = l.Split(
                            new string[] { " - ", ", " },
                            StringSplitOptions.None
                        );
                        return new DormInfo(
                            split[0],
                            split.Skip(1).Select(
                                a => Convert.ToInt32(a)
                            ).ToArray()
                        );
                    }
                ).ToArray();
            var manuallyScheduled = Utils.LoadCSV<ManuallyScheduledCSV>(path + "Manually Scheduled.csv").OrderBy(a => a.Day).ThenBy(a => a.Start).ThenBy(a => a.Dorm).ToList();
            var blocks = Utils.LoadCSV<BlockCSV>(path + "Days.csv");

            AddParameterType("Activity", false, Enumerable.Range(0, activities.Count).Select(a => (byte)a).ToArray());
            var dormBytes = Enumerable.Range(0, dorms.Count()).Select(d => (byte)d).Append<byte>(255).ToArray();
            AddParameterType("Dorm", true, dormBytes);
            AddParameterType("OtherDorm", true, dormBytes.Skip(1).ToArray());
            dormBytes = null;

            var immutableList = new List<Gene>();

            int blockIndex = 0;
            byte time = 0;
            ushort geneID = ushort.MaxValue;
            foreach(var manual in manuallyScheduled)
            {
                while (blocks[blockIndex].Day < manual.Day)
                {
                    ++blockIndex;
                    time = 0;
                }
                while (blocks[blockIndex].Hour < manual.Start.Hours || blocks[blockIndex].Minute < manual.Start.Minutes)
                {
                    ++blockIndex;
                    ++time;
                }

                int blockIndexTemp = blockIndex;
                byte timeTemp = time;
                do
                {
                    immutableList.Add(
                        new Gene(
                            geneID,
                            new byte[]
                            {
                                (byte)manual.Day,
                                time,
                                (byte)exclusives.First(a => a.Activity.Abbreviation == manual.Activity).Index,
                                (byte)Array.FindIndex(dorms, d => d.Abbreviation == manual.Dorm),
                                255
                            },
                            true
                        )
                    );
                    --geneID;
                    ++blockIndexTemp;
                    ++timeTemp;
                } while (
                    blockIndexTemp < blocks.Length &&
                    blocks[blockIndexTemp].Day == manual.Day && 
                    blocks[blockIndexTemp].Hour < manual.End.Hours && 
                    blocks[blockIndexTemp].Minute < manual.End.Minutes
                );
            }

            ImmutableGenes = immutableList.ToArray();
            immutableList = null;

            //List<Gene> genePool = new List<Gene>();
            //List<Tuple<byte, byte, byte>> genePool = new List<Tuple<byte, byte, byte>>();
            List<SimpleImmutable> genePool = new List<SimpleImmutable>();
            geneID = 0;
            var activitiesByDuration = activities.FindAll(a => !a.Flags.HasFlag(ActivityFlags.Manual) && !a.Flags.HasFlag(ActivityFlags.Concurrent)).GroupBy(a => a.Duration);
            byte concurrentActivity = (byte)activities.First(a => a.Flags.HasFlag(ActivityFlags.Concurrent)).ID;

            SetupConsts(dorms, activities, path);
            List<byte> times = new List<byte>();
            List<byte> tailTimes = new List<byte>();
            //Constants.SURVIVAL_DAYINFO = new DaySurvivalInfo[((byte)blocks.Last().Day) + 1];

            int pointer = 0;
            byte currentDay = 0;
            byte dayGenesCount = 0;
            byte[] timesGenesCounts = new byte[7];
            byte currentTime = 0; // time in the day
            while (pointer < blocks.Length)
            {
                List<BlockCSV> currentBlocks = new List<BlockCSV>();
                BlockCSV nextBlock = blocks[pointer];
                BlockCSV lastBlock = new BlockCSV();

                byte total = 0;      // total number of blocks in the set
                byte available = 0;  // number of blocks in the set available to start activities
                byte excess = 0;

                // time at the start of this blockset
                byte timeTemp = currentTime;

                do
                {
                    currentBlocks.Add(nextBlock);
                    lastBlock = nextBlock;
                    ++total;
                    if (lastBlock.Excess)
                        ++excess;
                    else
                        ++available;
                    ++pointer;
                    ++currentTime;
                    if (pointer != blocks.Length)
                        nextBlock = blocks[pointer];
                } while (
                    pointer < blocks.Length &&
                    nextBlock.Day == lastBlock.Day &&
                    nextBlock.Hour - lastBlock.Hour <= 1
                );
                tailTimes.Add((byte)(timeTemp + available - 1));

                foreach (var duration in activitiesByDuration)
                {
                    if (duration.Key > currentBlocks.Count)
                        continue;
                    for (byte actualTime = timeTemp; actualTime < available + timeTemp; actualTime = (byte)(actualTime + duration.Key))
                    {
                        //byte actualTime = (byte)(currentTime + timeOffset);
                        foreach (var activity in duration)
                        {
                            /*genePool.Add(
                                new Gene(
                                    geneID,
                                    new byte[]
                                    {
                                        currentDay,
                                        actualTime,
                                        (byte)activity.ID,
                                        255,
                                        255
                                    }
                                )
                            );*/
                            genePool.Add(new SimpleImmutable(currentDay, actualTime, (byte)activity.ID));
                            //++geneID;
                            ++dayGenesCount;
                            ++timesGenesCounts[actualTime];
                        }
                    }                    
                }

                for (byte actualTime = timeTemp; actualTime < available; ++actualTime)
                {
                    /*genePool.Add(
                        new Gene(
                            geneID,
                            new byte[]
                            {
                                currentDay,
                                actualTime,
                                concurrentActivity,
                                255,
                                255
                            }
                        )
                    );
                    ++geneID;*/
                    genePool.Add(new SimpleImmutable(currentDay, actualTime, concurrentActivity));
                    ++dayGenesCount;
                    ++timesGenesCounts[actualTime];
                }

                if (nextBlock.Day != lastBlock.Day)
                {
                    /*Constants.SURVIVAL_DAYINFO[currentDay] = new DaySurvivalInfo(
                        timesGenesCounts, 
                        tailTimes, 
                        dayGenesCount
                    );*/
                    timesGenesCounts = new byte[7];
                    dayGenesCount = 0;
                    ++currentDay;
                    currentTime = 0;
                }
            }

            int numGenes = genePool.Count;
            NUM_GENES = (byte)numGenes;
            Chromosome.CROSSOVER_GEN = new ByteGenerator((byte)numGenes);
            Pool = new Chromosome[POOL_SIZE];
            SamplePool = new Chromosome[SAMPLE_POOL_SIZE];
            Func<GeneParameter[]> getGeneParams = () => GeneParams;

            ImmutableValues = genePool.ToArray();
            /*
            for (int i = 0; i < numGenes; ++i)
            {
                var gene = genePool[i];
                ImmutableValues[i, 0] = gene.Item1;
                ImmutableValues[i, 1] = gene.Item2;
                ImmutableValues[i, 2] = gene.Item3;
            }*/
            genePool = null;

            for (int c = 0; c < POOL_SIZE; ++c)
            {
                Gene[] chromosome = new Gene[numGenes];
                for (int g = 0; g < numGenes; ++g)
                {
                    //chromosome[g] = genePool[g].Randomize(2);
                }
                Pool[c] = new Chromosome(chromosome, getGeneParams);
            }

            PoolMinFitness = 0;
            return numGenes;
        }

        private static void SetupConsts(DormInfo[] dorms, List<ActivityInfo> activities, string path)
        {
            var dormActivityPriorities = Utils.LoadCSV<DormActivityPriorityCSV>(
                path + "Additional Dorm Priorities.csv"
            ).GroupBy(
                p => p.Dorm
            ).ToDictionary(
                g => g.Key,
                g => g.ToDictionary(
                    p => activities.FindIndex(
                        a => a.Abbreviation == p.ActivityAbbreviation
                    ), p => p.PriorityChange + 1
                )
            );

            int numDorms = dorms.Count();
            int boysMaxAgeGroup = dorms.Last(d => !d.IsGirl).AgeGroup;
            int girlsMaxAgeGroup = dorms.Last(d => d.IsGirl).AgeGroup;
            var dormAgeIndices = new int[numDorms];
            for (int i = 0; i < numDorms; ++i)
            {
                dormAgeIndices[dorms[i].SetAgeIndex(boysMaxAgeGroup, girlsMaxAgeGroup)] = i;
            }

            Constants.SURVIVAL_DORMINFO = new DormSurvivalInfo[dorms.Length];
            Constants.FITNESS_DORMINFO = new DormFitnessInfo[dorms.Length];

            var dormsByAge = dormAgeIndices.Select(d => dorms[d]).ToList();
            var olderDorms = dorms.SkipWhile(d => d.AgeGroup < 5);
            int numGirls = dorms.Count(d => d.IsGirl);
            var genderRatio = ((double)numGirls) / (numDorms - numGirls);
            var genderDiffRatio = ((genderRatio >= 1 ? genderRatio : (1.0 / genderRatio)) - 1);
            for (int i = 0; i < numDorms; ++i)
            {
                var dorm = dormsByAge[i];

                // Fitness
                DormFitnessInfo fitness = new DormFitnessInfo();

                IEnumerable<DormInfo> otherDorms = null;
                var girlEquivalent = (int)(dorm.AgeGroup * genderRatio);
                if (!dorm.IsGirl && girlEquivalent > dorm.AgeGroup)
                {
                    int diffCutOff = (girlEquivalent - dorm.AgeGroup) + 1;
                    otherDorms = dorms.Where(
                        d => (d.AgeGroup - dorm.AgeGroup) <= 1 ||
                             (d.IsGirl && (d.AgeGroup - dorm.AgeGroup) <= diffCutOff)
                    );
                }
                else
                    otherDorms = dorms.TakeWhile(d => (d.AgeGroup - dorm.AgeGroup) <= 1);

                foreach (var other in otherDorms)
                {
                    fitness.DormPriorities.Add(
                        (byte)other.ID,
                        (byte)(dorm.IsGirl == other.IsGirl ?
                            3 - (other.AgeGroup - dorm.AgeGroup) :
                            4 - (2 * (other.AgeGroup - dorm.AgeGroup)))
                    );
                }

                if (dorm.AgeGroup == 1)
                {
                    foreach (var olderDorm in olderDorms)
                    {
                        fitness.DormPriorities.Add(
                            (byte)olderDorm.ID, 
                            1
                        );
                    }
                }

                var activityPriorities = dormActivityPriorities.TryGetValue(
                    dorm.Abbreviation, 
                    out Dictionary<int, int> priorities
                ) ? priorities : new Dictionary<int, int>();
                foreach(var priority in activityPriorities)
                {
                    fitness.ActivityPriorities.Add((byte)priority.Key, (byte)priority.Value);
                }

                Constants.FITNESS_DORMINFO[i] = fitness;

                // Survival
                DormSurvivalInfo survival = new DormSurvivalInfo(
                    dorm.AllowedExclusiveActivities.Select(a => (byte)a).ToArray(),
                    fitness.DormPriorities.Keys.ToArray()
                );
            }

            Constants.SURVIVAL_ACTIVITYINFO = activities.Select(
                info => new ActivitySurvivalInfo(info)
            ).ToArray();
            Constants.FITNESS_ACTIVITYINFO = activities.Select(a => (byte)a.Priority).ToArray();
        }
        #endregion

        public static void Evolve(string infoFilesFolder)
        {
            CreatePool(infoFilesFolder);

            EvolutionStatus evoStatus = new EvolutionStatus();
            Thread evolutionThread = new Thread(
                new ParameterizedThreadStart(
                    obj =>
                    {
                        var status = (EvolutionStatus)evoStatus;
                        while(true)
                        {
                            Crossover();
                            Mutate();
                            Selection();

                            lock(EvoLock)
                            {
                                ++status.Generation;
                                status.Stats = PoolLastStats;
                                status.MinFitness = PoolMinFitness;
                            }

                            if (status._doPause)
                            {
                                try
                                {
                                    status._doPause = false;
                                    status._isPaused = true;
                                    Thread.Sleep(Timeout.Infinite);
                                }
                                catch (ThreadInterruptedException)
                                {
                                    
                                }
                                catch (ThreadAbortException)
                                {
                                    break;
                                }
                                finally
                                {
                                    status._isPaused = false;
                                }
                            }
                        }
                    }
                )
            );

            evolutionThread.Priority = ThreadPriority.AboveNormal;
            evolutionThread.IsBackground = false;

            evolutionThread.Start(evoStatus);
            Console.WriteLine("Starting evolution at {0}.", DateTime.Now.TimeOfDay);
            while (true)
            {
                for (int i = 0; i < 4; ++i)
                {
                    for (int j = 0; j < 3; ++j)
                    {
                        Thread.Sleep(5000);
                        lock (EvoLock)
                        {
                            Console.WriteLine("Generation {0} - {1}", evoStatus.Generation, evoStatus.MinFitness);
                        }
                    }
                    lock(EvoLock)
                    {
                        Console.WriteLine("Generation {0} - {1}", evoStatus.Generation, evoStatus.Stats.ToArrayString());
                    }
                }

                evoStatus._doPause = true;
                SpinWait.SpinUntil(() => evoStatus._isPaused);

                int cIndex = 0;
                lock(EvoLock)
                {
                    evoStatus.LastEvolutionStop = DateTime.Now;
                    evoStatus.SaveAs(PATH + "EvoStatus.json");
                    foreach (var chromosome in Pool)
                    {
                        ++cIndex;
                        chromosome.SerializeTo(PATH + "GenePool", "Chromosome" + cIndex.ToString());
                    }
                    Console.WriteLine("CHROMOSOMES SERIALIZED");
                }

                evolutionThread.Interrupt();
            }
        }

        private static void Selection()
        {
            // stats of each chromosome of the pool
            SortedSet<KeyValuePair<ushort, EvoStats>> evoStats = new SortedSet<KeyValuePair<ushort, EvoStats>>(
                SamplePool.Select((c, i) => new KeyValuePair<ushort, EvoStats>((ushort)i, c.Select())),
                new EvoStatsComparer()
            );
            //SortedList<EvoStats, int> evoStats = new SortedList<EvoStats, int>(SAMPLE_POOL_SIZE, new EvoStatsComparer());

            // inferior chromosomes aren't replaced so much as they are shifted down
            int currentPoolSize = POOL_SIZE;
            List<Chromosome> oldPool = new List<Chromosome>(Pool);
            int statsPointer = 0;
            int startingIndex = -1;
            int currentNumFit = PoolNumFit;

            var candidates = evoStats.Take(POOL_SIZE);
            var first = candidates.First();
            IEnumerator<KeyValuePair<ushort, EvoStats>> candidatesEnumerator = null;

            void ReplaceFit()
            {
                candidatesEnumerator.MoveNext();

                for (int i = startingIndex; i < currentNumFit; ++i)
                {
                    var current = candidatesEnumerator.Current.Value;
                    if (current.Fitness >= PoolLastStats[statsPointer])
                    {
                        oldPool.Insert(i, SamplePool[candidatesEnumerator.Current.Key]);
                        ++currentNumFit;
                        ++currentPoolSize;
                        candidatesEnumerator.MoveNext();
                    }
                    else
                    {
                        ++statsPointer;
                        if (statsPointer == PoolNumFit)
                            break;
                    }
                }
            }

            void ReplaceUnfit()
            {
                candidatesEnumerator.MoveNext();

                for (int i = currentNumFit; i < currentPoolSize; ++i)
                {
                    var current = candidatesEnumerator.Current.Value;
                    if (current.Survives || current.Fertility >= PoolLastStats[statsPointer])
                    {
                        oldPool.Insert(i, SamplePool[candidatesEnumerator.Current.Key]);
                        ++currentPoolSize;
                        candidatesEnumerator.MoveNext();
                    }
                    else
                    {
                        // essentially searching for the next startingIndex for the remaining candidates
                        ++statsPointer;
                        if (statsPointer == POOL_SIZE)
                            break;
                    }
                }
            }

            if (PoolMinFitness != 0)
            {
                if (first.Value.Fitness >= PoolMinFitness)
                {
                    // using greater-than-or-equals-to for diversity

                    startingIndex = Array.FindIndex(PoolLastStats, s => s <= first.Value.Fitness);
                    statsPointer = startingIndex;
                    if (startingIndex >= PoolNumFit)
                    {
                        // if the only replaceable chromosomes are unfit ones

                        candidates = candidates.TakeWhile(
                            c => c.Value.Survives || 
                                 c.Value.Fertility >= PoolMinFitness
                        );
                        candidatesEnumerator = candidates.GetEnumerator();

                        ReplaceUnfit();
                    }
                    else if (PoolNumFit != POOL_SIZE)
                    {
                        candidates = candidates.TakeWhile(
                            c => c.Value.Survives ||
                                 c.Value.Fertility >= PoolMinFitness
                        );
                        candidatesEnumerator = candidates.GetEnumerator();

                        ReplaceFit();

                        ReplaceUnfit();
                    }
                    else
                    {
                        candidates = candidates.TakeWhile(c => c.Value.Fitness >= PoolMinFitness);
                        candidatesEnumerator = candidates.GetEnumerator();

                        ReplaceFit();
                    }
                }
            }

            candidatesEnumerator.Dispose();

            Pool = oldPool.Take(16).ToArray();
            PoolLastStats = Pool.Select(c => c.Stats.GetStat()).ToArray();
            PoolMinFitness = PoolLastStats[POOL_SIZE - 1];
            Array.Clear(SamplePool, 0, SAMPLE_POOL_SIZE);
        }

        private static void Crossover()
        {
            int numOffspring = 0;
            int numIterations = 0;
            while (numOffspring < SAMPLE_POOL_SIZE)
            {
                var offspring = Pool[ProdDist()].CrossoverWith(Pool[ProdDist()], POOL_SIZE);
                var maxOffspring = Math.Min(offspring.Length, SAMPLE_POOL_SIZE - numOffspring);
                for (int i = 0; i < maxOffspring; ++i)
                {
                    SamplePool[numOffspring] = offspring[i];
                    ++numOffspring;
                }
                ++numIterations;
            }

            //LogManager.Enqueue("GeneticScheduling", EntryType.DEBUG, "Sample pool filled in " + numIterations.ToString() + " crossover iterations");
        }

        private static void SerializeGenePoolParams()
        {
            GenePoolInfo info = new GenePoolInfo();
            foreach(var param in GeneParameterIndices)
            {
                info.GeneParameters.Add(param.Key, new KeyValuePair<GeneParameter, byte[]>(GeneParams[param.Value], ParamsPossibleValues[param.Value]));
            }
            info.MutationChance = MutationChance;
            info.NumGenes = NUM_GENES;
            info.NumImmutable = ImmutableGenes.Length;
            info.SaveAs(Path + "GenePoolInfo.json");

            using (FileStream file = new FileStream(Path + "ImmutableGenes.csm", FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                //file.WriteByte((byte)ImmutableGenes.Length);
                foreach(var immutable in ImmutableGenes)
                {
                    immutable.SerializeWith(file);
                }
            }

            using (FileStream file = new FileStream(Path + "ImmutableValues.csm", FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                foreach (var immutable in ImmutableValues)
                {
                    file.WriteByte(immutable.Day);
                    file.WriteByte(immutable.Time);
                    file.WriteByte(immutable.Activity);
                }
            }
        }

        private static void Mutate()
        {
            for (int i = 0; i < SAMPLE_POOL_SIZE; ++i)
            {
                SamplePool[i].Mutate(MutationChance);
            }
        }

        private static int ProdDist() =>
            (int)(Chromosome.GEN.NextDouble() * Chromosome.GEN.NextDouble() * POOL_SIZE);

        private class EvolutionStatus
        {
            [JilDirective(true)]
            public volatile bool _isPaused;
            [JilDirective(true)]
            public volatile bool _doPause;

            public int Generation { get; set; }
            public float MinFitness { get; set; }
            public float[] Stats { get; set; }
            public int PoolNumFit { get; set; }

            [JilDirective(false)]
            private DateTime LastEvolutionStart { get; set; }
            public DateTime LastEvolutionStop { get; set; }

            public EvolutionStatus()
            {
                LastEvolutionStart = DateTime.Now;
                LastEvolutionStop = DateTime.MinValue;
                Generation = 0;
                MinFitness = 0;
                PoolNumFit = 0;
                Stats = new float[POOL_SIZE];
            }


        }

        private class GenePoolInfo
        {
            public SortedDictionary<string, KeyValuePair<GeneParameter, byte[]>> GeneParameters { get; set; }
            public float MutationChance { get; set; }
            public int NumGenes { get; set; }
            public int NumImmutable { get; set; }

            public GenePoolInfo()
            {
                this.GeneParameters = new SortedDictionary<string, KeyValuePair<GeneParameter, byte[]>>();
            }
        }
    }
}
