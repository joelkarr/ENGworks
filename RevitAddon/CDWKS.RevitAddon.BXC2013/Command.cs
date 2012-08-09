using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CDWKS.RevitAddon.BXC2013
{
    [Transaction(TransactionMode.Automatic)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command : IExternalCommand
    {
        public string DownloadFolder { get; set; }
        public string DateFolder { get; set; }

        #region IExternalCommand Members

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var app = commandData.Application;
            var doc = app.ActiveUIDocument;
            var browser = new UserControl(doc);
            var dr = browser.ShowDialog();
            if (dr == DialogResult.OK)
            {
            }
            return Result.Succeeded;
        }

        #endregion
    }
}