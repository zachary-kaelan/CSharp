using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LojbanLib.Helpers;

namespace LojbanLib.Models
{
    public struct Triple
    {
        public char InitialLetter { get; set; }
        public char MedialLetter { get; set; }
        public char FinalLetter { get; set; }
        public bool IsValid { get; private set; }

        public Triple(char initial, char medial, char final)
        {
            InitialLetter = initial;
            MedialLetter = medial;
            FinalLetter = final;

            IsValid = false;
        }

        private bool Initialize()
        {
            if (LojbanData.VOWELS.Contains(InitialLetter) || LojbanData.VOWELS.Contains(MedialLetter) || LojbanData.VOWELS.Contains(FinalLetter))
                return false;

            if (InitialLetter == 'n')
            {
                if (MedialLetter == 'd')
                {
                    if (FinalLetter == 'j' || FinalLetter == 'z')
                        return false;
                }
                else if (MedialLetter == 't')
                {
                    if (FinalLetter == 'c' || FinalLetter == 's')
                        return false;
                }
            }

            Pair initialPair = new Pair(InitialLetter, MedialLetter, false);
            if (!initialPair.IsValid)
                return false;
            Pair finalPair = new Pair(MedialLetter, FinalLetter, false);
            if (!finalPair.IsValid || !MorphologyHelper.IsPermissibleInitialPair(MedialLetter, FinalLetter))
                return false;

            return true;
        }
    }
}
