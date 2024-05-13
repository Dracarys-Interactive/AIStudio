using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DracarysInteractive.AIStudio
{
    public static class StringHelper
    {
        public static string[] ExtractTaggedStrings(string input, string tag)
        {
            List<string> extractedStrings = new List<string>();

            // Regular expression pattern to match the XML open and closing tag
            string pattern = $@"<{tag}[^>]*>(.*?)<\/{tag}>";

            // Match the pattern in the input string
            MatchCollection matches = Regex.Matches(input, pattern);

            // Iterate through the matches and extract the text inside the XML elements
            foreach (Match match in matches)
            {
                // Get the text inside the XML element without the opening and closing tag
                string extractedString = match.Groups[1].Value;
                extractedStrings.Add(extractedString);
            }

            return extractedStrings.ToArray();
        }

        public static string RemoveTaggedStrings(string input, string tag)
        {
            // Regular expression pattern to match the XML open and closing tag
            string pattern = $@"<{tag}[^>]*>(.*?)<\/{tag}>";

            // Remove all occurrences of the pattern from the input string
            string result = Regex.Replace(input, pattern, "");

            return result;
        }

        public static string[] ExtractStringsInParentheses(string input)
        {
            // Define the regular expression pattern
            string pattern = @"\((.*?)\)";

            // Create a regular expression object
            Regex regex = new Regex(pattern);

            // Find all matches in the input string
            MatchCollection matches = regex.Matches(input);

            // Create an array to store the extracted strings
            string[] extractedStrings = new string[matches.Count];

            // Iterate over the matches and extract the strings
            for (int i = 0; i < matches.Count; i++)
            {
                // Remove the parentheses from the matched string
                string extractedString = matches[i].Groups[1].Value;
                extractedStrings[i] = extractedString;
            }

            return extractedStrings;
        }

        public static string RemoveStringsInParentheses(string input)
        {
            // Define the regular expression pattern
            string pattern = @"\((.*?)\)";

            // Create a regular expression object
            Regex regex = new Regex(pattern);

            // Replace all matches with an empty string
            string result = regex.Replace(input, "");

            return result;
        }

        public static string[] SplitCompletion(string completion)
        {
            List<string> subcompletions = new List<string>();
            string delim = "\\n\\n";
            int i = completion.IndexOf(delim);

            if (i == -1)
            {
                i = completion.IndexOf(delim = "\\n");
            }

            while (i != -1)
            {
                subcompletions.Add(filterSubcompletion(completion.Substring(0, i)));
                completion = completion.Substring(i + delim.Length);
                i = completion.IndexOf(delim);
            }

            subcompletions.Add(filterSubcompletion(completion));

            return subcompletions.ToArray();
        }

        private static string filterSubcompletion(string subcompletion)
        {
            return subcompletion.Replace("\\n", " ").Replace("\\", "");
        }

        public static string Remove(string completion, string regex)
        {
            return Regex.Replace(completion, regex, String.Empty);
        }

        public static string PostProcessJSON(string JSON)
        {
            string s = JSON.Replace("\\", "\\\\");
            s = s.Replace("'", "\\'");
            s = s.Replace("`", "\\`");
            return s;
        }
    }
}
