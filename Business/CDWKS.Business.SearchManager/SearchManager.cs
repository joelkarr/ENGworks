using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CDWKS.Model.Poco.Content;
using CDWKS.Utility.Lucene.Index;

namespace CDWKS.Business.SearchManager
{
    public class SearchManager :ISearchManager
    {


        public SearchResult Search(string keyword, int pageSize, int pageNumber)
        {
            var searchResult = new SearchResult();
            searchResult.Results = new List<ItemSummary>();
           var luceneResult = Utility.Lucene.Index.Search.SearchBIMXchange("family", keyword, pageSize, pageNumber);
            foreach(var doc in luceneResult.Results)
            {
                var itemSummary = new ItemSummary {FamilyName = doc.Get("family"),
                                                        Name = doc.Get("name")};
                var field = doc.fields_ForNUnit;
                searchResult.Results.Add(itemSummary);

            }
            searchResult.TotalCount = luceneResult.TotalCount;
            return searchResult;
        }

        public SearchResult Search(Dictionary<string, string> criteria, int pageSize, int pageNumber)
        {
           return new SearchResult();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
