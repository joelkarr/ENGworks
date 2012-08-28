using System.Collections.Generic;
using System.Web.Mvc;

using CDWKS.BIMXchange.Web.Controllers.Base;
using CDWKS.BIMXchange.Web.Models.Partial;

namespace CDWKS.BIMXchange.Web.Controllers
{
    public class DataController : BaseController
    {
        #region Actions

        public JsonResult Tree(int id)
        {
            var root = new TreeViewModel
            {
                data = "Root Node",
                metadata = new Dictionary<string, string> { { "id", "1" } }
            };
            var top = new TreeViewModel
            {
                data = "top",
                metadata = new Dictionary<string, string> { { "id", "2" } },
                children = new List<TreeViewModel>()
            };
            var middle = new TreeViewModel
            {
                data = "middle",
                metadata = new Dictionary<string, string> { { "id", "3" } }
                ,
                children = new List<TreeViewModel>()
            };

            root.children = new List<TreeViewModel> { top, middle };

            return Json(root, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
