using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourChanLib
{
    internal struct BoardTemp
    {
        public string board { get; private set; }
        public string title { get; private set; }
        public int ws_board { get; private set; }
        public int per_page { get; private set; }
        public int pages { get; private set; }
        public long max_filesize { get; private set; }
        public long max_webm_filesize { get; private set; }
        public int max_comment_chars { get; private set; }
        public int max_webm_duration { get; private set; }
        public int bump_limit { get; private set; }
        public int image_limit { get; private set; }
        public Dictionary<string, int> cooldowns { get; private set; }
        public string meta_description { get; private set; }
        public int is_archived { get; private set; }

        public Dictionary<int, IEnumerable<IListedPostTemp>> GetPages()
        {
            return FourChan.MakeRequest<IEnumerable<ThreadsPage<IListedPostTemp>>>(board + "/threads.json").ToDictionary(
                p => p.page,
                p => p.threads
            );
        }

        // For /b/, and potentially some other boards, there is no archive.
        public IEnumerable<long> GetArchived()
        {
            try
            {
                return FourChan.MakeRequest<IEnumerable<long>>(board + "/archive.json");
            }
            catch
            {
                return Enumerable.Empty<long>();
            }
        }

        public Dictionary<int, IEnumerable<ThreadTemp>> GetCatalog()
        {
            return FourChan.MakeRequest<IEnumerable<ThreadsPage<ThreadTemp>>>(board + "/catalog.json").ToDictionary(
                p => p.page,
                p => p.threads
            );
        }

        internal struct ThreadsPage<T>
        {
            public int page { get; private set; }
            public IEnumerable<T> threads { get; private set; }
        }
    }

    internal interface IListedPostTemp
    {
        long no { get; }
        long last_modified { get; }
    }

    internal interface PostTemp : IListedPostTemp
    {
        int since4pass { get; }
        string now { get; }
        long time { get; }
        long resto { get; }

        string name { get; }
        string com { get; }
        string capcode { get; } // For mods and such
    }

    internal interface PostWithFileTemp : PostTemp
    {
        int filedeleted { get; }
        int spoiler { get; }
        int custom_spoiler { get; }

        string filename { get; }
        string ext { get; }
        int w { get; }
        int h { get; }
        int tn_w { get; }
        int tn_h { get; }
        long tim { get; }
        string md5 { get; }
        long fsize { get; }
    }

    internal interface ThreadTemp : PostWithFileTemp
    {
        int sticky { get; }
        int closed { get; }
        int archived { get; }
        int archived_on { get; }

        int bumplimit { get; }
        int imagelimit { get; }
        string semantic_url { get; }
        string tag { get; }

        int replies { get; }
        int images { get; }
        int omitted_posts { get; }
        int omitted_images { get; }
        int unique_ips { get; }

        string id { get; }
        string trip { get; }
        string country { get; }
        string country_name { get; }
        string subject { get; }
        
        Dictionary<string, IEnumerable<long>> capcode_replies { get; }
    }


}
