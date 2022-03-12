namespace WordleSolver
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Dictionary that will contain all the words used to guess the words based on a pattern provided
    /// </summary>
    public class WordDictionary
    {
        /// <summary>
        /// A list that contains all 5 letter words in a collection
        /// </summary>
        private readonly List<string> WordList = new List<string>();

        /// <summary>
        /// Default dictionary location.
        /// From: https://github.com/dwyl/english-words
        /// </summary>
        private readonly string DictionaryDefaultSource = "https://raw.githubusercontent.com/dwyl/english-words/master/words.txt";
        
        /// <summary>
        /// A wildcard character
        /// </summary>
        private readonly char WildcardCharacter = '*';

        /// <summary>
        /// Constructor for the class.
        /// Will populate the dictionary with a custom or default source.
        /// </summary>
        /// <param name="dictionaryPath"></param>
        /// <param name="wordLength">The length needed for this word to be included</param>
        public WordDictionary(string dictionaryPath, int wordLength = 5)
        {
            if (string.IsNullOrWhiteSpace(dictionaryPath) || 
                !this.PopulateDictionary(dictionaryPath, wordLength))
            {
                Console.WriteLine($"Building dictionary from default addres: {this.DictionaryDefaultSource}");
                using HttpClient client = new();
                using StreamReader reader = new(client.GetStreamAsync(this.DictionaryDefaultSource).GetAwaiter().GetResult());
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    EvaluateAndAddToDictionary(line, wordLength);
                }
            }
        }

        /// <summary>
        /// Function used to populate the list with 5 letter words contained in the source (custom or default)
        /// </summary>
        /// <param name="dictionaryPath">The path to a local dictionary</param>
        /// <param name="wordLength">The length needed for this word to be included</param>
        /// <returns>True if the dictionary was created, false otherwise</returns>
        private bool PopulateDictionary(string dictionaryPath, int wordLength)
        {
            Console.WriteLine($"Building dictionary from {dictionaryPath}");
            try
            {
                foreach (string line in File.ReadLines(dictionaryPath))
                {
                    EvaluateAndAddToDictionary(line, wordLength);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an issue with the dictionary provided: " + ex.ToString());
            }

            bool isDictionaryPopulated = this.WordList.Count != 0;

            if (!isDictionaryPopulated)
            {
                Console.WriteLine("Your dictionary has no 5 letter words.");
            }

            return isDictionaryPopulated;
        }

        /// <summary>
        /// Adds a word to the dictionary if it contains 5 letters
        /// </summary>
        /// <param name="word">The word to be evaluated</param>
        /// <param name="wordLength">The length needed for this word to be included</param>
        /// <returns>True if the word was added. False otherwise</returns>
        private bool EvaluateAndAddToDictionary(string word, int wordLength)
        {
            if (word.Length == wordLength)
            {
                this.WordList.Add(word);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Receives a pattern with Wildcards to try and match with words contained in the dictionary
        /// </summary>
        /// <param name="patternToMatch">A pattern with wildcards</param>
        /// <returns>A list of words that match the pattern</returns>
        /// <exception cref="InvalidOperationException">Throws this exception when the pattern is not a valid one.</exception>
        public List<string> FindCoincidences(string patternToMatch)
        {
            if (string.IsNullOrWhiteSpace(patternToMatch))
            {
                throw new InvalidOperationException("You should provide a non-empty pattern");
            }

            patternToMatch = patternToMatch.ToLower();

            int count = patternToMatch.Count(character => character.Equals(WildcardCharacter));

            if (count == 0)
            {
                throw new InvalidOperationException("The pattern provided should have a hint in the form of '*as*y'");
            }

            string regexPattern = "^" + Regex.Escape(patternToMatch).Replace(@"\*", ".*").Replace(@"\?", ".") + "$";
            Regex evaluator = new Regex(regexPattern, RegexOptions.IgnoreCase);

            List<string> results = new List<string>();
            foreach (string word in this.WordList)
            {
                if (evaluator.IsMatch(word))
                {
                    results.Add(word);
                }
            }

            Console.WriteLine($"Found {results.Count} coincidences");

            return results;
        }

        /// <summary>
        /// Number of words in the dictionary
        /// </summary>
        /// <returns>The number of items in this list</returns>
        public int WordCount()
        {
            return this.WordList.Count;
        }

        /// <summary>
        /// Prints the words in this list
        /// </summary>
        public void PrintDictionary()
        {
            foreach (string word in this.WordList)
            {
                Console.WriteLine(word);
            }
        }
    }
}
