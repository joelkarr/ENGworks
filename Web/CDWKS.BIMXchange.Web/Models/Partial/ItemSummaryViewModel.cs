using System.Collections.Generic;
using CDWKS.Model.Poco.Content;

namespace CDWKS.BIMXchange.Web.Models.Partial
{
    public class ItemSummaryViewModel
    {
        public string FamilyName { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> FeaturedAttributes { get; set; }
        public string TypeCatalog { get; set; }

        public bool HasTypeCatalog { get; set; }

        public ItemSummaryViewModel Populate(ItemSummary result)
        {
            FamilyName = result.FamilyName;
            Name = result.Name;
            FeaturedAttributes = result.FeaturedAttributes ?? new Dictionary<string, string>(); 
            return this;
        }
    }
}