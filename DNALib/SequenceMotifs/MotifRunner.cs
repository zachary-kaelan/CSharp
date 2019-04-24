using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DNALib.SequenceMotifs
{
    internal class MotifRunner<TElement> where TElement : Enum
    {
        private int Length { get; set; }
        private Func<Motif<TElement>> GetMotif;
        private Func<IReadOnlyList<TElement>> GetSequence;
        private List<TElement> Sequence;
        private int LastCodeIndex { get; set; }

        private int CodeIndex = -1;
        private int SequenceIndex = 0;
        private MotifOp<TElement> CurrentOp { get; set; }
        private Func<TElement, bool> CurrentOpFunc = null;

        private int CurrentOpMatches = 0;
        private bool IsLooping = false;

        // the remaining minimum code length
        private int RemainingLength { get; set; }
        private int Marker = -1;
        private List<MotifMatch<TElement>> Matches = new List<MotifMatch<TElement>>();

        public MotifRunner(Motif<TElement> motif, IReadOnlyList<TElement> sequence)
        {
            Length = sequence.Count;
            LastCodeIndex = motif.Code.Length - 1;
            RemainingLength = Length;
            GetMotif = () => motif;
            GetSequence = () => sequence;
        }

        private bool Run(int startIndex, int count, int maxMatches)
        {
            ILGenerator gen = null;
            gen.Emit(
                //OpCodes.
            );

            var motif = GetMotif();
            if (Length < motif.MinimumSequenceLength)
                return false;

            bool doCount = count < Length - startIndex;
            bool doStartIndex = startIndex > 0 && startIndex < Length - 1;
            IncrementCodeIndex();

            Sequence = (doStartIndex || doCount ? (
                        doStartIndex && doCount ?
                            GetSequence().Skip(startIndex).Take(count) : (
                                doStartIndex ?
                                    GetSequence().Skip(startIndex) :
                                    GetSequence().Take(count)
                            )
                    ) : GetSequence()).ToList();

            for (int i = 0; i < Length; ++i)
            {
                //var op = Code[codeIndex];
                
            }

            return false;
        }

        private bool Go(Motif<TElement> motif, int opIndex, int startIndex, int remainingLength)
        {
            return false;
        }

        private bool Loop(Motif<TElement> motif, int opIndex, int startIndex, int remainingLength)
        {
            var func = motif.Delegates[opIndex];
            int initialRemainingLength = remainingLength;
            int opMatches = 0;
            var op = motif.Code[opIndex];
            int minFinalIndex = startIndex + op.Lower;

            bool isMatch = true;
            TElement current = default(TElement);
            for (int i = startIndex; isMatch && i <= minFinalIndex; ++i)
            {
                current = Sequence[i];
                if (!func(current))
                    isMatch = false;
            }

            

            if (isMatch)
            {
                // if a range instead of a set count
                if (op.Upper != -1)
                {
                    // doesn't bother running ranges that leave insufficient length for remaining code
                    // remainingLength includes the lower bound of this loop op
                    int maxFinalIndex = Math.Min(startIndex + op.Upper, startIndex + remainingLength);
                    int index;
                    bool nextIsLoop = motif.Code[opIndex + 1].Code.HasFlag(MotifCode.Loop);
                    isMatch = false;

                    // potential range includes the previously-confirmed match at the end of the lower bound

                    // sets the index to the maximum of the range
                    for (index = minFinalIndex; func(current) && index <= maxFinalIndex; ++index) { }

                    if (opIndex == LastCodeIndex)
                    {
                        EndMatch(index);
                        return true;
                    }

                    for (; index >= minFinalIndex; --index) // greedy by default
                    {
                        // forbids matches from being subsequences of each other by default
                        if (nextIsLoop)
                        {
                            if (
                                Loop(
                                    motif,
                                    opIndex + 1,
                                    index + 1,
                                    remainingLength - (index - startIndex)
                                )
                            )
                                return true;
                        }
                        else
                        {
                            if (
                                Go(
                                    motif,
                                    opIndex + 1,
                                    index + 1,
                                    remainingLength - (index - startIndex)
                                )
                            )
                                return true;
                        }
                    }
                }
                else if (opIndex == LastCodeIndex)
                {
                    EndMatch(minFinalIndex);
                    return true;
                }
            }
            
            return isMatch;
        }

        private void EndMatch(int endIndex)
        {
            int count = endIndex - SequenceIndex;
            Matches.Add(
                new MotifMatch<TElement>(
                    SequenceIndex,
                    count,
                    Sequence.GetRange(SequenceIndex, count).ToArray()
                )
            );
        }

        private bool IsOpMatch(int opIndex, TElement element)
        {
            bool isCurrentOp = opIndex == -1;
            var op = isCurrentOp ? CurrentOp : GetMotif().Code[opIndex];
            if (op.Code.HasFlag(MotifCode.Loop))
            {
                if ((IsLooping ? CurrentOpFunc : GetMotif().Delegates[opIndex])(element))
                {
                    ++CurrentOpMatches;
                    return true;
                }
                return false;
            }

            bool result = op.Code.HasFlag(MotifCode.One) ?
                element.Equals(op.Value) :
                op.Value.HasFlag(element);
            return op.Code.HasFlag(MotifCode.Not) ?
                !result : result;
        }

        private void IncrementCodeIndex()
        {
            var motif = GetMotif();
            ++CodeIndex;
            CurrentOp = motif.Code[CodeIndex];
            CurrentOpMatches = 0;
            CurrentOpFunc = null;
            if (CurrentOp.Code.HasFlag(MotifCode.Loop))
            {
                CurrentOpFunc = motif.Delegates[CodeIndex];
                IsLooping = true;
            }
        }
    }
}
