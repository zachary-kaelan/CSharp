using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DnD
{
    public class Table
    {
        const string basePath = @"E:\Installations\Programming_Frameworks\DnD\";
        const string DMGPath = basePath + "DMG.pdf";
        const string DMG5ePath = basePath + @"DnD 5e Dungeon Masters Guide.pdf";
        const string MatchesPath = basePath + "Matches.txt";

        public static Dictionary<string[], string[]> matches = File.ReadAllLines(MatchesPath)
            .Select(l => l.Split(new string[] { " :=: " }, StringSplitOptions.None))
            .ToDictionary(
                l => l[0].Split(new string[] { ", ", "," }, StringSplitOptions.None),
                l => l[1].Split(new string[] { ", ", "," }, StringSplitOptions.None)
            );
        public string name { get; set; }
        public int index { get; set; }
        public int columnCount { get; set; }
        public string[] columnNames { get; set; }
        public DataTable table { get; set; }

        public Table(int index, string name)
        {
            this.index = index;
            this.name = name;
        }

        public void GetTableFromBounds(List<string> bounds)
        {
            table = new DataTable(name);

            string[] firstRow = bounds[0].Split(' ');
            if (firstRow.Length > columnCount)
            {
                if (firstRow.All(c => Char.IsLetter(c[0]) && Char.IsUpper(c[0])))
                {

                }
            }
        }
    }

    public class Row
    {
        public const string ParanethesesPat = @" ([A-Za-z0-9]+)( \(.+?\))";

        public string[] rowData { get; set; }
        public Row(string row, int columnCount)
        {
            List<string> data = row.Split(' ').ToList();
            if (data.Count > columnCount)
            {
                var match = Regex.Match(row, ParanethesesPat);
                if (match.Success)
                {
                    int index = data.FindIndex(c => c.StartsWith(match.Groups[1].Value));
                    data = row.Replace(match.Value, "").Split(' ').ToList();
                    data.Insert(index, match.Groups[1].Value + match.Groups[2].Value);
                }
            }
        }
    }
    
}
