using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNALib.SequenceMotifs
{
    public class Motif<TElement> where TElement : Enum, IConvertible
    {
        internal MotifOp<TElement>[] Code { get; set; }
        internal int MinimumSequenceLength = 0;
        internal SortedDictionary<int, Func<TElement, bool>> Delegates = new SortedDictionary<int, Func<TElement, bool>>();
        private Func<char, int> CHARSET;

        public Motif(string str)
        {
            ParseMotif(str);
        }

        private static readonly SortedSet<char> OPEN_GROUP = new SortedSet<char>() { '[', '{' };
        private static readonly SortedSet<char> CLOSE_GROUP = new SortedSet<char>() { ']', '{' };

        private void ParseMotif(string str)
        {
            if (typeof(TElement) == typeof(DNACodon))
                CHARSET = c => MotifConstants.CODON_CHARS.TryGetValue(c, out DNACodon codon) ? (int)codon : -1;
            else
                CHARSET = c => Constants.NUCLEOTIDE_LETTERS.IndexOf(c);

            List<MotifOp<TElement>> code = new List<MotifOp<TElement>>();
            if (str.Contains('-'))
            {
                var fragments = str.Split('-');
                foreach (var fragment in fragments)
                    ParseFragment(fragment, code);
            }
            else
                ParseFragment(str, code);
            Code = code.ToArray();
        }

        private void ParseFragment(string str, List<MotifOp<TElement>> code)
        {
            int length = str.Length;
            for (int i = 0; i < length; ++i)
            {
                int val = 0;
                TElement element = default(TElement);
                char chr = str[i];
                if (OPEN_GROUP.Contains(chr))
                {
                    char open = chr;
                    List<char> chars = new List<char>();
                    ++i;
                    chr = str[i];
                    do
                    {
                        chars.Add(chr);
                        val += CHARSET(chr);
                        ++i;
                        chr = str[i];
                    } while (!CLOSE_GROUP.Contains(chr));

                    MotifCode temp = MotifCode.NIL;
                    element = (TElement)Enum.ToObject(typeof(TElement), val);

                    temp |= chars.Count == 1 ? MotifCode.One : MotifCode.Set;
                    if (chr == ']')
                        temp |= MotifCode.Not;

                    if (TryGetRange(str.Substring(i + 1), out int lower, out int upper))
                    {
                        MinimumSequenceLength += lower;
                        temp |= MotifCode.Loop;
                        var op = new MotifOp<TElement>(temp, element, lower, upper);
                        code.Add(op);
                        AddDelegate(code.Count - 1, op);
                        i = str.IndexOf(')', i + 3);
                    }
                    else
                    {
                        ++MinimumSequenceLength;
                        code.Add(new MotifOp<TElement>(temp, element, -1, -1));
                    }
                }
                else
                {
                    ++MinimumSequenceLength;
                    if (chr == 'x')
                        code.Add(
                            new MotifOp<TElement>(
                                MotifCode.Skip, 
                                element, 
                                -1, -1
                            )
                        );
                    else if (CHARSET(chr) == -1)
                        code.Add(
                            new MotifOp<TElement>(
                                MotifCode.One, 
                                element, 
                                -1, -1
                            )
                        );
                    else
                        throw new FormatException("Motif pattern is in an invalid format.");
                }
            }
        }

        private static bool TryGetRange(string str, out int lower, out int upper)
        {
            char chr = str[0];
            lower = -1;
            upper = -1;
            if (chr == '(')
            {
                List<char> numberChars = new List<char>();
                for (int i = 1; i < str.Length; ++i)
                {
                    chr = str[i];
                    if (Char.IsDigit(chr))
                        numberChars.Add(chr);
                    else
                    {
                        int bound = numberChars.Count == 1 ?
                            Convert.ToInt32(Char.GetNumericValue(numberChars[0])) :
                            Convert.ToInt32(new string(numberChars.ToArray()));
                        if (lower == -1)
                            lower = bound;
                        else
                        {
                            upper = bound;
                            return true;
                        }
                        if (chr == ',')
                            numberChars.Clear();
                        else
                            return true;
                    }
                }
                return false;
            }
            else
                return false;
        }

        private void AddDelegate(int index, MotifOp<TElement> op)
        {
            bool not = op.Code.HasFlag(MotifCode.Not);
            Func<TElement, bool> func = null;
            if(op.Code.HasFlag(MotifCode.One))
            {
                if (not)
                    func = c => !op.Value.Equals(c);
                else
                    func = c => op.Value.Equals(c);
            }
            else
            {
                if (not)
                    func = c => !op.Value.HasFlag(c);
                else
                    func = c => op.Value.HasFlag(c);
            }
            Delegates.Add(index, func);
        }
    }
}
