using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using SpotifyAPI.Web;

namespace MinimalSongRecApi.Utilities
{
    public class CustomNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            // Capitalize the first letter of the property name
            return char.ToUpper(name[0]) + name.Substring(1);
        }
    }

    // Only to be instantiated with JSON deserialization
    public class SongRecommendationResponse
    {
        public List<FullTrack> Tracks { get; set; }
        public List<RecommendationSeed> Seeds { get; set; }
    }

    public class RecommendationSeed
    {
        public int AfterFilteringSize { get; set; }
        public int AfterRelinkingSize { get; set; }
        public string Href { get; set; }
        public string Id { get; set; }
        public int InitialPoolSize { get; set; }
        public string Type { get; set; }
    }
}