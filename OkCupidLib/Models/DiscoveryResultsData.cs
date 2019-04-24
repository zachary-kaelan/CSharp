using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidLib.Models
{
    public enum DiscoverySearchSection
    {
        match_search_pets,
        snapshots,
        match_search_children,
        question_comments,
        match_search_smoking,
        instagram,
        match_search_drinking,
        important_questions,
        match_search_diet
    }

    public enum DiscoveryResultsLayout
    {
        PictureTitleSubtitle
    }

    internal struct DiscoveryResultsQuery
    {
        public DiscoverySearchSection section { get; protected set; }
        public int limit { get; protected set; }

        public DiscoveryResultsQuery(DiscoverySearchSection section, int limit)
        {
            this.section = section;
            this.limit = limit;
        }
    }

    public class DiscoveryResultsData
    {
        public object section_header { get; protected set; }
        public DiscoverySearchSection section_id { get; protected set; }
        public DiscoveryResultsMetadata metadata { get; protected set; }
        public string title { get; protected set; }
        public ComponentMetadata<ImportantQuestionsMetadata> header_component { get; protected set; }
        public object footer_component { get; protected set; }
        public DiscoveryResults results { get; protected set; }

        public struct DiscoveryResultsMetadata
        {
            public DiscoverySearchSection section { get; protected set; }
        }
    }

    public class DiscoveryResults
    {
        public DiscoveryResultsLayout layout { get; protected set; }
        public DiscoveryRows rows { get; protected set; }
    }

    public class ComponentMetadata<T>
    {
        public T metadata { get; protected set; }
    }

    public class ImportantQuestionsMetadata
    {
        public Question[] questions { get; protected set; }
    }
}
