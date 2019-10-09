using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZachLib;

namespace OkCupidLib.Models
{
    [Flags]
    public enum SearchFields
    {
        Default = 0,
        wiw = 1,
        userinfo = 2,
        thumbs = 4,
        personality_traits = 8,
        percentages = 16,
        online = 32,
        nudge,
        match_genres,
        location = 64,
        linked_account,
        likes = 128,
        last_contacts = 256,
        hidden,
        first_message = 512,
        essays = 1024,
        detail_tags = 2048,
        details = 4096,
        bookmarked = 8192,
        blocked = 16384,
        interests = 32768
    }

    public class FieldParam
    {
        public int limit { get; protected set; }
        public string data { get; protected set; }

        public FieldParam(int limit)
        {
            this.limit = limit;
        }

        public FieldParam(string data)
        {
            this.data = data;
        }

        public FieldParam(int limit, string data)
        {
            this.limit = limit;
            this.data = data;
        }

        public override string ToString()
        {
            return (limit > 0 ? ".limit(" + limit.ToString() + ")" : "") +
                (!String.IsNullOrWhiteSpace(data) ? "{" + data + "}" : "");
        }
    }

    public class Fields
    {
        public SearchFields fields { get; protected set; }
        public IDictionary<SearchFields, FieldParam> fieldParams { get; protected set; }

        public Fields(SearchFields fields, IDictionary<SearchFields, FieldParam> fieldParams)
        {
            this.fields = fields;
            this.fieldParams = fieldParams;
        }

        public Fields(SearchFields fields, IEnumerable<KeyValuePair<SearchFields, FieldParam>> fieldParams) : this(fields, fieldParams.ToDictionary()) { }

        public Fields(SearchFields fields, params KeyValuePair<SearchFields, FieldParam>[] fieldParams) : this(fields, fieldParams.ToDictionary()) { }

        public override string ToString()
        {
            if (fields != SearchFields.Default)
            {
                var fieldFlags = fields.GetFlags();
                string str = "";
                if (fieldParams != null && fieldParams.Count > 0)
                {
                    fieldFlags = fieldFlags.Except(fieldParams.Keys);
                    foreach (var fieldParam in fieldParams)
                    {
                        str += fieldParam.Key.ToString() + fieldParam.Value.ToString();
                    }
                }
                return str + String.Join(",", fieldFlags);
            }
            else
                return "userinfo,thumbs,percentages,likes,last_contacts,online";
        }
    }
}
