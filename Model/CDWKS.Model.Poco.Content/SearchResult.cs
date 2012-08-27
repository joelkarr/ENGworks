using System.Collections.Generic;

namespace CDWKS.Model.Poco.Content
{
    public class SearchResult
    {
        public int TotalCount { get; set; }
        public List<ItemSummary> Results { get; set; }
    }
}
