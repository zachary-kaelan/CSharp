using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Jil;

namespace MusicAnalysis
{
    public enum SpotifyType
    {
        album,
        artist,
        playlist,
        track
    }

    public enum AlbumType
    {
        album,
        single,
        compilation
    }

    public class SpotifyObject
    {
        public Dictionary<string, string> external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public SpotifyType type { get; set; }
        public string uri { get; set; }
    }

    public class TrackSimple : SpotifyObject
    {
        public IEnumerable<SpotifyObject> artists { get; set; }
        public IEnumerable<string> available_markets { get; set; }
        public int duration_ms { get; set; }
        [JilDirective(Name = "explicit")]
        public bool is_explicit { get; set; }
        public bool is_playable { get; set; }
        public string preview_url { get; set; }
        public int track_number { get; set; }
    }

    public class Track : TrackSimple
    {
        public AlbumSimple album { get; set; }
        public Dictionary<string, string> external_ids { get; set; }
        public int popularity { get; set; }
    }

    public class AlbumSimple : SpotifyObject
    {
        public IEnumerable<SpotifyObject> artists { get; set; }
        public IEnumerable<string> available_markets { get; set; }
        public AlbumType album_type { get; set; }
        public IEnumerable<Image> images { get; set; }
    }

    public class Album : AlbumSimple
    {
        public Dictionary<string, string> external_ids { get; set; }
        public int popularity { get; set; }
        public IEnumerable<Copyright> copyrights { get; set; }
        public IEnumerable<string> genres { get; set; }
        public string label { get; set; }
        public string release_date { get; set; }
        public string release_date_precision { get; set; }
        public Pager<TrackSimple> tracks { get; set; }
    }

    public class Artist : SpotifyObject
    {
        public Followers followers { get; set; }
        public IEnumerable<string> genres { get; set; }
        public IEnumerable<Image> images { get; set; }
        public int popularity { get; set; }
    }

    public class AudioFeatures : SpotifyObject
    {
        /// <summary>
        /// Confidence measure of whether the track is acoustic.
        /// </summary>
        public float acousticness { get; set; }

        /// <summary>
        /// HTTP URL to access the full audio analysis of this track.
        /// <para>Access token required.</para>
        /// </summary>
        public string analysis_url { get; set; }

        /// <summary>
        /// How suitable a track is for dancing based on a combination of musical elements including tempo, rhythym stability, beat strength, and overall regularity.
        /// </summary>
        public float danceability { get; set; }

        /// <summary>
        /// The duration of the track in milliseconds.
        /// </summary>
        public int duration_ms { get; set; }

        /// <summary>
        /// A perceptual measure of intensity and activity.
        /// <para>Energetic tracks feel fast, loud, and noisy.</para>
        /// <para>Perceptual features contributing to this include dynamic range, perceived loudness, timbre, onset rate, and general entropy.</para>
        /// </summary>
        public float energy { get; set; }

        /// <summary>
        /// Predicts whether a contains no vocals.
        /// <para>"Ooh" and "aah" sounds are treated as instrumental in this context.</para>
        /// </summary>
        public float instrumentalness { get; set; }

        /// <summary>
        /// The key the track is in.
        /// <para>Integers map to pitches using standard Pitch Class Notation.</para>
        /// </summary>
        public int key { get; set; }

        /// <summary>
        /// Detects the presence of an audience in the recording.
        /// </summary>
        public float liveness { get; set; }

        /// <summary>
        /// The overall loudness of a track in decibels (dB).
        /// </summary>
        public float loudness { get; set; }

        /// <summary>
        /// Indicates the modality (major or minor) of a track, the type of scale from which its melodic content is derived.
        /// <para>Major is represented by 1 and minor is 0.</para>
        /// </summary>
        public int mode { get; set; }

        /// <summary>
        /// Detects the presence of spoken words in a track. The more exclusively speech-like the recording, the higher the value.
        /// </summary>
        public float speechiness { get; set; }

        /// <summary>
        /// The overall estimated tempo of a track in beats per minute (BPM).
        /// </summary>
        public float tempo { get; set; }

        /// <summary>
        /// An estimated overall time signature (meter) of a track.
        /// </summary>
        public int time_signature { get; set; }

        /// <summary>
        /// The musical positiveness conveyed by a track. 
        /// <para>Low valence is more negative emotions and high valence is more positive emotions.</para>
        /// </summary>
        public float valence { get; set; }
    }

    public struct Image
    {
        public int height { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

    public struct Copyright
    {
        public string text { get; set; }
        public char type { get; set; }
    }

    public struct Followers
    {
        public string href { get; set; }
        public int total { get; set; }
    }

    public struct Pager<T>
    {
        public string href { get; set; }
        public IEnumerable<T> items { get; set; }
        public int limit { get; set; }
        public string next { get; set; }
        public int offset { get; set; }
        public string previous { get; set; }
        public int total { get; set; }
    }
}
