using System.Collections.Generic;

namespace CDWKS.BIMXchange.Web.Models.Partial
{
    public class TreeViewModel 
    {
        //Lower case needed for jstree
        // ReSharper disable InconsistentNaming
        public string data { get; set; }
        public Dictionary<string,string> metadata { get; set; }
        public List<TreeViewModel> children { get; set; }
        // ReSharper restore InconsistentNaming
    }
}
