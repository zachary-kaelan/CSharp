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
        public int max_filesize { get; private set; }
        public int max_webm_filesize { get; private set; }
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
        int no { get; }
        int last_modified { get; }
    }

    internal interface AuthorTemp
    {
        string name { get; }
        string capcode { get; } // For mods and such
        int? since4pass { get; }
        string id { get; }
        string trip { get; }
        string country { get; }
        string country_name { get; }
    }

    internal interface PostTemp : IListedPostTemp, AuthorTemp
    {
        string now { get; }
        int time { get; }
        int resto { get; }
        string com { get; }

        string subject { get; }
    }

    internal interface FileTemp
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
        int fsize { get; }
    }
    
    internal interface IPostWithFileTemp :PostTemp, FileTemp
    {

    }

    public struct PostWithFileTemp : IPostWithFileTemp
    {
        public int filedeleted { get; }
        public int spoiler { get; }
        public int custom_spoiler { get; }

        public string filename { get; }
        public string ext { get; }
        public int w { get; }
        public int h { get; }
        public int tn_w { get; }
        public int tn_h { get; }
        public long tim { get; }
        public string md5 { get; }
        public int fsize { get; }

        public string now { get; }
        public int time { get; }
        public int resto { get; }
        public string com { get; }

        public string subject { get; }

        public string name { get; }
        public string capcode { get; } // For mods and such
        public int? since4pass { get; }
        public string id { get; }
        public string trip { get; }
        public string country { get; }
        public string country_name
        {
            get;
        }

        public int no { get; }
        public int last_modified { get; }
    }

    public struct ThreadTemp : IPostWithFileTemp
    {
        public int sticky { get; }
        public int closed { get; }
        public int archived { get; }
        public int archived_on { get; }

        public int bumplimit { get; }
        public int imagelimit { get; }
        public string semantic_url { get; }
        public string tag { get; }

        public int replies { get; }
        public int images { get; }
        public int omitted_posts { get; }
        public int omitted_images { get; }
        public int unique_ips { get; }
        public int filedeleted { get; }
        public int spoiler { get; }
        public int custom_spoiler { get; }

        public string filename { get; }
        public string ext { get; }
        public int w { get; }
        public int h { get; }
        public int tn_w { get; }
        public int tn_h { get; }
        public long tim { get; }
        public string md5 { get; }
        public int fsize { get; }

        public string now { get; }
        public int time { get; }
        public int resto { get; }
        public string com { get; }

        public string subject { get; }

        public string name { get; }
        public string capcode { get; } // For mods and such
        public int? since4pass { get; }
        public string id { get; }
        public string trip { get; }
        public string country { get; }
        public string country_name { get; }

        public int no { get; }
        public int last_modified { get; }

        public Dictionary<string, int[]> capcode_replies { get; }

        public PostWithFileTemp[] last_replies { get; }
    }

    internal struct CatalogPageTemp
    {
        public int page { get; set; }
        public IEnumerable<ThreadTemp> threads { get; set; }
    }

    internal struct ThreadsPageTemp
    {
        public int page { get; set; }
        public IEnumerable<IListedPostTemp> threads { get; set; }
    }
}
