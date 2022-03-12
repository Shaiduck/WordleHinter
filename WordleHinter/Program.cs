using WordleSolver;

Console.WriteLine("Welcome to Wordle Hinter!");
Console.WriteLine("Input the path to a custom dictionary or do nothing to download the default one");
string? path = Console.ReadLine() ?? string.Empty;

Console.WriteLine("Input the LENGTH of the words you want in this dictionary");
if (!int.TryParse(Console.ReadLine(), out int wordLength))
{
    wordLength = 5;
}

WordDictionary dict = new(path, wordLength);
string patternToMatch = string.Empty;

while(!string.Equals("Exit", patternToMatch, StringComparison.InvariantCultureIgnoreCase))
{
    Console.WriteLine("Input the PATTERN of the word you want to search for");
    patternToMatch = Console.ReadLine() ?? string.Empty;
    if (string.IsNullOrWhiteSpace(patternToMatch))
    {
        patternToMatch = "Exit";
        continue;
    }

    Console.WriteLine($"Pattern to search: {patternToMatch}");
    List<string> results = dict.FindCoincidences(patternToMatch);
    foreach(string word in results)
    {
        Console.WriteLine(word);
    }

    Console.WriteLine("Write a new pattern or type 'Exit' to exit the program");
}

Console.WriteLine("Bye!");