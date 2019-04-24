using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ZachLib;
using RestSharp;
using RestSharp.Extensions;
using Newtonsoft.Json;

namespace FourChanLib
{
    public class Board
    {
        public string URLName { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }

        public bool IsWorkSafe { get; private set; }
        public bool IsArchived { get; private set; }
        public int NumThreadsPerPage { get; private set; }
        public int NumPages { get; private set; }
        public Limits Limits { get; private set; }
        public Cooldowns Cooldowns { get; private set; }

        private Dictionary<int, Thread> Threads = null;
        private Stopwatch LastUpdated = new Stopwatch();

        public Board(string urlname) : this()
        {
            URLName = urlname;
        }

        public Board()
        {
            URLName = null;
            Title = null;
            Description = null;
            IsWorkSafe = false;
            IsArchived = false;
            NumThreadsPerPage = 0;
            NumPages = 0;
            Limits = new Limits();
            Cooldowns = new Cooldowns();
        }

        internal Board(BoardTemp board)
        {
            URLName = board.board;
            Title = board.title;
            Description = board.meta_description;

            IsWorkSafe = board.ws_board == 1;
            IsArchived = board.is_archived == 1;
            NumThreadsPerPage = board.per_page;
            NumPages = board.pages;

            Limits = new Limits(board);
            Cooldowns = new Cooldowns(board);
        }

        public Thread[] GetCatalog()
        {
            LastUpdated.Stop();
            if (Threads == null)
            {
                Threads = FourChan.MakeRequest<CatalogPageTemp[]>(URLName + "/catalog.json").SelectMany(
                    p => p.threads
                ).ToDictionary(
                    t => t.no,
                    t => new Thread(t)
                );
                LastUpdated.Restart();
            }
            else if (LastUpdated.Elapsed.TotalSeconds >= 60.0)
            {
                List<Thread> threadsTemp = new List<Thread>();
                var threadids = FourChan.MakeRequest<ThreadsPageTemp[]>(URLName + "/threads.json").SelectMany(
                    p => p.threads.Select(t => t.no)
                );

                Threads = threadids.ToDictionary(
                    i => i,
                    i => Threads.TryGetValue(i, out Thread t) ? t : 
                        new Thread(
                            FourChan.MakeRequest<ThreadTemp>(
                                "b/thread/" + i.ToString() + ".json"
                            ), true
                        )
                );
                LastUpdated.Restart();
            }
            else
                LastUpdated.Start();
            return Threads.Values.ToArray();
        }
    }

    public struct Limits
    {
        public int Filesize { get; private set; }
        public int WebmFilesize { get; private set; }
        public int CommentLength { get; private set; }
        public int WebmDuration { get; private set; }
        public int BumpsPerThread { get; private set; }
        public int ImagesPerThread { get; private set; }

        internal Limits(BoardTemp board)
        {
            BumpsPerThread = board.bump_limit;
            CommentLength = board.max_comment_chars;
            Filesize = board.max_filesize;
            ImagesPerThread = board.image_limit;
            WebmDuration = board.max_webm_duration;
            WebmFilesize = board.max_webm_filesize;
        }
    }

    public struct Cooldowns
    {
        // In seconds
        public int Threads { get; private set; }
        public int Replies { get; private set; }
        public int Images { get; private set; }

        internal Cooldowns(BoardTemp board)
        {
            Threads = board.cooldowns["threads"];
            Replies = board.cooldowns["replies"];
            Images = board.cooldowns["images"];
        }
    }

    public interface IPost
    {
        int PostID { get; }
        int ReplyTo { get; }
        string Now { get; }
        DateTime CreatedAt { get; }
        string Subject { get; }
        string Comment { get; }

        PostFile File { get; }
        Author Author { get; }
    }

