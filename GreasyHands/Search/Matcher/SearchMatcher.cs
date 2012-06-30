using System.Text.RegularExpressions;
using GreasyHands.DAL.Container;

namespace GreasyHands.Search.Matcher
{
    public class SearchMatcher : ISearchMatcher
    {
        public bool MatchFilename(Query query, string filename, C2CPreference c2CPreferences, MatchTitle matchTitle)
        {
            var match = Regex.Match(filename, @"^(.+?)\s+(?:v\d+)?\s*(\d+)\s*(\(?of\s+\d+\)?)?\s*(?:\(?(\d\d\d\d)\)?)?", RegexOptions.IgnoreCase);
            var result = false;
            var covertocovermatch = new Regex(@"C2C", RegexOptions.IgnoreCase);

            if (match.Success)
            {
                var name = match.Groups[1].Value.ToLower();

                int num;
                    
                if (!int.TryParse(match.Groups[2].Value, out num))
                {
                    return false;
                }

                if (name.FuzzyTitle() == query.Title.FuzzyTitle() && num == query.Num)
                {
                    result = true;
                }
                else if (matchTitle == MatchTitle.Partial && name.FuzzyTitle().Contains(query.Title.FuzzyTitle()) && num == query.Num)
                {
                    result = true;
                }

                if (result)
                {
                    if (c2CPreferences == C2CPreference.Ignore && covertocovermatch.IsMatch(filename))
                    {
                        result = false;
                    }
                    else if (c2CPreferences == C2CPreference.Only && !covertocovermatch.IsMatch(filename))
                    {
                        result = false;
                    }

                    if (result)
                    {
                        if (!string.IsNullOrWhiteSpace(query.Year))
                        {
                            result = query.Year == match.Groups[4].Value;
                        }
                    }
                }

            }

            return result;
        }
    }
}