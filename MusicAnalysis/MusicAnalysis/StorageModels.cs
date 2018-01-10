using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicAnalysis
{
    public interface ObjectModel
    {
        string Name { get; set; }
        string ID { get; set; }
        SpotifyType Type { get; }
        int Popularity { get; set; }
    }

    public struct AlbumModel : ObjectModel
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public SpotifyType Type => SpotifyType.album;
        public int Popularity { get; set; }

        public Album AlbumType { get; set; }
        public IEnumerable<string> Artists { get; set; }
        public IEnumerable<string> Genres { get; set; }

        public Image Image { get; set; }
        public string Label { get; set; }
        public DateTime ReleaseDate { get; set; }
        public IEnumerable<string> Tracks { get; set; }
    }

    public struct ArtistModel : ObjectModel
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public SpotifyType Type => SpotifyType.artist;
        public int Popularity { get; set; }

        public int Followers { get; set; }
        public IEnumerable<string> Genres { get; set; }
        public Image Image { get; set; }
    }

    public struct TrackModel : ObjectModel
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public SpotifyType Type => SpotifyType.track;
        public int Popularity { get; set; }

        public string Album { get; set; }
        public IEnumerable<string> Artists { get; set; }
        public int Duration { get; set; }
        public bool Explicit { get; set; }
        public string Preview { get; set; }
    }
}
