using GreasyHands.DAL.Container;

namespace GreasyHands.Search.Matcher
{
    public interface ISearchMatcher
    {
        bool MatchFilename(Query query, string filename, C2CPreference c2CPreferences, MatchTitle matchTitle);
    }
}
