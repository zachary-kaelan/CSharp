using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Jil;
using RestSharp;
using HtmlAgilityPack;
using ZachLib;

namespace ZillowLib
{
    public static class FactFinder
    {
        static FactFinder()
        {
            //CLIENT.AddDefaultHeader("X-Requested-With", "XMLHttpRequest");
        }

        private static readonly CookieContainer COOKIES = new CookieContainer();
        private static readonly RestClient CLIENT = new RestClient("https://factfinder.census.gov/")
        {
            CookieContainer = COOKIES,
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36"
        };

        public static string SearchSuggestion(string searchString) =>
            JSON.DeserializeDynamic(
                CLIENT.Execute(
                    new RestRequest(
                        "rest/communityFactsNav/suggest",
                        Method.GET
                    ).AddOrUpdateParameter(
                        "lookaheadSearchTerms",
                        searchString,
                        ParameterType.QueryString
                    )
                ).Content
            ).SuggestionsList.DimGroups[0].Dims.name;

        private static CFMetaData Search(string searchString, string spotlight) =>
            JSON.Deserialize<CFMetaDataResponse>(
                CLIENT.Execute(
                    new RestRequest(
                        "rest/communityFactsNav/nav",
                        Method.GET
                    ).AddOrUpdateParameters(
                        new Dictionary<string, string>() {
                            { "N", "0" },
                            { "searchTerm", searchString },
                            { "spotlightId", spotlight }
                        }
                    )
                ).Content
            ).CFMetaData;

        private static readonly Lazy<Regex> RGX_SOURCE_YEAR = new Lazy<Regex>(
            () => new Regex(@"(\d{4})", RegexOptions.Compiled)
        );
        public static AreaStats GetStats(string searchString)
        {
            var results = Search(searchString, "ALL");
            if (results.isNotValidGeo || results.displayNoDataAvailableMsg)
                return null;
            var allMeasures = results.measuresAndLinks.allMeasures;
            var dict = allMeasures.ToDictionary(
                m => m.label,
                m =>
                {
                    if (!String.IsNullOrWhiteSpace(m.value))
                        return m.value;
                    else
                    {
                        if (m.list.Count() == 1)
                            return m.list.First().value;
                        else
                        {
                            int index = Array.FindIndex(m.list, s => s.source.label.StartsWith("2012-2016"));
                            string value = null;
                            if (index >= 0)
                                value = m.list[index].value;
                            else
                                value = m.list.GetByMax(
                                    s => Convert.ToInt32(
                                        RGX_SOURCE_YEAR.Value.Match(s.label).Groups[1].Value
                                    )
                                ).value;

                            if (value == "N/A")
                            {
                                var listTemp = m.list.Where(s => s.value != "N/A");
                                if (!listTemp.Any())
                                    return "-1";
                                return listTemp.GetByMax(
                                    s => Convert.ToInt32(
                                        RGX_SOURCE_YEAR.Value.Match(s.label).Groups[1].Value
                                    )
                                ).value;
                            }
                            return value;
                        }
                    }
                } 
            );

            return new AreaStats(dict, searchString);
        }

        public static int GetMedianIncome(string searchString)
        {
            var searchResult = Search(searchString, "INCOME");
            return int.TryParse(
                searchResult.measuresAndLinks.measure.value,
                NumberStyles.Number, CultureInfo.CurrentCulture, out int result
            ) ? result : -1;
        }

        private class CFMetaDataResponse
        {
            public CFMetaData CFMetaData { get; private set; }
        }

        private class CFMetaData
        {
            public string backToBreadcrumbTitle { get; private set; }
            public string breadcrumbTitle { get; private set; }
            public string currentContext { get; private set; }
            public string currentContextURI { get; private set; }
            public string disambiguationContent { get; private set; }
            public bool displayNoDataAvailableMsg { get; private set; }
            public string eusbreadcrumb { get; private set; }
            public bool isNotValidGeo { get; private set; }
            public string leftNavSelection { get; private set; }
            public string measureAndLinksContent { get; private set; }
            public MeasuresAndLinks measuresAndLinks { get; private set; }
            public string geo { get; private set; }
            public string topicId { get; private set; }
        }

