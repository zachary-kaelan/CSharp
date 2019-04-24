using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LojbanLib.Helpers;

namespace LojbanLib.Models
{
    public struct Pair : IComparable<Pair>, IEquatable<Pair>
    {
        public char InitialLetter { get; set; }
        public char FinalLetter { get; set; }
        public bool IsDiphthong { get; private set; }
        public bool IsValid { get; private set; }
        public bool IsConsonantPair { get; private set; }

        internal Pair(bool isValid, char initial, char final, bool isDiphthong)
        {
            InitialLetter = initial;
            FinalLetter = final;
            IsDiphthong = isDiphthong;
            IsValid = isValid;
            IsConsonantPair = false;
            if (!IsDiphthong)
                IsConsonantPair = GetConsonantPairStatus();
        }

        internal Pair(char initial, char final, bool isDiphthong) : this()
        {
            InitialLetter = initial;
            FinalLetter = final;
            Initialize(isDiphthong);
        }

        public Pair(char initial, char final) : this()
        {
            InitialLetter = initial;
            FinalLetter = final;
            Initialize();
        }

        public Pair(bool isValid, char initial, char final) : this()
        {
            InitialLetter = initial;
            FinalLetter = final;
            IsValid = isValid;
            IsConsonantPair = false;
            IsDiphthong = LojbanData.DIPHTHONGS.Contains(this);
            if (!IsDiphthong)
                IsConsonantPair = GetConsonantPairStatus();
        }

        public Pair(string pair) : this()
        {
            if (pair.Length != 2)
                throw new IndexOutOfRangeException("Inputted string is not of length 2.");
            InitialLetter = pair[0];
            FinalLetter = pair[1];
            Initialize();
        }

        private void Initialize(bool isDiphthong)
        {
            IsDiphthong = isDiphthong;
            IsConsonantPair = GetConsonantPairStatus();
            IsValid = GetValidity();
        }

        private void Initialize() => Initialize(LojbanData.DIPHTHONGS.Contains(this));

        private bool GetValidity() => IsDiphthong || (
                 IsConsonantPair && !(
                    InitialLetter == FinalLetter ||
                    InitialLetter.IsVoiced() != FinalLetter.IsVoiced() ||
                    (MorphologyHelper.CLUSTER_FORBIDDEN_SHARED_CHARS.Contains(InitialLetter) && MorphologyHelper.CLUSTER_FORBIDDEN_SHARED_CHARS.Contains(FinalLetter)) ||
                    (InitialLetter == 'x' && (FinalLetter == 'c' || FinalLetter == 'k')) || (FinalLetter == 'x' && (InitialLetter == 'c' || InitialLetter == 'k')) ||
                    (InitialLetter == 'm' && FinalLetter == 'z')
                )
            );

        private bool GetConsonantPairStatus() => !IsDiphthong && !LojbanData.VOWELS.Contains(InitialLetter) && !LojbanData.VOWELS.Contains(FinalLetter);

        public int CompareTo(Pair other)
        {
            int comparison = InitialLetter.CompareTo(other.InitialLetter);
            return comparison == 0 ?
                FinalLetter.CompareTo(other.FinalLetter) :
                comparison;
        }

        public bool Equals(Pair other) => 
            InitialLetter == other.InitialLetter && 
            FinalLetter == other.FinalLetter;

        public override string ToString() => InitialLetter.ToString() + FinalLetter;
    }
}
