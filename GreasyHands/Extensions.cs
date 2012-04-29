using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GreasyHands
{
    public static class Extensions
    {
        private static readonly Random Random = new Random((int)DateTime.Now.Ticks);
        //private const string RandomCharSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        private static readonly Dictionary<char, string> RandomStringTemplates = new Dictionary<char, string>
            {
                {'?', "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"},

                {'X', "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"},
                {'x', "abcdefghijklmnopqrstuvwxyz0123456789"},

                {'A', "ABCDEFGHIJKLMNOPQRSTUVWXYZ"},
                {'a', "abcdefghijklmnopqrstuvwxyz"},

                {'0', "0123456789"},

                {'H', "0123456789ABCDEF"},
                {'h', "0123456789abcdef"},
            };

        public static string FuzzyTitle(this String input)
        {
            var nonAlpahNum = new Regex(@"\W", RegexOptions.IgnoreCase);

            var output = input;

            output = Regex.Replace(output, @"The\s+", "", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"&", "and", RegexOptions.IgnoreCase);
            output = nonAlpahNum.Replace(output, "");
            output = output.ToLower();

            return output;
        }

        public static string Randomise(this String input)
        {
            var builder = new StringBuilder();

            //foreach (char ch in input.Select(t => RandomCharSet[Random.Next(RandomCharSet.Length)]))

            foreach (char c in input)
            {
                var randomSet = RandomStringTemplates[c];
                var ch = randomSet[Random.Next(randomSet.Length)];

                builder.Append(ch);
            }

            return builder.ToString();
        }

        public static string NiceDate(this DateTime input)
        {
            return input.ToString("yyyy-MM-dd");
        }

        public static string ReplaceMatch(this String input, string tomatch, string replace)
        {
            var regex = new Regex(tomatch, RegexOptions.IgnoreCase);
            var match = regex.Match(input);
            var result = input;

            if (match.Success)
            {
                result = result.Replace(match.Value, replace);
            }

            return result;
        }

    }
}