using System.Collections.Generic;

namespace CDWKS.Model.Poco.Content
{
    public class XmlTree 
    {
        public XmlTree()
        {
            Root = new Root();
        }
        public Root Root
        {
            get;
            set;
        }
    }
    public class Root : Folder
    {
    }
    public class Folder
    {
        public Folder()
        {
            Folders = new List<Folder>();
            Families = new List<Family>();
        }
        public string Name { get; set; }
        public List<Folder> Folders { get; set; }
        public List<Family> Families { get; set; }
    }
    public class Family
    {
        public string Name { get; set; }
    }
}
