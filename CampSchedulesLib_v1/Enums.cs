using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedulesLib_v1
{
    [Flags]
    public enum Dorm
    {
        None = 0,
        _1B = 1,
        _2B,
        _3B = 4,
        _4B = 8,
        _5B = 16,
        _6B = 32,
        _1G = 64,
        _2G = 128,
        _3G = 256,
        _4G = 512,
        _5G = 1024,
        _6G = 2048,
        _7G = 4096
    };

    [Flags]
    public enum Activity
    {
        NS = 1,
        AC,
        A2 = 4,
        AF = 8,
        AR = 16,
        BS = 32,
        CA = 64,
        CC = 128,
        WH = 256,
        CK = 512,
        CL = 1024,
        VB = 2048,
        CV = 4096,
        CW = 8192,
        C4 = 16384,
        GB = 32768,
        HF = 65536,
        WS = 131072

        /*NineSquare = 1,
        ArtsCrafts,
        ArtsCrafts2 = 4,
        ActivityField = 8,
        Archery = 16,
        Blacksmithing = 32,
        Canoeing = 64,
        CounselorsChoice = 128,
        WaterfallHike = 256,
        CookingClass = 512,
        ChristianLiving = 1024,
        Volleyball = 2048,
        Caving = 4096,
        ClimbingWall = 8192,
        ConnectFour = 16384,
        Gagaball = 32768,
        HumanFoosball = 65536,
        WildernessSurvival = 131072*/
    };

    [Flags]
    public enum SpecialActivity
    {
        None = 0,
        CT,
        CV,
        CE = 4,
        HR = 8,
        LR = 16,
        RC = 32

        /*None = 0,
        CanoeTripOffsite,
        Caving,
        CreekExploration = 4,
        HighRopes = 8,
        LowRopesChallengeCourse = 16,
        RockClimbing = 32*/
    }
}
