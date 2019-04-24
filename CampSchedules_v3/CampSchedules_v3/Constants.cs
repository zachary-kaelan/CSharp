using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZachLib;

namespace CampSchedules_v3
{
    internal static class Constants
    {

        static Constants()
        {
            ALL_POWERS_OF_2_REVERSE = new List<ulong>(64);
            ALL_POWERS_OF_2_REVERSE.Add(1);
            ulong prev = 1;
            for (int i = 1; i < 64; ++i)
            {
                prev *= 2;
                ALL_POWERS_OF_2_REVERSE.Add(prev);
            }
        }

        public static readonly Random GEN = new Random();

        public static readonly List<byte> POWERS_OF_2 = new List<byte>() { 128, 64, 32, 16, 8, 4, 2, 1 };
        public static List<ulong> ALL_POWERS_OF_2_REVERSE { get; private set; }
        public static readonly List<byte> POWERS_OF_2_REVERSE = new List<byte>() { 1, 2, 4, 8, 16, 32, 64, 128 };

        public static DormSurvivalInfo[] SURVIVAL_DORMINFO { get; set; }
        public static ActivitySurvivalInfo[] SURVIVAL_ACTIVITYINFO { get; set; }
        //public static DaySurvivalInfo[] SURVIVAL_DAYINFO { get; set; }
        public static DormFitnessInfo[] FITNESS_DORMINFO { get; set; }
        public static byte[] FITNESS_ACTIVITYINFO { get; set; }

        internal static void SerializeToFolder(string folder) =>
            new InfoParams().SaveAs(folder + "InfoParams.json");

        private class InfoParams
        {
            public DormSurvivalInfo[] DormSurvivalInfos { get; private set; }
            public ActivitySurvivalInfo[] ActivitySurvivalInfos { get; private set; }
            //public DaySurvivalInfo[] DaySurvivalInfos { get; private set; }
            public DormFitnessInfo[] DormFitnessInfos { get; private set; }
            public byte[] ActivityFitnessInfos { get; private set; }

            public InfoParams()
            {
                DormSurvivalInfos = SURVIVAL_DORMINFO;
                ActivitySurvivalInfos = SURVIVAL_ACTIVITYINFO;
                //DaySurvivalInfos = SURVIVAL_DAYINFO;
                DormFitnessInfos = FITNESS_DORMINFO;
                ActivityFitnessInfos = FITNESS_ACTIVITYINFO;
            }
        }
    }
}