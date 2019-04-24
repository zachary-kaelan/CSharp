using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CampSchedulesLib_v2.Models.Info;
using CampSchedulesLib_v2.Models.CSV;
using Jil;
using ZachLib;
using ZachLib.Logging;
using GeneParameter = CampSchedules_v3.GeneticAlgorithm.GeneParameter;

namespace CampSchedules_v3.SimpleGeneticAlgorithm
{
    public static class SimpleGenePool
    {

        #region Migration
        /*private SimpleChromosome[] Pool { get; set; }
        private SimpleChromosome[] SamplePool { get; set; }

        private readonly SortedDictionary<long, KeyValuePair<long, long>> ParentsHistory = new SortedDictionary<long, KeyValuePair<long, long>>();

        public SimpleGenePool()
        {

        }*/
        #endregion

        //private static readonly Random GEN = new Random(324793728);

        public static bool Frozen { get; private set; }
        private static SortedDictionary<string, byte> GeneParameterIndices { get; set; }
        private static List<GeneParameter> GeneParameters { get; set; }
        public static GeneParameter[] GeneParams { get; private set; }
        private static byte GeneLength { get; set; }

        internal static float MutationChance { get; private set; }
        internal static float MinMutationChance { get; private set; }
        internal static float MaxMutationChance { get; private set; }
        //private static float PoolMinFitness { get; set; }
        //private static float PoolMinFertility { get; set; }
        private static float PoolMaxFitness { get; set; }
        private static float PoolMaxFertility { get; set; }
        private static int PoolNumFit { get; set; }
        private static float[] PoolLastStats { get; set; }

        private static readonly object EvoLock = new object();

        internal static List<byte[]> ParamsPossibleValues { get; private set; }
        private static ByteGenerator[] ParamsValueGenerators;
        private static int Generation = 0;

        private static string Path { get; set; }
        private const ushort MAX_AGE = 4;
        private const ushort POOL_SIZE = 64;
        private const ushort POOL_MIDPOINT = POOL_SIZE / 2;
        private const ushort POOL_HALF_MIDPOINT = POOL_MIDPOINT / 2;
        private const ushort SAMPLE_POOL_SIZE = POOL_MIDPOINT * POOL_MIDPOINT;
        internal const byte NUM_IMMUTABLE = 3;
        internal const byte NUM_PARAMS = 5;
        internal static byte NUM_ACTIVITIES;
        internal static byte NUM_ODD_ACTIVITIES;
        internal static byte GENES_PER_DAY;
        internal static ushort NUM_GENES { get; private set; }
        internal static byte NUM_MUTABLE { get; private set; }

        private const ushort MAX_NONCANDIDATE_INDEX = SAMPLE_POOL_SIZE - POOL_SIZE;
        private const float SPONTANEOUS_DEATH_CHANCE = 0.075f;
        private const float OLD_AGE_DEATH_CHANGE = 0.25f;
        private const float GLOBAL_MUTATION_MULTIPLIER = 2f;
        private const float NEW_SPECIES_IMMIGRATION_CHANCE = 0.75f;

        internal static byte NUM_BLOCKS { get; private set; }
        internal static sbyte[] BASE_TIMES { get; private set; }

        private const string PATH = @"E:\Work Programming\Higher Ground Program Files\Genetics\";

        private static byte GeneParamIndex = 0;

        private static SimpleChromosome[] Pool { get; set; }
        private static SimpleChromosome[] SamplePool { get; set; }
        internal static SimpleGene[] ImmutableSimpleGenes { get; private set; }
        internal static SortedSet<ushort> ConstantsIndices { get; private set; }
        internal static SimpleFullGeneCollection ImmutableGenes { get; private set; }
        internal static SimpleImmutable[] ImmutableValues { get; private set; }
        private static float LastStatsChange;
        internal static SortedSet<byte>[] AllDormsReserved = new SortedSet<byte>[16];
        
        private static float LastSpeciesMaxFitness = 0;
        //private static float LastMinFitness = 0;
        private static byte SpeciesCounter = 0;

        private static string[] ActivityAbbrvs { get; set; }
        private static string[] DormAbbrvs { get; set; }

        internal static string[] Preview
        {
            get
            {
                var chromosome = Pool[0];
                var groups = chromosome.Genes.Where(
                    g => g.Dorm != 255
                ).Zip(
                    ImmutableValues,
                    (g, v) => new SimpleFullGene(v.Day, v.Time, v.Activity, g.Dorm, g.OtherDorm)
                ).GroupBy(
                    g => g.Day,
                    (d, grp) => new KeyValuePair<byte, IEnumerable<IGrouping<byte, SimpleFullGene>>>(
                        d, grp.GroupBy(
                            g => g.Time
                        )
                    )
                );

                List<string> feed = new List<string>();
                foreach(var day in groups)
                {
                    feed.Add(day.Key.ToString());
                    foreach(var time in day.Value)
                    {
                        feed.Add("  " + time.ToString());
                        foreach(var gene in time)
                        {
                            feed.Add(
                                String.Format(
                                    "    {0} - {1}, {2} ",
                                    ActivityAbbrvs[gene.Activity],
                                    DormAbbrvs[gene.Dorm],
                                    gene.OtherDorm == 255 ? "None" : DormAbbrvs[gene.OtherDorm]
                                )
                            );
                        }
                    }
                }

                return feed.ToArray();
            }
        }

