
using System;
using System.IO;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;

namespace CDWKS.Utility.Lucene.Index
{
    public class Search
    {
        public static void SearchBIMXchange(string key)
        {
            var directory = FSDirectory.Open(
                                     new DirectoryInfo("LuceneIndex")
                                  );
            var analyzer = new StandardAnalyzer(Version.LUCENE_29);

            var parser = new QueryParser(Version.LUCENE_29, "name", analyzer);
            var query = parser.Parse(String.Format("{0}*", key));

            var searcher = new IndexSearcher(directory, true);

            var topDocs = searcher.Search(query, 10);

            var results = topDocs.ScoreDocs.Length;
            Console.WriteLine("Found {0} results", results);

            for (var i = 0; i < results; i++)
            {
                var scoreDoc = topDocs.ScoreDocs[i];
                var score = scoreDoc.score;
                var docId = scoreDoc.doc;
                var doc = searcher.Doc(docId);

                Console.WriteLine("Result num {0}, score {1}", i + 1, score);
                Console.WriteLine("ID: {0}", doc.Get("id"));
                Console.WriteLine("Text found: {0}\r\n", doc.Get("postBody"));
            }

            searcher.Close();
            directory.Close();
        }
    }
}
