using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZachLib;

namespace OkCupidLib.Models
{
    public enum MatchSearchOrdering
    {
        SPECIAL_BLEND,
        MATCH,
        MATCH_AND_DISTANCE,
        LOGIN
    }

    public enum BodyType
    {
        thin,
        fit,
        jacked,
        curvy,
        full_figured,
        average,
        a_little_extra,
        overweight
    }

    public enum ChildrenParams
    {
        wants_kids,
        might_want,
        doesnt_want,
        has_one_or_more,
        doesnt_have
    }

    public enum DrinkingFrequency
    {
        not_at_all,
        socially,
        rarely,
        very_often,
        often,
        desperately
    }

    public enum DrugsFrequency
    {
        never,
        sometimes,
        often
    }

    public enum SmokingFrequency
    {
        no,
        sometimes,
        when_drinking,
        trying_to_quit,
        yes
    }

    public enum Education
    {
        high_school,
        two_year_college,
        post_grad,
        college_university
    }

    public enum Ethnicity
    {
        asian,
        black,
        hispanic_latin,
        indian,
        middle_eastern,
        native_american,
        pacific_islander,
        white,
        other
    }

    public enum Religion
    {
        agnosticism,
        atheism,
        buddhism,
        catholicism,
        christianity,
        hinduism,
        judaism,
        islam,
        sikh,
        other
    }

    public enum Tags
    {
        availability,
        minimum_height,
        maximum_height,
        bodytype,
        languages,
        speaks_my_language,
        ethnicity,
        religion,
        monogamy,
        looking_for,
        personality_filters,
        smoking,
        drinking,
        drugs,
        questions,
        interest_ids,
        education,
        children,
        cats,
        dogs
    }

    public enum Attractiveness
    {
        Average = 4000,
        Attractive = 6000,
        Very_Attractive = 8000,
        Hot = 10000
    }

    public enum Monogamy
    {
        yes,
        no,
        unknown
    }

    public class MatchSearchQuery
    {
        public MatchSearchOrdering order_by { get; set; }
        public int[] gentation { get; set; }
        public int[] gender_tags { get; set; }
        public int orientation_tags { get; set; }
        public int minimum_age { get; set; }
        public int maximum_age { get; set; }

        public int locid { get; set; }
        public int radius { get; set; }
        public string lquery { get; set; }
        public LocationInfo location { get; set; }
        public byte locatedAnywhere { get; set; }

        public long last_login { get; set; }
        public IWant i_want { get; set; }
        public IWant they_want { get; set; }
        public int? minimum_height { get; set; }
        public int? maximum_height { get; set; }
        public Attractiveness? minimum_attractiveness { get; set; }
        public Attractiveness? maximum_attractiveness { get; set; }
        public BodyType[] bodytype { get; set; }

        public int languages { get; set; }
        public bool speaks_my_language { get; set; }
        public Ethnicity[] ethnicity { get; set; }
        public Religion[] religion { get; set; }

        public string availability { get; set; }
        public Monogamy monogamy { get; set; }
        public LookingFor[] looking_for { get; set; }

        public Dictionary<PersonalityTrait, Comparative> personality_filters { get; set; }

        public SmokingFrequency[] smoking { get; set; }
        public DrinkingFrequency[] drinking { get; set; }
        public DrugsFrequency[] drugs { get; set; }

        public int[] questions { get; set; }
        public int[] answers { get; set; }

        public string[] interest_ids { get; set; }
        public Education[] education { get; set; }
        public ChildrenParams[] children{ get; set; }
        public string[] cats { get; set; }
        public string[] dogs { get; set; }

        public Tags[] tagOrder { get; set; }
        public bool save_search { get; set; }
        public int limit { get; set; }
        public string fields { get; set; }
        internal string after { get; set; }

        private static readonly long ONE_YEAR_AGO = DateTime.Now.AddYears(-1).ToUnixTimestamp();
        public MatchSearchQuery()
        {
            order_by = MatchSearchOrdering.SPECIAL_BLEND;
            gentation = new int[] { 34 };
            gender_tags = null;
            orientation_tags = 0;
            /*if (myAge == -1)
            {
                minimum_age = 18;
                maximum_age = 99;
            }
            else
            {
                minimum_age = Math.Max(18, myAge - 5);
                maximum_age = Math.Max(99, myAge + 11);
            }*/

            location = LocationInfo.DEFAULT.Value;
            locatedAnywhere = 0;

            last_login = ONE_YEAR_AGO;
            minimum_age = 18;
            maximum_age = 38;
            save_search = true;
            limit = 18;
            fields = "userinfo,thumbs,percentages,likes,last_contacts,online";

            i_want = IWant.women;
            they_want = IWant.men;
            minimum_height = null;
            maximum_height = null;
            minimum_attractiveness = null;
            maximum_attractiveness = null;
            bodytype = Array.Empty<BodyType>();

            languages = 0;
            speaks_my_language = false;
            ethnicity = Array.Empty<Ethnicity>();
            religion = Array.Empty<Religion>();

            availability = "any";
            monogamy = Monogamy.unknown;
            looking_for = Array.Empty<LookingFor>();

            personality_filters = new Dictionary<PersonalityTrait, Comparative>();

            smoking = Array.Empty<SmokingFrequency>();
            drinking = Array.Empty<DrinkingFrequency>();
            drugs = Array.Empty<DrugsFrequency>();

            questions = Array.Empty<int>();
            answers = Array.Empty<int>();

            interest_ids = Array.Empty<string>();
            education = Array.Empty<Education>();
            children = Array.Empty<ChildrenParams>();
            cats = Array.Empty<string>();
            dogs = Array.Empty<string>();
        }

        public MatchSearchQuery(Tags[] tagOrder, int limit, Fields fields) : this()
        {
            if (tagOrder == null || tagOrder.Length == 0)
                this.tagOrder = new Tags[]
                {
                    Tags.availability,
                    Tags.minimum_height,
                    Tags.maximum_height,
                    Tags.bodytype,
                    Tags.languages,
                    Tags.speaks_my_language,
                    Tags.ethnicity,
                    Tags.religion,
                    Tags.monogamy,
                    Tags.looking_for,
                    Tags.personality_filters,
                    Tags.smoking,
                    Tags.drinking,
                    Tags.drugs,
                    Tags.education,
                    Tags.children,
                    Tags.cats,
                    Tags.dogs
                };

            this.fields = fields.ToString();
            this.limit = limit;
        }

        public void AddQuestionsAnswers(IDictionary<int, int> ids)
        {
            questions = ids.Keys.ToArray();
            answers = ids.Values.ToArray();
        }

        public void AddQuestionsAnswers(IEnumerable<KeyValuePair<int, int>> ids) => AddQuestionsAnswers(ids.ToDictionary());
    }
}
