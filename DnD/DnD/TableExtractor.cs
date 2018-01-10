using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DnD
{
    class TableExtractor
    {
        public static Dictionary<string[], string[]> matches = File.ReadAllLines(MatchesPath)
            .Select(l => l.Split(new string[] { " :=: " }, StringSplitOptions.None))
            .ToDictionary(
                l => l[0].Split(new string[] { ", ", "," }, StringSplitOptions.None),
                l => l[1].Split(new string[] { ", ", "," }, StringSplitOptions.None)
            );

        const string basePath = @"E:\Installations\Programming_Frameworks\DnD\";
        const string DMGPath = basePath + "DMG.pdf";
        const string MatchesPath = basePath + "Matches.txt";

        const string TableNamePat = @"^Table (\d.\d: [A-Za-z ]+?)$";

        public List<Table> tables { get; set; }
        public List<string> page { get; set; }

        public TableExtractor(string page)
        {
            this.page = page.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            this.tables = 
                this.page.FindAll(
                    l => Regex.IsMatch(l, TableNamePat)
                ).Select(
                    (m, i) => new Table(i, m)
                ).ToList();
        }

        private void GetTableBounds()
        {
            int lastIndex = 0;
            foreach(Table table in tables)
            {
                page.RemoveRange(0, (table.index - lastIndex) + 1);
                lastIndex = table.index;
                table.columnNames = page[0].Split(' ');
                table.columnCount = table.columnNames.Length;

                List<string> tableRange = page.TakeWhile(
                    l => !l.StartsWith("Table ") && l.Split(' ').Length >= table.columnCount
                ).ToList();

                tableRange.RemoveRange(0, 2);
                table.GetTableFromBounds(tableRange);
            }
        }
    }
}