        private class MeasuresAndLinks
        {
            public MeasureWithList[] allMeasures { get; protected set; }
            public Measure measure { get; protected set; }
            public Dictionary<string, Source[]> links { get; protected set; }
        }

        private class Measure
        {
            public string label { get; protected set; }
            public string value { get; protected set; }
        }

        private class Source
        {
            public string label { get; protected set; }
            public string url { get; protected set; }
        }

        private class MeasureWithSource : Measure
        {
            public Source source { get; protected set; }
        }

        private class MeasureWithList : Measure
        {
            public MeasureWithSource[] list { get; protected set; }
        }
    }
    
    public class AreaStats
    {
        private const NumberStyles INT_STYLE = NumberStyles.AllowThousands;
        private const NumberStyles DBL_STYLE = NumberStyles.AllowDecimalPoint;
        private static readonly CultureInfo CURRENT_CULTURE = CultureInfo.CurrentCulture;

        public string AreaName { get; set; }
        public int Population { get; set; }
        public double MedianAge { get; set; }
        public double GraduationRate { get; set; }
        public int HousingUnits { get; set; }
        public double MedianIncome { get; set; }
        public int ForeignBorn { get; set; }
        public double PovertyRate { get; set; }
        public int Veterans { get; set; }

        public AreaStats()
        {

        }

        public AreaStats(Dictionary<string, string> dict, string name)
        {
            AreaName = name;
            string str = null;
            Population = dict.TryGetValue("2016 ACS 5-Year Population Estimate", out str) &&
                                int.TryParse(str, INT_STYLE, CURRENT_CULTURE, out int num) ? num : -1;
            MedianAge = dict.TryGetValue("Median Age", out str) &&
                                double.TryParse(str, out double dbl) ? dbl : -1;
            //NumCompanies = dict.TryGetValue("Number of Companies", out str) &&
            //                    int.TryParse(str, INT_STYLE, CURRENT_CULTURE, out num) ? num : -1;
            GraduationRate = dict.TryGetValue("Educational Attainment: Percent high school graduate or higher", out str) &&
                                double.TryParse(str.TrimEnd('%'), out dbl) ? dbl / 100.0 : -1;
            //NumGovernments = dict.TryGetValue("Count of Governments", out str) &&
            //                    int.TryParse(str, INT_STYLE, CURRENT_CULTURE, out num) ? num : -1;
            HousingUnits = dict.TryGetValue("Total housing units", out str) &&
                                int.TryParse(str, INT_STYLE, CURRENT_CULTURE, out num) ? num : -1;
            MedianIncome = dict.TryGetValue("Median Household Income", out str) &&
                                double.TryParse(str, out dbl) ? dbl : -1;
            ForeignBorn = dict.TryGetValue("Foreign Born Population", out str) &&
                                int.TryParse(str, INT_STYLE, CURRENT_CULTURE, out num) ? num : -1;
            PovertyRate = dict.TryGetValue("Individuals below poverty level", out str) &&
                                double.TryParse(str.TrimEnd('%'), out dbl) ? dbl / 100.0 : -1;
            Veterans = dict.TryGetValue("Veterans", out str) &&
                                int.TryParse(str, INT_STYLE, CURRENT_CULTURE, out num) ? num : -1;
        }

        public double[] ToVector()
        {
            double dblPop = Population;
            return new double[]
            {
                Population,
                MedianAge,
                //NumCompanies,
                GraduationRate,
                //NumGovernments,
                HousingUnits,
                MedianIncome,
                ForeignBorn,
                PovertyRate,
                Veterans,
                HousingUnits > 0 ? dblPop / HousingUnits : -1,
                Veterans / dblPop,
                ForeignBorn / dblPop
            };
        }
    }
}
