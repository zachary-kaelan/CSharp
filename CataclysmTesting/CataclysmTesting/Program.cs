using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using HtmlAgilityPack;
using RestSharp;
using ZachLib;

namespace CataclysmTesting
{
    class Program
    {
        public const string PATH = @"E:\Programming\Cataclysm DDA\";

        static void Main(string[] args)
        {
            GetDetergents();
        }

        private static void GetDetergents()
        {
            string[] forbidden = new string[] { "&reg;", "&trade;", "&nbsp;" };
            List<Detergent> detergents = new List<Detergent>();
            List<Ingredient> ingredients = new List<Ingredient>();
            RestClient ingredientClient = new RestClient("https://churchdwight.com");
            var links = Utils.LoadDictionary(PATH + "Detergents.txt", " -:- ");
            SortedSet<string> allIngredients = new SortedSet<string>();

            foreach (var link in links)
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(ingredientClient.Execute(new RestRequest(link.Value)).Content);
                var tableNode = doc.DocumentNode.SelectNodes("/html/body/main/div/article/div[2]/div/table[3]/tr");
                var detergentIngredients = tableNode.Skip(1).Select(
                    n => n.SelectSingleNode("./td/p/span").InnerText.Replace('\n', ' ').Trim()
                ).Where(
                    i => !forbidden.Any(s => i.Contains(s))
                ).OrderBy();
                allIngredients.UnionWith(detergentIngredients);
                detergents.Add(
                    new Detergent()
                    {
                        Name = link.Key.Trim(),
                        Ingredients = detergentIngredients.ToArray()
                    }
                );
            }
            
            foreach(var ingredient in allIngredients)
            {
                Console.WriteLine(ingredient);
                ingredients.Add(
                    new Ingredient()
                    {
                        Name = ingredient,
                        Detergents = detergents.Where(d => Array.BinarySearch(d.Ingredients, ingredient) >= 0).Select(d => d.Name).ToArray()
                    }
                );
            }

            ingredients.SaveAs(PATH + "DetergentsByIngredient.txt");
            detergents.SaveAs(PATH + "IngredientsByDetergent.txt");
            Console.ReadLine();
        }

        private static void MakeMarkdownTable()
        {
            Regex symbols = new Regex(@"[^A-Za-z0-9\s\/ ]");

            List<GithubCategory> languages = new List<GithubCategory>();
            List<GithubCategory> issueTypes = new List<GithubCategory>();
            List<GithubCategory> gameplayCategories = new List<GithubCategory>();
            List<GithubCategory> flags = new List<GithubCategory>();
            var categories = Utils.LoadCSV<GithubCategory>(@"E:\Programming\Cataclysm DDA\Github Categories.csv");
            foreach (var category in categories)
            {
                if (category.Name[0] == '[')
                    languages.Add(category);
                else if (category.Name[0] == '<')
                    issueTypes.Add(category);
                else if (Char.IsLower(category.Name[0]) || symbols.IsMatch(category.Name) || !Regex.IsMatch(category.Name, "[a-z]"))
                    flags.Add(category);
                else
                    gameplayCategories.Add(category);
            }

            using (StreamWriter writer = new StreamWriter(@"E:\Programming\Cataclysm DDA\Github Category Tables.txt"))
            {
                foreach (
                    var metaCategory in new List<GithubCategory>[] {
                        languages,
                        issueTypes,
                        gameplayCategories,
                        flags
                    }
                )
                {
                    var links = metaCategory.ToDictionary(
                        c => String.Format("[{0}]({1})", c.Name, c.Link),
                        c => c.Desc
                    );
                    int maxLinkLength = links.Max(l => l.Key.Length) + 1;
                    if (maxLinkLength % 4 != 0)
                        maxLinkLength += 4 - (maxLinkLength % 4);
                    int maxDescLength = links.Max(l => l.Value.Length) + 1;
                    if (maxDescLength % 4 != 0)
                        maxDescLength += 4 - (maxDescLength % 4);

                    Console.WriteLine("NumCategories: {0}", links.Count);
                    Console.WriteLine("MaxLinkLength: {0}", maxLinkLength);
                    Console.WriteLine("MaxDescLength: {0}", maxDescLength);
                    Console.WriteLine();

                    writer.WriteLine("| Tag" + new string(' ', maxLinkLength - 3) + "| Description");
                    writer.WriteLine("|---" + new string(' ', maxLinkLength - 2) + "|---");
                    foreach (var link in links)
                    {
                        writer.WriteLine("| " + link.Key + new string(' ', maxLinkLength - link.Key.Length) + "| " + link.Value);
                    }
                    writer.WriteLine();
                }
            }
            Console.ReadLine();
        }
    }

    public class GithubCategory
    {
        public string Link { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
    }

    public class Detergent
    {
        public string Name { get; set; }
        public string[] Ingredients { get; set; }
    }

    public class Ingredient
    {
        public string Name { get; set; }
        public string[] Detergents { get; set; }
    }
}
