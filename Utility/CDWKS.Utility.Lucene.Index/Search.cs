
using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;

namespace CDWKS.Utility.Lucene.Index
{
    public class Search
    {
        public static LuceneResult SearchBIMXchange(string field, string key, int pageSize, int pageNumber)
        {
            var directory = FSDirectory.Open(new DirectoryInfo("C:\\LuceneIndex"));
            var analyzer = new StandardAnalyzer(Version.LUCENE_29);

            var parser = new QueryParser(Version.LUCENE_29, field, analyzer);
            var query = parser.Parse(String.Format("{0}*", key));

            var searcher = new IndexSearcher(directory, true);

            var topDocs = searcher.Search(query, 1000000);

            var docs = new List<Document>();
            var start = (pageNumber-1)*pageSize;
            for (var i = start; i < start + pageSize && i < topDocs.TotalHits; i++)
            {
                var scoreDoc = topDocs.ScoreDocs[i];
                var docId = scoreDoc.doc;
                var doc = searcher.Doc(docId);
                docs.Add(doc);
            }

            searcher.Close();
            directory.Close();
            var result = new LuceneResult {Results = docs, TotalCount = topDocs.TotalHits};
            return result;
        }

        public static LuceneResult MultiSearchBIMXchange(Dictionary<string,string> terms, int pageSize, int pageNumber)
        {
            var directory = FSDirectory.Open(new DirectoryInfo("LuceneIndex"));
            var booleanQuery = new BooleanQuery();
            foreach(var term in terms)
            {
                var query = new TermQuery(new Term(term.Key, term.Value));
                booleanQuery.Add(query,BooleanClause.Occur.MUST);
            }
            var searcher = new IndexSearcher(directory, true);

            var topDocs = searcher.Search(booleanQuery, 10);

            var docs = new List<Document>();
            var start = (pageNumber - 1) * pageSize;
            for (var i = start; i < start + pageSize && i < topDocs.TotalHits; i++)
            {
                var scoreDoc = topDocs.ScoreDocs[i];
                var docId = scoreDoc.doc;
                var doc = searcher.Doc(docId);
                docs.Add(doc);
            }

            searcher.Close();
            directory.Close();
            var result = new LuceneResult {Results = docs, TotalCount = topDocs.TotalHits};
            return result;
        }
        

    
    }
}
