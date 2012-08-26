namespace CDWKS.Utility.Lucene.Index
{
    class Program
    {
        static void Main()
        {
            Index.CreateBIMXchangeIndex();
            Search.SearchBIMXchange("start");
        }

     
    }
}
