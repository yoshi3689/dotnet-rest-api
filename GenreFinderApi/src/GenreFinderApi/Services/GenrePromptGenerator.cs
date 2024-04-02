namespace GenreFinderApi.Services
{
    public class GenrePromptGenerator
    {
        private readonly string _genreOptions = @"[
            acoustic,
            afrobeat,
            alt-rock,
            alternative,
            ambient,
            anime,
            black-metal,
            bluegrass,
            blues,
            bossanova,
            brazil,
            breakbeat,
            british,
            cantopop,
            chicago-house,
            children,
            chill,
            classical,
            club,
            comedy,
            country,
            dance,
            dancehall,
            death-metal,
            deep-house,
            detroit-techno,
            disco,
            disney,
            drum-and-bass,
            dub,
            dubstep,
            edm,
            electro,
            electronic,
            emo,
            folk,
            forro,
            french,
            funk,
            garage,
            german,
            gospel,
            goth,
            grindcore,
            groove,
            grunge,
            guitar,
            happy,
            hard-rock,
            hardcore,
            hardstyle,
            heavy-metal,
            hip-hop,
            holidays,
            honky-tonk,
            house,
            idm,
            indian,
            indie,
            indie-pop,
            industrial,
            iranian,
            j-dance,
            j-idol,
            j-pop,
            j-rock,
            jazz,
            k-pop,
            kids,
            latin,
            latino,
            malay,
            mandopop,
            metal,
            metal-misc,
            metalcore,
            minimal-techno,
            movies,
            mpb,
            new-age,
            new-release,
            opera,
            pagode,
            party,
            philippines-opm,
            piano,
            pop,
            pop-film,
            post-dubstep,
            power-pop,
            progressive-house,
            psych-rock,
            punk,
            punk-rock,
            r-n-b,
            rainy-day,
            reggae,
            reggaeton,
            road-trip,
            rock,
            rock-n-roll,
            rockabilly,
            romance,
            sad,
            salsa,
            samba,
            sertanejo,
            show-tunes,
            singer-songwriter,
            ska,
            sleep,
            songwriter,
            soul,
            soundtracks,
            spanish,
            study,
            summer,
            swedish,
            synth-pop,
            tango,
            techno,
            trance,
            trip-hop,
            turkish,
            work-out,
            world-music
        ]";

        // public GenrePromptGenerator()
        // {
        //     // Load genre options from text file
        //     _genreOptions = LoadGenreOptions();
        // }
        //
        // private List<string> LoadGenreOptions()
        // {
        //     var genreOptions = new List<string>();
        //     
        //     // Assuming genres.txt is in the project root directory
        //     string filePath = "genres.txt";
        //
        //     // Read each line from the text file and add it to the list
        //     using (StreamReader sr = new StreamReader(filePath))
        //     {
        //         string line;
        //         while ((line = sr.ReadLine()) != null)
        //         {
        //             genreOptions.Add(line);
        //         }
        //     }
        //
        //     return genreOptions;
        // }

        public string GenerateChatCompletionPrompt(IEnumerable<string> artists)
        {
            // Concatenate artist names
            string baseString = "\nArtist Names: " + string.Join(", ", artists);

            // Define prompt and constraint strings
            string prompt = "\nChoose music genre tags from the list that best describe the set of artists given.";
            string constraint = "\nOutput must be: comma separated, contains only tags, up to 5 tags, if no artists provided, no output";

            // Console.WriteLine("This is the genres set: " + _genreOptions);
            // Combine all parts into final prompt
            return _genreOptions + baseString + prompt + constraint;
        }
    }
}