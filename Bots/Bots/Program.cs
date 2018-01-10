using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

class Solution
{
    private static bool HasCalculated = false;
    private static KeyValuePair<int, int>[] moveSet = null;
    private static List<string> moves = new List<string>();
    private static int botY = 0;
    private static int botX = 0;

    public static void next_move(int posr, int posc, String[] board)
    {
        if (!HasCalculated)
        {
            botX = posr;
            botY = posc;

            KeyValuePair<int, int>[] dirt = board.SelectMany(
               (s, j) => s.ToList()
               .Select((c, i) => new { c = c, i = i })
               .ToList()
               .FindAll(
                   t => t.c == 'd')
               .Select(
                   t => new KeyValuePair<int, int>(t.i, j)
               )
           ).ToArray();


            Dictionary<KeyValuePair<KeyValuePair<int, int>, KeyValuePair<int, int>>, int> moveCosts = new Dictionary<KeyValuePair<KeyValuePair<int, int>, KeyValuePair<int, int>>, int>();
            for (int i = 0; i < dirt.Length; ++i)
            {
                for (int j = i + 1; j < dirt.Length; ++j)
                {
                    int cost = Math.Abs(dirt[i].Key - dirt[j].Key) +
                           Math.Abs(dirt[i].Value - dirt[j].Value);
                    moveCosts.Add(
                        new KeyValuePair<KeyValuePair<int, int>, KeyValuePair<int, int>>(
                            dirt[i], dirt[j]
                        ), cost
                    );

                    moveCosts.Add(
                        new KeyValuePair<KeyValuePair<int, int>, KeyValuePair<int, int>>(
                            dirt[j], dirt[i]
                        ), cost
                    );
                }
            }

            var permutations = Permutate<KeyValuePair<int, int>>(dirt, dirt.Length)
                .Select(p => p.ToList()).ToList()
                .Select(
                    p => new KeyValuePair<int, List<KeyValuePair<int, int>>>(
                        p.GetRange(0, p.Count - 1)
                        .Select((d, i) => new { d = d, i = i })
                        .Sum(
                            t => moveCosts[
                                new KeyValuePair<KeyValuePair<int, int>, KeyValuePair<int, int>>(
                                    t.d, p[t.i + 1]
                                )
                            ]
                        ), p
                    )
                );
            dirt = null;
            int minimumCost = permutations.Min(p => p.Key);
            moveSet = permutations.First(p => p.Key == minimumCost).Value.ToArray();
            permutations = null;
            moveCosts = null;

            foreach (KeyValuePair<int, int> move in moveSet)
            {
                MoveToObjective(move.Key, move.Value);
                moves.Add("CLEAN");
            }
        }

        Console.WriteLine(moves[0]);
        moves.RemoveAt(0);
    }

    private static void MoveToObjective(int targetX, int targetY)
    {
        while (targetY != botY)
        {
            if (targetY > botY)
            {
                ++botY;
                moves.Add("DOWN");
            }
            else
            {
                --botY;
                moves.Add("UP");
            }
        }

        while (targetX != botX)
        {
            if (targetX > botX)
            {
                ++botX;
                moves.Add("RIGHT");
            }
            else
            {
                --botX;
                moves.Add("LEFT");
            }
        }
    }

    private static IEnumerable<IEnumerable<T>> Permutate<T>(IEnumerable<T> sequence, int length)
    {
        if (length == 1)
            return sequence.Select(t => new T[] { t });
        return Permutate(sequence, length - 1)
            .SelectMany(t => sequence.Where(e => !t.Contains(e)),
            (t1, t2) => t1.Concat(new T[] { t2 }));
    }

    static void Main(String[] args)
    {
        String temp = Console.ReadLine();
        String[] position = temp.Split(' ');
        int[] pos = new int[2];
        String[] board = new String[5];
        for (int i = 0; i < 5; i++)
        {
            board[i] = Console.ReadLine();
        }
        for (int i = 0; i < 2; i++) pos[i] = Convert.ToInt32(position[i]);
        next_move(pos[0], pos[1], board);
    }
}
