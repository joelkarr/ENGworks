using System;
using System.Collections.Generic;
using System.Linq;
using CDWKS.Model.Poco.Content;
using Lucene.Net.Documents;
using Lucene.Net.Search.Vectorhighlight;

namespace CDWKS.Business.SearchManager
{
    public class SearchManager :ISearchManager
    {

        public SearchResult Search(string keyword, int pageSize, int pageNumber)
        {
            var searchResult = new SearchResult {Results = new List<ItemSummary>()};
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
            var searchResult = new SearchResult { Results = new List<ItemSummary>() };
            
            var luceneResult = Utility.Lucene.Index.Search.MultiSearchBIMXchange(criteria, pageSize, pageNumber);
            foreach (var doc in luceneResult.Results)
            {
                var itemSummary = new ItemSummary
                {
                    FamilyName = doc.Get("family"),
                    Name = doc.Get("name")
                };
                var fields = doc.fields_ForNUnit as List<Document>;
                itemSummary.FeaturedAttributes = new Dictionary<string, string>();
                var featuredFields = new List<String>{"make"};
                foreach(var f in fields)
                {
                    
                }
                searchResult.Results.Add(itemSummary);

            }
            searchResult.TotalCount = luceneResult.TotalCount;
            return searchResult;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
