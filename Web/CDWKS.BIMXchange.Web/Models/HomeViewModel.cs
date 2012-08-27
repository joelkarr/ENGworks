using System.Collections.Generic;
using CDWKS.BIMXchange.Web.Models.Partial;

namespace CDWKS.BIMXchange.Web.Models
{
    public class HomeViewModel :BaseViewModel
    {
        public List<ItemSummaryViewModel> Items { get; set; }

        public PaginationViewModel Pagination { get; set; }

        public Dictionary<string, string> Filters { get; set; }

        public int SelectedTreeNode { get; set; }
        public List<int> OpenTreeNodes { get; set; } 
    }
}