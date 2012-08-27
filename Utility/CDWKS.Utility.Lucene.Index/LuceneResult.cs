using System.Collections.Generic;
using Lucene.Net.Documents;

namespace CDWKS.Utility.Lucene.Index
{
    public class LuceneResult
    {
        public int TotalCount { get; set; }
        public List<Document> Results { get; set; }
    }
}