    public struct PostFile
    {
        public long TimeFilename { get; private set; }
        public string OriginalFilename { get; private set; }
        public string FileExtension { get; private set; }
        public int FileSize { get; private set; }
        public string MD5 { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int ThumbnailWidth { get; private set; }
        public int ThumbnailHeight { get; private set; }
        public bool IsDeleted { get; private set; }
        public bool IsSpoiler { get; private set; }
        public int CustomSpoiler { get; private set; }

        internal PostFile(FileTemp file) : this()
        {
            TimeFilename = file.tim;
            OriginalFilename = file.filename;
            FileExtension = file.ext;
            FileSize = file.fsize;
            MD5 = file.md5;
            Width = file.w;
            Height = file.h;
            ThumbnailWidth = file.tn_w;
            ThumbnailHeight = file.tn_h;
            IsDeleted = file.filedeleted == 1;
            IsSpoiler = file.spoiler == 1;
            CustomSpoiler = file.custom_spoiler;
        }

        public void DownloadTo(string folder)
        {
            FourChan.client.DownloadData(
                new RestRequest(
                    "b/" + TimeFilename.ToString() + FileExtension, Method.GET
                )
            ).SaveAs(
                folder + (folder.EndsWith(@"\") ? @"\" : "") + OriginalFilename + FileExtension
            );
        }
    }

    public struct Author
    {
        public string Name { get; private set; }
        public string TripCode { get; private set; }
        public string ID { get; private set; }
        public string Capcode { get; private set;  }
        public string CountryCode { get; private set; }
        public string CountryName { get; private set; }
        public int? Year4PassBought { get; private set; }

        internal Author(AuthorTemp author) : this()
        {
            Year4PassBought = author.since4pass.HasValue && author.since4pass.Value >= 2000 ? author.since4pass : null;
            Name = author.name;
            TripCode = author.trip;
            ID = author.id;
            Capcode = author.capcode;
            CountryCode = author.country;
            CountryName = author.country_name;
        }
    }

    public class Thread : Post
    {
        public bool IsStickied { get; private set; }
        public bool IsClosed { get; private set; }
        public bool IsArchived { get; private set; }
        public DateTime? ArchivedOn { get; private set; }
        public int Replies { get; private set; }
        public int Images { get; private set; }

        public int OmittedReplies { get; private set; }
        public int OmittedImages { get; private set; }
        public Dictionary<string, int[]> CapcodeReplies { get; private set; }
        public DateTime LastModified { get; private set; }
        public string Tag { get; private set; }
        public string SemanticURL { get; private set; }
        private Post[] Posts { get; set; }
        private string IfModifiedSince { get; set; }
        internal Stopwatch LastUpdated = new Stopwatch();

        internal Thread(ThreadTemp thread, bool startTimer = false) : base ((IPostWithFileTemp)thread)
        {
            ArchivedOn = null;
            IsArchived = false;

            IsStickied = thread.sticky == 1;
            IsClosed = thread.closed == 1;
            if (thread.archived == 1)
            {
                IsArchived = true;
                ArchivedOn = Utils.ConvertUnixTimestamp(thread.archived_on);
            }
            Replies = thread.replies;
            Images = thread.images;
            OmittedReplies = thread.omitted_posts;
            OmittedImages = thread.omitted_images;
            CapcodeReplies = (thread.capcode_replies != null && thread.capcode_replies.Any()) ?
                thread.capcode_replies : 
                new Dictionary<string, int[]>();
            LastModified = Utils.ConvertUnixTimestamp(thread.last_modified);
            Tag = thread.tag;
            SemanticURL = thread.semantic_url;

            Posts = thread.last_replies.Select(p => new Post(p)).ToArray();
            if (startTimer)
                LastUpdated.Restart();
        }

        public Post[] GetReplies()
        {
            LastUpdated.Stop();
            if (LastUpdated.ElapsedMilliseconds == 0)
            {
                LastUpdated.Start();
                Posts = FourChan.MakeRequest<KeyValuePair<string, PostWithFileTemp[]>>(
                    "b/thread/" + PostID.ToString() + ".json"
                ).Value.Select(
                    p => new Post(p)
                ).ToArray();
                IfModifiedSince = DateTime.Now.ToString("R");

            }
            else if (LastUpdated.Elapsed.TotalSeconds >= 15.0)
            {
                if (FourChan.TryMakeRequest<KeyValuePair<string, PostWithFileTemp[]>>("b/thread" + PostID.ToString() + ".json", IfModifiedSince, out KeyValuePair<string, PostWithFileTemp[]> content))
                {
                    Posts = content.Value.Skip(Posts.Length).Select(p => new Post(p)).ToArray();
                    LastUpdated.Restart();
                    IfModifiedSince = DateTime.Now.ToString("R");
                }
                else
                    LastUpdated.Start();
                    
            }
            else
                LastUpdated.Start();

            return Posts;
        }
    }

    public class Post : IPost
    {
        public int PostID { get; private set; }
        public int ReplyTo { get; private set; }
        public string Now { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public string Subject { get; private set; }
        public string Comment { get; private set; }
        public PostFile File { get; private set; }
        public Author Author { get; private set; }

        internal Post(IPostWithFileTemp post)
        {
            PostID = post.no;
            ReplyTo = post.resto;
            Now = post.now;
            CreatedAt = Utils.ConvertUnixTimestamp(post.time);

            Subject = post.subject;
            Comment = HttpUtility.HtmlDecode(ZachRGX.HTML_TAGS.Replace(post.com, ""));
            File = new PostFile((FileTemp)post);
            Author = new Author((AuthorTemp)post);
        }
    }
}
