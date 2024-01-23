using System.Text;

namespace MinimalSongRecApi.Utilities;

// only to be instantiated/initialized with JSON deserialization
public class SongRecommendationParameters
{
    // seed_artists=4NHQUGzhtTLFvgF5SZesLK%2C4NHQUGzhtTLFvgF5SZesLK&seed_genres=classical%2Ccountry%2Ccountry&seed_tracks=0c6xIDDpzE81m2q797ordA%2C0c6xIDDpzE81m2q797ordA&target_loudness=1
    // override toString method
        // to follow the above format
        // to add '&' between each param
    public int limit { get; set; } = 20;
    public string market { get; set; }
    public string seed_artists { get; set; }
    public string seed_genres { get; set; }
    public string seed_tracks { get; set; }
    public double? min_acousticness { get; set; }
    public double? max_acousticness { get; set; }
    public double? target_acousticness { get; set; }
    public double? min_danceability { get; set; }
    public double? max_danceability { get; set; }
    public double? target_danceability { get; set; }
    public int? min_duration_ms { get; set; }
    public int? max_duration_ms { get; set; }
    public int? target_duration_ms { get; set; }
    public double? min_energy { get; set; }
    public double? max_energy { get; set; }
    public double? target_energy { get; set; }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        // Iterate through the properties using reflection
        foreach (var property in GetType().GetProperties())
        {
            var value = property.GetValue(this);
            if (value != null)
            {
                stringBuilder.Append($"&{property.Name}={value}");
            }
        }
        return stringBuilder.ToString();
    }
    
    // public override string ToString()
    // {
    //     // Construct the formatted string
    //     return $"limit={limit}" +
    //            $"&market={market}" +
    //            $"&seed_artists={seed_artists}" +
    //            $"&seed_genres={seed_genres}" +
    //            $"&seed_tracks={seed_tracks}" + 
    //            min_acousticness != null ? $"&min_acousticness={min_acousticness}" : "" + 
    //            max_acousticness != null ? $"&max_acousticness={max_acousticness}" : "" + 
    //            target_acousticness != null ? $"&target_acousticness={target_acousticness}" : "" + 
    //            min_danceability != null ? $"&min_danceability={min_danceability}" : "" + 
    //            max_danceability != null ? $"&max_danceability={max_danceability}" : "" +
    //             target_danceability != null ? $"&target_danceability={target_danceability}" : "" +
    //             min_duration_ms != null ? $"&min_duration_ms={min_duration_ms}" : "" +
    //             max_duration_ms != null ? $"&max_duration_ms={max_duration_ms}" : "" +
    //             target_duration_ms != null ? $"&target_duration_ms={target_duration_ms}" : "" +
    //             min_energy != null ? $"&min_energy={min_energy}" : "" +
    //             max_energy != null ? $"&max_energy={max_energy}" : "" +
    //             target_energy != null ? $"&target_energy={target_energy}" : ""
    //            ;
    // }
}
