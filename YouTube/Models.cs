using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YouTube
{
    public enum WebPageType
    {
        BROWSE,
        WATCH
    }

    public struct Page
    {
        public string csn { get; set; }
        public string page { get; set; }
        public Timing timing { get; set; }
        public string url { get; set; }
        public string xsrf_token { get; set; }
        public Response response { get; set; }
    }

    public struct Timing
    {
        public TimingInfo info { get; set; }
    }

    public struct TimingInfo
    {
        public int st { get; set; }
    }

    public struct Response
    {
        public Contents contents { get; set; }
        /*public Dictionary<string, object> header { get; set; }
        public Dictionary<string, object> responseContext { get; set; }
        public Dictionary<string, object> topbar { get; set; }
        public string trackingParams { get; set; }*/
    }

    public struct Contents
    {
        public TwoColumnBrowserResultsRenderer twoColumnBrowserResultsRenderer { get; set; }
    }

    public struct TwoColumnBrowserResultsRenderer
    {
        public Tabs tabs { get; set; }
    }

    public struct Tabs
    {
        public TabRenderer tabRenderer { get; set; }
    }

    public struct TabRenderer
    {
        public TabRendererContent content { get; set; }
        public bool selected { get; set; }
        public string tabIdentifier { get; set; }
        public string trackingParams { get; set; }
    }

    public struct TabRendererContent
    {
        public SectionListRenderer sectionListRenderer { get; set; }
    }

    public struct SectionListRenderer
    {
        public SectionListRendererContents contents { get; set; }
        public string trackingParams { get; set; }
    }

    public struct SectionListRendererContents
    {
        public ItemSectionRenderer itemSectionRenderer { get; set; }
    }

    public struct ItemSectionRenderer
    {
        public VideoRenderer[] contents { get; set; }
        public Continuations continuations { get; set; }
        public string trackingParams { get; set; }
    }

    public struct VideoRenderer
    {
        private static readonly Regex RGX_TITLE = new Regex(
            @"^(?<Title>.+?)by (?<Author>.+?) (?<When>\d (?:year|month|week|day|hour|minute|second)s?) ago (?<Duration>(?:\d+ (?:second|minute|hour)[s, ]*){1,2}) (?<Views>[\d,]+) views$", 
            RegexOptions.Compiled | RegexOptions.Multiline, 
            TimeSpan.FromMilliseconds(100)
        );

        public Thumbnails channelThumbnail { get; set; }
        public SimpleText descriptionSnippet { get; set; }
        public SimpleText lengthText { get; set; }
        public SimpleText shortViewCountText { get; set; }
        public Thumbnails thumbnail { get; set; }
        public SimpleText title { get; set; }
        public ByLineText longBylineText { get; set; }
        public bool showActionMenu { get; set; }
        public string trackingParams { get; set; }
        public string videoId { get; set; }
        public SimpleText viewCountText { get; set; }
        public ThumbnailOverlay thumbnailOverlays { get; set; }

        public VideoListModel Simplify()
        {
            var run = longBylineText.runs.Single();
            return new VideoListModel()
            {
                Channel = new ChannelListModel()
                {
                    ID = run.navigationEndpoint.browseEndpoint["browseId"],
                    Name = run.text,
                    Thumbnail = channelThumbnail.thumbnails.Single(),
                    URL = run.navigationEndpoint.webNavigationEndpointData.url
                },
                Description = descriptionSnippet.simpleText,
                ID = videoId,
                Length = TimeSpan.Parse(lengthText.simpleText),
                PercentDurationWatched = thumbnailOverlays.thumbnailOverlayResumePlaybackRenderer.percentDurationWatched,
                Title = title.simpleText,
                Views = Convert.ToInt32(viewCountText.simpleText.Replace(",", "").Trim()),
                When = RGX_TITLE.Match(title.accessibility.accessibilityData["label"]).Groups["When"].Value
            };
        }
    }

    public struct ByLineText
    {
        public Run[] runs { get; set; }
    }

    public struct Run
    { 
        public NavigationEndpoint navigationEndpoint { get; set; }
        public string text { get; set; }
    }

    public struct NavigationEndpoint
    {
        public string clickTrackingParams { get; set; }
        public Dictionary<string, string> browseEndpoint { get; set; }
        public Dictionary<string, string> watchEndpoint { get; set; }
        public WebNavigationEndpoint webNavigationEndpointData { get; set; }
    }

    public struct WebNavigationEndpoint
    {
        public string url { get; set; }
        public WebPageType webPageType { get; set; }
    }

    public struct SimpleText
    {
        public Accessibility accessibility { get; set; }
        public string simpleText { get; set; }
    }

    public struct Accessibility
    {
        public Dictionary<string, string> accessibilityData { get; set; }
    }

    public struct ThumbnailOverlay
    {
        public ThumbnailOverlayResumePlaybackRenderer thumbnailOverlayResumePlaybackRenderer { get; set; }
    }

    public struct ThumbnailOverlayResumePlaybackRenderer
    {
        public int percentDurationWatched { get; set; }
    }

    public struct Thumbnails
    {
        public Thumbnail[] thumbnails { get; set; }
    }

    public struct Thumbnail
    {
        public int height { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

    public struct Continuations
    {
        public NextContinuationData nextContinuationData { get; set; }
    }

    public struct NextContinuationData
    {
        public string clickTrackingParams { get; set; }
        public string continuation { get; set; }
    }
}
