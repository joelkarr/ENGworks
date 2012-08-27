using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CDWKS.Model.Poco.Content
{
    public class ItemSummary
    {
        public string FamilyName { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> FeaturedAttributes { get; set; }
        public string TypeCatalog { get; set; }
    }
}
