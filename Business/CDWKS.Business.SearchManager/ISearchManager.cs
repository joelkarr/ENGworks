using System;
using System.Collections.Generic;
using CDWKS.Model.Poco.Content;

namespace CDWKS.Business.SearchManager
{
    public interface ISearchManager : IDisposable
    {
        SearchResult Search(string keyword, int pageSize, int pageNumber);
        SearchResult Search(Dictionary<string, string> criteria, int pageSize, int pageNumber);
    }
}
