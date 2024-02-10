namespace GenreFinderApi.Services
{
    public class GenrePromptGenerator
    {
        private readonly List<string> _genreOptions;

        public GenrePromptGenerator()
        {
            // Load genre options from text file
            _genreOptions = LoadGenreOptions();
        }

        private List<string> LoadGenreOptions()
        {
            var genreOptions = new List<string>();
            
            // Assuming genres.txt is in the project root directory
            string filePath = "genres.txt";

            // Read each line from the text file and add it to the list
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    genreOptions.Add(line);
                }
            }

            return genreOptions;
        }

        public string GenerateChatCompletionPrompt(IEnumerable<string> artists)
        {
            // Concatenate artist names
            string baseString = "\nArtist Names: " + string.Join(", ", artists);

            // Define prompt and constraint strings
            string prompt = "\nChoose music genre tags from the list that best describe the set of artists given.";
            string constraint = "\nOutput must be: comma separated, contains only tags, up to 5 tags";

            // Combine all parts into final prompt
            return string.Join("", _genreOptions) + baseString + prompt + constraint;
        }
    }
}