using System;
using System.Collections.Generic;
using CDWKS.Model.Poco.Content;

namespace CDWKS.Business.SearchManager
{
    public interface ISearchManager : IDisposable
    {
        List<Item> Search(string keyword);
        List<Item> Search(Dictionary<string, string> criteria);
    }
}
