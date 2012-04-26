using System.Collections.Generic;

namespace GreasyHands.Search.Provider
{
    public interface ISearchProvider
    {
        List<Result> Search(Query query);
    }
}