        internal readonly static float[] FieldMutationRates = new float[4];

        static SimpleGenePool()
        {
            LogManager.AddLog("GeneticScheduling", LogType.FolderFilesByDate);
            LogManager.Start(true);

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
        public static void FreezeGenePool()
        {
            Frozen = true;
            GeneLength = (byte)GeneParameters.Sum(gp => gp.Bits);
            MinMutationChance = (1f / NUM_GENES) * GLOBAL_MUTATION_MULTIPLIER;
            MaxMutationChance = (float)Math.Sqrt(MinMutationChance);
            MutationChance = (float)Math.Pow(MinMutationChance, 3.0 / 4);

            GeneParams = GeneParameters.ToArray();
            GeneParameters = null;

            // could just make them for the mutable params
            ParamsValueGenerators = new ByteGenerator[NUM_PARAMS];
            for (int p = 0; p < NUM_PARAMS; ++p)
            {
                ParamsValueGenerators[p] = new ByteGenerator(GeneParams[p].MaxValue);
            }

            Console.WriteLine("MinMutationChance: {0}", MinMutationChance);
            Console.WriteLine("MaxMutationChance: {0}", MaxMutationChance);
            Console.WriteLine("MutationChance:    {0}", MutationChance);
        }

        // gene pool is a list of every possible event block, most of which will be inactive
        public static int CreateInitialPool(string infoFilesFolder)
        {
            Path = infoFilesFolder;
            AddParameterType("Day", false, 1, 2, 3, 4, 5);
            AddParameterType("Time", false, 0, 1, 2, 3, 4, 5, 6);

            string path = infoFilesFolder.TrimEnd('\\') + "\\";
            var activitiesCSV = Utils.LoadCSV<ActivityCSV>(path + "Activities.csv");
            var activities = activitiesCSV.Select(a => a.ToInfo()).OrderBy(a => a.Duration).ThenBy(a => a.Abbreviation).ToList();
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

            var immutableList = new List<SimpleFullGene>();

            BASE_TIMES = new sbyte[dorms.Length];

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
                var activityIndex = (byte)exclusives.First(a => a.Activity.Abbreviation == manual.Activity).Index;
                var dormIndex = (byte)Array.FindIndex(dorms, d => d.Abbreviation == manual.Dorm);
                do
                {
                    immutableList.Add(
                        new SimpleFullGene(
                            (byte)manual.Day,
                            time,
                            activityIndex,
                            dormIndex,
                            255
                        )
                    );
                    ++BASE_TIMES[dormIndex];
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

            ImmutableGenes = new SimpleFullGeneCollection(immutableList);
            immutableList = null;

            byte timeIndex = 0;
            for (byte day = 0; day < 4; ++day)
            {
                for (byte timeTemp = 0; timeTemp < 4; ++timeTemp)
                {
                    AllDormsReserved[timeIndex] = ImmutableGenes.DormsReserved(day, timeTemp);
                    ++timeIndex;
                }
            }

            activities.RemoveAll(a => a.Flags.HasFlag(ActivityFlags.Manual));
            ActivityAbbrvs = new string[activities.Count];
            var activityIDs = new byte[activities.Count];
            SortedDictionary<byte, byte> activityIDMappings = new SortedDictionary<byte, byte>();
            for (byte i = 0; i < activities.Count; ++i)
            {
                activityIDs[i] = i;
                ActivityAbbrvs[i] = activities[i].Abbreviation;
                activityIDMappings.Add(i, (byte)activities[i].ID);
            }
            activityIDMappings.SaveDictAs(infoFilesFolder + "ActivityIDMappings.txt");

            // need to watch out for using activities by ID
            AddParameterType("Activity", false, activityIDs);

            var dormBytes = Enumerable.Range(0, dorms.Count()).Select(d => (byte)d).Append<byte>(255).ToArray();
            DormAbbrvs = dorms.Select(d => d.Abbreviation).Append("None").ToArray();

            AddParameterType("Dorm", true, dormBytes);
            AddParameterType("OtherDorm", true, dormBytes.Skip(1).ToArray());
            dormBytes = null;

            //List<Gene> genePool = new List<Gene>();
            //List<Tuple<byte, byte, byte>> genePool = new List<Tuple<byte, byte, byte>>();
            List<SimpleImmutable> genePool = new List<SimpleImmutable>();
            geneID = 0;
            var activitiesByDuration = activities./*FindAll(a => !a.Flags.HasFlag(ActivityFlags.Concurrent))*/GroupBy(a => a.Duration, a => activities.FindIndex(a2 => a2.ID == a.ID)).OrderBy(g => g.Key);
            byte concurrentActivity = (byte)activities.FindIndex(a => a.Flags.HasFlag(ActivityFlags.Concurrent));

            SetupConsts(dorms, activities, path);
            //List<byte> times = new List<byte>();
            //List<byte> tailTimes = new List<byte>();
            //Constants.SURVIVAL_DAYINFO = new DaySurvivalInfo[(byte)blocks.Last().Day - (byte)blocks.First().Day];

            byte totalTimesCount = 0;
            int pointer = 0;
            byte currentDay = 0;
            //byte dayGenesCount = 0;
            //byte[] timesGenesCounts = new byte[7];
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
                totalTimesCount += available;
                //tailTimes.Add((byte)(timeTemp + available - 1));
                
                foreach (var duration in activitiesByDuration)
                {
                    if (duration.Key > currentBlocks.Count)
                        continue;
                    for (byte actualTime = timeTemp; actualTime < available + timeTemp; actualTime = (byte)(actualTime + duration.Key))
                    {
                        //byte actualTime = (byte)(currentTime + timeOffset);
                        foreach (var activity in duration)
                        {
                            genePool.Add(new SimpleImmutable(currentDay, actualTime, (byte)activity));
                            //++geneID;
                            //++dayGenesCount;
                            //++timesGenesCounts[actualTime];
                        }
                    }                    
                }

                for (byte actualTime = timeTemp; actualTime < available + timeTemp; ++actualTime)
                {
                    genePool.Add(new SimpleImmutable(currentDay, actualTime, concurrentActivity));
                    //++dayGenesCount;
                    //++timesGenesCounts[actualTime];
                }

                if (nextBlock.Day != lastBlock.Day)
                {
                    /*Constants.SURVIVAL_DAYINFO[currentDay] = new DaySurvivalInfo(
                        timesGenesCounts, 
                        tailTimes, 
                        dayGenesCount
                    );
                    Array.Clear(timesGenesCounts, 0, 7);
                    tailTimes.Clear();
                    dayGenesCount = 0;*/
                    ++currentDay;
                    currentTime = 0;
                }
            }

            NUM_BLOCKS = totalTimesCount;
            BASE_TIMES = BASE_TIMES.Select(t => (sbyte)(NUM_BLOCKS - t)).ToArray();

            int numGenes = genePool.Count;
            NUM_GENES = (ushort)numGenes;
            GENES_PER_DAY = (byte)(NUM_GENES / 4);
            // MaxValue is byte, while numGenes is ushort
            SimpleChromosome.CROSSOVER_GEN = new ByteGenerator((byte)numGenes);
            Pool = new SimpleChromosome[POOL_SIZE];
            SamplePool = new SimpleChromosome[SAMPLE_POOL_SIZE];
            Func<GeneParameter[]> getGeneParams = () => GeneParams;

            ImmutableValues = genePool.OrderBy(g => g.Day).ThenBy(g => g.Time).ThenBy(g => g.Activity).ToArray();
            var day1 = ImmutableValues.TakeWhile(g => g.Day == 0);
            GENES_PER_DAY = (byte)day1.Count();
            var time1 = day1.TakeWhile(g => g.Time == 0);
            NUM_ACTIVITIES = (byte)time1.Count();
            var time2 = day1.TakeWhile(g => g.Time == 0 && !ActivitySurvivalInfo.IsLonger(g.Activity));
            NUM_ODD_ACTIVITIES = (byte)time2.Count();
            SimpleChromosome.SECONDARY_SHIFT_BLOCK_SIZE = (byte)(NUM_ACTIVITIES + NUM_ODD_ACTIVITIES);

            FieldMutationRates[0] = (1 / (NUM_GENES / 5f)) * GLOBAL_MUTATION_MULTIPLIER;
            FieldMutationRates[1] = (1 / (NUM_GENES / 7f)) * GLOBAL_MUTATION_MULTIPLIER;
            FieldMutationRates[2] = (1 / (NUM_GENES / activities.Count)) * GLOBAL_MUTATION_MULTIPLIER;
            FieldMutationRates[3] = (1 / (NUM_GENES / dorms.Length)) * GLOBAL_MUTATION_MULTIPLIER;
            /*
            for (int i = 0; i < numGenes; ++i)
            {
                var gene = genePool[i];
                ImmutableValues[i, 0] = gene.Item1;
                ImmutableValues[i, 1] = gene.Item2;
                ImmutableValues[i, 2] = gene.Item3;
            }*/
            genePool = null;

            FreezeGenePool();

            for (int c = 0; c < SAMPLE_POOL_SIZE; ++c)
            {
                SimpleGene[] chromosome = new SimpleGene[numGenes];
                for (ushort g = 0; g < numGenes; ++g)
                {
                    chromosome[g] = new SimpleGene(g);
                }
                SamplePool[c] = new SimpleChromosome(chromosome);
            }

            SerializeGenePoolParams();

            if (!Directory.Exists(PATH + @"Species\"))
                Directory.CreateDirectory(PATH + @"Species\");
            else
            {
                foreach (var file in Directory.GetFiles(PATH + @"Species\"))
                {
                    File.Delete(file);
                }
            }

            DIVERSITY_DIVISOR = (float)(NUM_GENES * 2f * (Math.Log(SAMPLE_POOL_SIZE - 1)));

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
            var olderDorms = dormsByAge.SkipWhile(d => d.AgeGroup < 5);
            int numGirls = dorms.Count(d => d.IsGirl);
            var genderRatio = ((double)numGirls) / (numDorms - numGirls);
            var genderDiffRatio = ((genderRatio >= 1 ? genderRatio : (1.0 / genderRatio)) - 1);
            for (int i = 0; i < numDorms; ++i)
            {
                var dorm = dormsByAge[i];

                // Fitness
                DormFitnessInfo fitness = new DormFitnessInfo();

                var activityPriorities = dormActivityPriorities.TryGetValue(
                    dorm.Abbreviation,
                    out Dictionary<int, int> priorities
                ) ? priorities : new Dictionary<int, int>();

                foreach (var priority in activityPriorities)
                {
                    fitness.ActivityPriorities.Add((byte)priority.Key, (byte)priority.Value);
                }

                SortedSet<byte> allowedOtherDorms = null;
                IEnumerable<DormInfo> otherDorms = null;

                otherDorms = dormsByAge.Where(d => Math.Abs(d.AgeGroup - dorm.AgeGroup) <= 1);
                allowedOtherDorms = new SortedSet<byte>(otherDorms.Select(d => (byte)d.ID));
                otherDorms = otherDorms.Skip(i + 1).TakeWhile(d => (d.AgeGroup - dorm.AgeGroup) <= 1);
                if (dorm.Abbreviation == "5G")
                {
                    var extra = dormsByAge.Last();
                    otherDorms.Append(extra);
                    allowedOtherDorms.Add((byte)extra.ID);
                }
                else if (dorm.Abbreviation == "7G")
                {
                    var extra = dormsByAge[i - 3];
                    allowedOtherDorms.Add((byte)extra.ID);
                }

                allowedOtherDorms.Remove((byte)dorm.ID);

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
                    if (dorm.IsGirl)
                    {
                        foreach (var olderDorm in olderDorms.Where(d => d.IsGirl))
                        {
                            fitness.DormPriorities.Add(
                                (byte)olderDorm.ID,
                                1
                            );
                        }
                    }
                    else
                    {
                        foreach (var olderDorm in olderDorms)
                        {
                            fitness.DormPriorities.Add(
                                (byte)olderDorm.ID,
                                1
                            );
                        }
                    }
                }
                Constants.FITNESS_DORMINFO[i] = fitness;

                // Survival
                Constants.SURVIVAL_DORMINFO[i] = new DormSurvivalInfo(
                    dorm.AllowedExclusiveActivities.Select(a => (byte)a).ToArray(),
                    allowedOtherDorms.ToArray()
                );
            }

            // activities are stored by post-prune index so that they line up with the Constants arrays
            Constants.SURVIVAL_ACTIVITYINFO = activities.Select(
                info => new ActivitySurvivalInfo(info)
            ).ToArray();
            var activityMax = GeneParameters[2].MaxValue;
            List<byte> longer = new List<byte>();
            for (byte activity = 0; activity < activityMax; ++activity)
            {
                if (Constants.SURVIVAL_ACTIVITYINFO[activity].LongerDuration)
                    longer.Add(activity);
            }
            ActivitySurvivalInfo.LONGER = new SortedSet<byte>(longer);
            ActivitySurvivalInfo.NUM_LONGER = (byte)ActivitySurvivalInfo.LONGER.Count;
            Constants.FITNESS_ACTIVITYINFO = activities.Select(a => (byte)a.Priority).ToArray();
        }
        #endregion

        public static void Evolve(string infoFilesFolder)
        {
            Console.WriteLine("Chromosome length: {0}", CreateInitialPool(infoFilesFolder));

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            //Selection();

            float[] diversityOverTime = new float[30];
            float quality = 0;
            for (int i = 0; i < 30; ++i)
            {
                Selection(out _);
                float diversity = GeneticDiversity();
                quality = SolutionQuality();
                Console.WriteLine(
                    "Generation {0}\r\n\tDiversity: {1}\r\n\tQuality: {2}\r\n\tGeneration Performance: {3}",
                    Generation,
                    diversity,
                    quality,
                    SelectionPerformance(diversity, quality)
                );
                diversityOverTime[i] = diversity;
                Crossover();
                Mutate();
            }
            Console.WriteLine();
            var avgDiversity = diversityOverTime.Average();
            Console.WriteLine("Average Genetic Diversity: {0}", avgDiversity);
            Console.WriteLine("Final Performance: {0}", SelectionPerformance(avgDiversity, quality));
            Console.ReadLine();

            EvolutionStatus evoStatus = new EvolutionStatus();
            Thread evolutionThread = new Thread(
                new ParameterizedThreadStart(
                    obj =>
                    {
                        var status = (EvolutionStatus)evoStatus;
                        while(true)
                        {
                            if (Selection(out string speciesName))
                            {
                                if (String.IsNullOrEmpty(speciesName))
                                {
                                    /*lock (EvoLock)
                                    {
                                        Console.WriteLine();
                                        Console.WriteLine("ERROR: Stuck at local minima.");
                                        LogManager.WaitAndFlush();
                                        //LogManager.Stop();
                                        //break;
                                    }*/
                                }
                                else
                                {
                                    lock (EvoLock)
                                    {
                                        //Console.WriteLine();
                                        Console.WriteLine("New species created: {0} with fitness {1}", speciesName, 1f / LastSpeciesMaxFitness);
                                        //Console.WriteLine();
                                        LogManager.WaitAndFlush();
                                    }
                                }
                            }

                            lock (EvoLock)
                            {
                                ++status.Generation;
                                status.Stats = PoolLastStats;
                                status.MaxStat = Pool[0].Stats.GetStat();
                                status.MaxFitness = PoolMaxFitness;
                                status.MaxFertility = PoolMaxFertility;
                                status.PoolNumFit = PoolNumFit;
                                status.LastStatsChange = LastStatsChange;
                            }
                            
                            Crossover();
                            Mutate();

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

                        Thread.CurrentThread.Abort();
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
                            Console.WriteLine(
                                "Generation {0} - {1}/{2}, {3}",
                                evoStatus.Generation,
                                evoStatus.MaxFitness != 0 ? (1f / evoStatus.MaxFitness) : 0,
                                (1f / evoStatus.MaxFertility).ToString("#.0000"),
                                evoStatus.LastStatsChange.ToString("#.000000")
                            );
                            if (evolutionThread.ThreadState == System.Threading.ThreadState.Aborted || !evolutionThread.IsAlive)
                                break;
                        }
                    }
                    lock(EvoLock)
                    {
                        Console.WriteLine(
                            "Generation {0} - {1}", 
                            evoStatus.Generation, 
                            evoStatus.Stats.Take(POOL_HALF_MIDPOINT).ToArrayString("#.0000")
                        );
                        if (evolutionThread.ThreadState == System.Threading.ThreadState.Aborted || !evolutionThread.IsAlive)
                            break;
                    }
                }

                evoStatus._doPause = true;
                SpinWait.SpinUntil(() => evoStatus._isPaused);

                int cIndex = 0;
                lock(EvoLock)
                {
                    evoStatus.LastEvolutionStop = DateTime.Now;
                    evoStatus.SaveAs(PATH + "EvoStatus.json");

                    Stopwatch timer = Stopwatch.StartNew();
                    foreach (var chromosome in Pool)
                    {
                        ++cIndex;
                        chromosome.SerializeTo(PATH + "GenePool", "Chromosome" + cIndex.ToString());
                    }
                    timer.Stop();
                    Console.WriteLine("CHROMOSOMES SERIALIZED - {0} ms", timer.ElapsedMilliseconds);
                }

                if (evolutionThread.ThreadState == System.Threading.ThreadState.Aborted || !evolutionThread.IsAlive)
                    break;

                evolutionThread.Interrupt();
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("EVOLUTION ABORTED");
            Console.ReadLine();
        }

        private static void SerializeGenePoolParams()
        {
            Stopwatch timer = Stopwatch.StartNew();

            Constants.SerializeToFolder(PATH);

            GenePoolInfo info = new GenePoolInfo();
            foreach (var param in GeneParameterIndices)
            {
                info.GeneParameters.Add(param.Key, new KeyValuePair<GeneParameter, byte[]>(GeneParams[param.Value], ParamsPossibleValues[param.Value]));
            }
            info.MutationChance = MutationChance;
            info.NumGenes = NUM_GENES;
            info.NumImmutable = ImmutableGenes.Count;
            info.SaveAs(PATH + "GenePoolInfo.json");

            using (FileStream file = new FileStream(PATH + "ImmutableGenes.csm", FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                //file.WriteByte((byte)ImmutableGenes.Length);
                foreach (var immutable in ImmutableGenes)
                {
                    immutable.SerializeWith(file);
                }
            }

            using (FileStream file = new FileStream(PATH + "ImmutableValues.csm", FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                foreach (var immutable in ImmutableValues)
                {
                    file.WriteByte(immutable.Day);
                    file.WriteByte(immutable.Time);
                    file.WriteByte(immutable.Activity);
                }
            }

            timer.Stop();
            Console.WriteLine("GENE POOL PARAMS SERIALIZED - {0} ms", timer.ElapsedMilliseconds);
        }

        private static float DIVERSITY_DIVISOR;
        private static float GeneticDiversity()
        {
            float diversitySum = 0;
            for (int i = 0; i < POOL_SIZE; ++i)
            {
                for (int j = i + 1; j < POOL_SIZE; ++j)
                {
                    diversitySum += Pool[i].DistanceFrom(Pool[j]) * 2f;
                }
            }
            return diversitySum / DIVERSITY_DIVISOR;
        }

        private static float SolutionQuality()
        {
            float max = Pool[0].Stats.GetStat();
            float min = Pool[POOL_SIZE - 1].Stats.GetStat();
            return (float)(max / Math.Sqrt((max * max) + (min * min)));
        }

        private static float SelectionPerformance(float diversity, float quality) =>
            (diversity / Generation) + (((Generation - 1) * quality) / Generation);

        private static void TruncationSelection(KeyValuePair<ushort, EvoStats>[] evoStats)
        {
            // select a portion between 10% and 50%
            var selectionPortion = ((float)SimpleChromosome.GEN.NextDouble() * 0.4f) + 0.1f;
            int selectionPressure = (int)(SAMPLE_POOL_SIZE * selectionPortion);

            for (ushort chromosome = 0; chromosome < selectionPressure; ++chromosome)
            {
                Pool[0] = SamplePool[evoStats[0].Key];
            }
        }

        private static bool Selection(out string speciesName)
        {
            Stopwatch timer = Stopwatch.StartNew();
            bool firstTime = Pool[0] == null;

            // stats of each chromosome of the pool
            var evoStats = SamplePool.Select(
                (c, i) => new KeyValuePair<ushort, EvoStats>(
                    (ushort)i, c.Select()
                )
            ).OrderBy(
                kv => !kv.Value.Survives
            ).ThenByDescending(
                kv => kv.Value.Fitness
            ).ThenByDescending(
                kv => kv.Value.Fertility
            ).ToArray();

            /*SortedSet<KeyValuePair<ushort, EvoStats>> evoStats = new SortedSet<KeyValuePair<ushort, EvoStats>>(
                SamplePool.Select((c, i) => new KeyValuePair<ushort, EvoStats>((ushort)i, c.Select())),
                new EvoStatsComparer()
            );*/
            timer.Stop();



            var minStats = new EvoStats();
            if (!firstTime)
            {
                minStats = Pool[POOL_HALF_MIDPOINT].Stats;
                int numFitIndividuals = 0;
                if (minStats.Survives)
                    numFitIndividuals = evoStats.TakeWhile(c => c.Value.Survives && c.Value.Fitness >= minStats.Fitness).Count();
                else
                    numFitIndividuals = evoStats.TakeWhile(c => c.Value.Survives || c.Value.Fertility >= minStats.Fertility).Count();
                LogManager.Enqueue(
                    "GeneticScheduling",
                    EntryType.DEBUG,
                    "Selection found " + numFitIndividuals.ToString() + " fit individuals",
                    timer.ElapsedMilliseconds.ToString() + " ms"
                );
            }
            //SortedList<EvoStats, int> evoStats = new SortedList<EvoStats, int>(SAMPLE_POOL_SIZE, new EvoStatsComparer());

            timer.Restart();

            // inferior chromosomes aren't replaced so much as they are shifted down
            byte currentPoolSize = (byte)POOL_SIZE;
            List<SimpleChromosome> oldPool = new List<SimpleChromosome>(Pool);

            int statsPointer = 0;
            int startingIndex = -1;
            int currentNumFit = PoolNumFit;

            //var numFitCandidates = evoStats.TakeWhile(kv => kv.Value.GetStat() >= PoolMinFertility).Count();
            var candidates = evoStats.Take(POOL_MIDPOINT);
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
                        if (!candidatesEnumerator.MoveNext())
                            break;
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
                        if (!candidatesEnumerator.MoveNext())
                            break;
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

            if (PoolMaxFitness != 0)
            {
                if (first.Value.Fitness >= minStats.Fitness)
                {
                    // using greater-than-or-equals-to for diversity
                    startingIndex = Array.FindIndex(PoolLastStats, s => s <= first.Value.Fitness);
                    statsPointer = startingIndex;
                    if (startingIndex >= PoolNumFit)
                    {
                        // if the only replaceable chromosomes are unfit ones

                        candidates = candidates.TakeWhile(
                            c => c.Value.Survives ||
                                 c.Value.Fertility >= minStats.Fertility
                        );
                        candidatesEnumerator = candidates.GetEnumerator();

                        ReplaceUnfit();
                    }
                    else if (PoolNumFit != POOL_SIZE)
                    {
                        candidates = candidates.TakeWhile(
                            c => c.Value.Survives ||
                                 c.Value.Fertility >= minStats.Fertility
                        );
                        candidatesEnumerator = candidates.GetEnumerator();

                        ReplaceFit();

                        ReplaceUnfit();
                    }
                    else
                    {
                        candidates = candidates.TakeWhile(c => c.Value.Fitness >= minStats.Fitness);
                        candidatesEnumerator = candidates.GetEnumerator();

                        ReplaceFit();
                    }
                }
            }
            else if (PoolMaxFertility != 0)
            {
                if (first.Value.Fertility >= minStats.Fertility)
                {
                    startingIndex = Array.FindIndex(PoolLastStats, s => s <= first.Value.Fertility);
                    statsPointer = startingIndex;

                    candidates = candidates.TakeWhile(c => c.Value.Fertility >= minStats.Fertility);
                    candidatesEnumerator = candidates.GetEnumerator();

                    ReplaceUnfit();
                }
            }
            else
                oldPool.InsertRange(
                    0, candidates.Take(POOL_MIDPOINT).Select(
                        c => SamplePool[c.Key]
                    )
                );

            if (candidatesEnumerator != null)
                candidatesEnumerator.Dispose();

            // these limits prevented reconvergence
            if (!firstTime)// && currentPoolSize > POOL_MIDPOINT)
            {
                byte oldAge = 0;
                byte spontaneousDeath = 0;
                byte unfitDeath = (byte)(currentPoolSize > POOL_SIZE ? currentPoolSize - POOL_SIZE : 0);
                byte totalIterations = 0;

                for (byte chromosome = 0; chromosome < POOL_HALF_MIDPOINT && currentPoolSize > POOL_SIZE; ++chromosome)
                {
                    if (oldPool[chromosome].Age >= MAX_AGE)
                    {
                        if (SimpleChromosome.GEN.NextDouble() < OLD_AGE_DEATH_CHANGE)
                        {
                            oldPool.RemoveAt(chromosome);
                            --currentPoolSize;
                            --chromosome;
                            ++oldAge;
                        }
                    }
                    else if (oldPool[chromosome].Age > 0 && SimpleChromosome.GEN.NextDouble() < SPONTANEOUS_DEATH_CHANCE)
                    {
                        oldPool.RemoveAt(chromosome);
                        --currentPoolSize;
                        --chromosome;
                        ++spontaneousDeath;
                    }
                    ++totalIterations;
                }

                for (byte chromosome = (byte)POOL_HALF_MIDPOINT; chromosome < POOL_MIDPOINT && chromosome < currentPoolSize/* && currentPoolSize > POOL_MIDPOINT*/; ++chromosome)
                {
                    if (oldPool[chromosome].Age >= MAX_AGE)
                    {
                        if (SimpleChromosome.GEN.NextDouble() < OLD_AGE_DEATH_CHANGE)
                        {
                            oldPool.RemoveAt(chromosome);
                            --currentPoolSize;
                            --chromosome;
                            ++oldAge;
                        }
                    }
                    else if (/*oldPool[chromosome].Age > 0 && */SimpleChromosome.GEN.NextDouble() < SPONTANEOUS_DEATH_CHANCE)
                    {
                        oldPool.RemoveAt(chromosome);
                        --currentPoolSize;
                        --chromosome;
                        ++spontaneousDeath;
                    }
                    ++totalIterations;
                }
                if (oldAge > 0 || spontaneousDeath > 0 || unfitDeath > 0)
                    LogManager.Enqueue(
                        "GeneticScheduling",
                        EntryType.DEBUG,
                        totalIterations.ToString() + " iterations",
                        String.Format(
                            "{0} chromosomes died from competition, {1} from old age, and {2} from bad luck",
                            unfitDeath,
                            oldAge,
                            spontaneousDeath
                        )
                    );
            }

            oldPool = new List<SimpleChromosome>(oldPool.Take(Math.Min(POOL_MIDPOINT, currentPoolSize)));

            //var noncandidates = evoStats.Skip(POOL_SIZE).ToArray();
            byte diverseChromosomesNeeded = (byte)(POOL_SIZE - oldPool.Count);
            for (byte chromosome = 0; chromosome < diverseChromosomesNeeded; ++chromosome)
            {
                oldPool.Add(
                    SamplePool[
                        evoStats[
                            POOL_SIZE + (ushort)((SimpleChromosome.GEN.NextDouble() * SimpleChromosome.GEN.NextDouble()) * MAX_NONCANDIDATE_INDEX)
                        ].Key
                    ]
                );
            }

            Pool = oldPool.OrderBy(
                p => !p.Stats.Survives
            ).ThenByDescending(
                p => p.Stats.Fitness
            ).ThenByDescending(
                p => p.Stats.Fertility
            ).ToArray();

            if (!firstTime)
            {
                float totalChange = 0;
                for (byte chromosome = 0; chromosome < (POOL_HALF_MIDPOINT); ++chromosome)
                {
                    var obj = Pool[chromosome];
                    var stat = obj.Stats.GetStat();
                    totalChange += stat - PoolLastStats[chromosome];
                    PoolLastStats[chromosome] = stat;

                    obj.FlushLogBuffer();
                    ++obj.Age;
                }

                totalChange /= POOL_SIZE;
                var maxStats = Pool[0].Stats;
                totalChange = (totalChange + (maxStats.Survives ? maxStats.Fitness - PoolMaxFitness : maxStats.Fertility - PoolMaxFertility)) / 2;
                LogManager.Enqueue(
                    "GeneticScheduling",
                    EntryType.DEBUG,
                    "Average change of " + totalChange.ToString("#.000000") + " to pool fitness"
                );
                LastStatsChange = totalChange;
            }
            else
                PoolLastStats = Pool.Select(c => c.Stats.GetStat()).ToArray();
            
            PoolNumFit = Pool.Take(POOL_HALF_MIDPOINT).TakeWhile(c => c.Stats.Survives).Count();
            //PoolMinFitness = PoolNumFit > 0 ? PoolLastStats[PoolNumFit - 1] : 0;
            //PoolMinFertility = PoolNumFit < POOL_MIDPOINT ? PoolLastStats[POOL_MIDPOINT - 1] : Pool[POOL_MIDPOINT - 1].Stats.Fertility;
            Array.Clear(SamplePool, 0, SAMPLE_POOL_SIZE);
            float maxStat = PoolLastStats[0];
            bool doSpeciesCounter = false;

            if (PoolNumFit == 0)
            {
                PoolMaxFitness = 0;
                doSpeciesCounter = maxStat <= PoolMaxFertility;
                PoolMaxFertility = maxStat;
            }
            else
            {
                if (PoolMaxFitness != 0)
                    doSpeciesCounter = maxStat <= PoolMaxFitness;
                PoolMaxFitness = maxStat;
                PoolMaxFertility = Pool[0].Stats.Fertility;
            }

            timer.Stop();

            LogManager.Enqueue(
                "GeneticScheduling",
                EntryType.DEBUG,
                "Selection complete",
                timer.ElapsedMilliseconds.ToString() + " ms"
            );

            //var minStat = PoolNumFit > 0 ? PoolMinFitness : PoolMinFertility;
            if (doSpeciesCounter/* || LastStatsChange < 0.00005*/)
            {
                ++SpeciesCounter;
                if (SpeciesCounter > MAX_AGE)
                {
                    var species = Pool[0];
                    //var maxStat = species.Stats.GetStat();
                    speciesName = species.ChromosomeID.ToString();
                    if (maxStat <= LastSpeciesMaxFitness/* || maxStat - LastSpeciesMaxFitness < 0.0025*/)
                    {
                        LogManager.Enqueue(
                            "GeneticScheduling",
                            EntryType.ERROR,
                            "NEW SPECIES DEVELOPED",
                            speciesName + " has fitness less or equal to last species': " + maxStat.ToString("#.00000")
                        );
                        speciesName = null;
                        SpeciesCounter = 0;
                        return true;
                    }

                    species.SerializeTo(PATH + @"Species\", speciesName);
                    LogManager.Enqueue(
                        "GeneticScheduling",
                        EntryType.DEBUG,
                        "NEW SPECIES DEVELOPED",
                        speciesName + " with fitness " + maxStat.ToString("#.00000")
                    );
                    LastSpeciesMaxFitness = maxStat;

                    for (byte c = (byte)POOL_HALF_MIDPOINT + 1; c < POOL_MIDPOINT; ++c)
                    {
                        if (SimpleChromosome.GEN.NextDouble() < NEW_SPECIES_IMMIGRATION_CHANCE)
                        {
                            SimpleGene[] chromosome = new SimpleGene[NUM_GENES];
                            for (ushort g = 0; g < NUM_GENES; ++g)
                            {
                                chromosome[g] = new SimpleGene(g);
                            }
                            Pool[c] = new SimpleChromosome(chromosome);
                            Pool[c].Select();
                        }
                    }

                    SpeciesCounter = 0;
                    Pool = Pool.OrderBy(
                        p => !p.Stats.Survives
                    ).ThenByDescending(
                        p => p.Stats.Fitness
                    ).ThenByDescending(
                        p => p.Stats.Fertility
                    ).ToArray();
                    return true;

                }
            }
            else
                SpeciesCounter = 0;
            speciesName = null;

            ++Generation;
            
            return false;
        }

        private static void Crossover()
        {
            Stopwatch timer = Stopwatch.StartNew();
            int numOffspring = 0;
            int numIterations = 0;
            List<KeyValuePair<byte, byte>> crossoversHistory = new List<KeyValuePair<byte, byte>>();
            while (numOffspring < SAMPLE_POOL_SIZE)
            {
                var index1 = ProdDist();
                SimpleChromosome[] offspring = null;

                if (SimpleChromosome.GEN.NextDouble() < MutationChance)
                {
                    var crossoverPair = new KeyValuePair<byte, byte>(index1, 255);
                    if (crossoversHistory.Contains(crossoverPair))
                        continue;
                    SimpleGene[] chromosome = new SimpleGene[NUM_GENES];
                    for (ushort g = 0; g < NUM_GENES; ++g)
                    {
                        chromosome[g] = new SimpleGene(g);
                    }
                    SimpleChromosome randomSecondParent = new SimpleChromosome(chromosome);
                    crossoversHistory.Add(crossoverPair);
                    offspring = Pool[index1].CrossoverWith(randomSecondParent, POOL_MIDPOINT);
                }
                else
                {
                    var index2 = ProdDist();
                    while (index2 == index1)
                        index2 = ProdDist();
                    if (index1 < index2)
                    {
                        var temp = index1;
                        index1 = index2;
                        index2 = temp;
                    }
                    var crossoverPair = new KeyValuePair<byte, byte>(index1, index2);
                    if (crossoversHistory.Contains(crossoverPair))
                        continue;

                    crossoversHistory.Add(crossoverPair);
                    offspring = Pool[index1].CrossoverWith(Pool[index2], POOL_MIDPOINT);
                }

                var maxOffspring = Math.Min(offspring.Length, SAMPLE_POOL_SIZE - numOffspring);
                for (int i = 0; i < maxOffspring; ++i)
                {
                    SamplePool[numOffspring] = offspring[i];
                    ++numOffspring;
                }
                ++numIterations;
            }

            timer.Stop();
            LogManager.Enqueue(
                "GeneticScheduling", 
                EntryType.DEBUG,
                "Sample pool filled in " + numIterations.ToString() + " crossover iterations",
                timer.ElapsedMilliseconds.ToString() + " ms"
            );
        }

        private static void Mutate()
        {
            ushort[] numMutated = new ushort[SAMPLE_POOL_SIZE];
            Stopwatch timer = Stopwatch.StartNew();
            for (int i = 0; i < SAMPLE_POOL_SIZE; ++i)
            {
                numMutated[i] = SamplePool[i].Mutate();
            }
            Array.Sort(numMutated);
            timer.Stop();
            LogManager.Enqueue(
                "GeneticScheduling",
                EntryType.DEBUG,
                String.Format(
                    "Avg/min/max mutations = {0}/{1}/{2}",
                    (ushort)numMutated.Average(n => n),
                    numMutated[0],
                    numMutated.Last()
                ),
                timer.ElapsedMilliseconds.ToString() + " ms"
            );
        }

        // don't round or can return index POOL_SIZE
        private static byte ProdDist() =>
            (byte)(SimpleChromosome.GEN.NextDouble() * SimpleChromosome.GEN.NextDouble() * POOL_SIZE);

        private class EvolutionStatus
        {
            [JilDirective(true)]
            public volatile bool _isPaused;
            [JilDirective(true)]
            public volatile bool _doPause;

            public int Generation { get; set; }
            public float MaxStat { get; set; }
            public float MaxFitness { get; set; }
            public float MaxFertility { get; set; }
            public float[] Stats { get; set; }
            public int PoolNumFit { get; set; }
            public float LastStatsChange { get; set; }

            [JilDirective(false)]
            private DateTime LastEvolutionStart { get; set; }
            public DateTime LastEvolutionStop { get; set; }

            public EvolutionStatus()
            {
                LastEvolutionStart = DateTime.Now;
                LastEvolutionStop = DateTime.MinValue;
                Generation = 0;
                MaxFitness = 0;
                MaxFertility = 0;
                LastStatsChange = 0;
                PoolNumFit = 0;
                MaxStat = 0;
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
