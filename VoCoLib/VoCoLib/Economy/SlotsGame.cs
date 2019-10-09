using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoCoLib.Economy
{
    public enum Slot : byte
    {
        Grey_Question,
        X,
        Cherries,
        Donator,
        DonatorPlus,
        Gem,
        VCN,
        VoCrate
    }

    public struct SlotsGame
    {
        // Minimum bet of 5, maximum bet of 15 for initiate/guest
        public byte Bet { get; set; }
        public Slot Slot1 { get; set; }
        public Slot Slot2 { get; set; }
        public Slot Slot3 { get; set; }
        public short Result { get; set; }

        public SlotsGame(byte bet, Slot slot1, Slot slot2, Slot slot3, short result)
        {
            Bet = bet;
            Slot1 = slot1;
            Slot2 = slot2;
            Slot3 = slot3;
            Result = result;
        }
    }
}
