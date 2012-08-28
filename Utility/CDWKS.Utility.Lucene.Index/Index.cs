using System.IO;
using System.Linq;
using CDWKS.Model.EF.BIMXchange;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;

namespace CDWKS.Utility.Lucene.Index
{
    public class Index
    {
        public static void CreateBIMXchangeIndex()
        {

            var directory = FSDirectory.Open(
                                     new DirectoryInfo("C:\\LuceneIndex")
                                  );
            Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_29);
            var writer = new IndexWriter(directory, analyzer,
                                         IndexWriter.MaxFieldLength.UNLIMITED);

            //get all Revit Items
            using (var context = new BXCModelEntities())
            {
                var items = context.Items;
                foreach (var item in items)
                {
                    var doc = new Document();
                    doc.Add(new Field("id", item.Id.ToString(), Field.Store.YES, Field.Index.NO));
                    doc.Add(new Field("family", item.AutodeskFile.Name, Field.Store.YES, Field.Index.ANALYZED));
                    doc.Add(new Field("version", item.AutodeskFile.Version.ToString(), Field.Store.YES, Field.Index.ANALYZED));

                    var nodes = item.AutodeskFile.AutodeskFileTreeNodes;
                    var nodeString = nodes.Aggregate(string.Empty, (current, node) => string.Format("{0}{1} ", current, node.TreeNodes_Id));
                    doc.Add(new Field("nodes", nodeString, Field.Store.YES, Field.Index.ANALYZED));

                    var avaiableRevitVersions = item.AutodeskFile.RevitVersions
                                            .Aggregate(string.Empty, (current, version) =>
                                                current + string.Format("{0} ", version));
                    doc.Add(new Field("revitversion", avaiableRevitVersions, Field.Store.YES, Field.Index.ANALYZED));
                    doc.Add(new Field("name", item.Name, Field.Store.YES, Field.Index.ANALYZED));
                    foreach (var parameter in item.Parameters)
                    {
                        doc.Add(new Field(parameter.SearchName.Name, parameter.SearchValue.Value, Field.Store.YES, Field.Index.ANALYZED));
                    }

                    writer.AddDocument(doc);
                }
            }


            writer.Optimize();
            writer.Commit();
            writer.Close();
        }
    }
}
