using System;
using System.IO;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;

namespace CDWKS.RevitAddon.Taco2012
{
    [Transaction(TransactionMode.Automatic)]
    [Regeneration(RegenerationOption.Manual)]
    public class Startup : IExternalApplication
    {
        #region IExternalApplication Members

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            var rootPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\\Autodesk\\Revit\\Addins\\2012\\BIMXchangeTaco.bundle";
            var panel = application.CreateRibbonPanel("Taco");

            var pdata = new PushButtonData("Taco Pump Finder", "Taco Pump Finder",
                                           string.Format("{0}\\CDWKS.RevitAddon.Taco2012.dll", rootPath), "CDWKS.RevitAddon.Taco2012.Command");
            var btnBrowser = (PushButton) panel.AddItem(pdata);
            if (File.Exists(string.Format("{0}\\Library_32.png", rootPath)))
            {
                btnBrowser.LargeImage = new BitmapImage(new Uri(string.Format("{0}\\Library_32.png", rootPath)));
            }
            if (File.Exists(string.Format("{0}\\Library_16.png", rootPath)))
            {
                btnBrowser.Image = new BitmapImage(new Uri(rootPath + @"\Library_16.png"));
            }
            return Result.Succeeded;
        }

        #endregion
    }
}