using System;
using System.IO;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;

namespace CDWKS.RevitAddon.BXC2011
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
            var rootPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +  @"\\Autodesk\\Revit\\Addins\\2011\\BIMXchange.bundle";
            var version = application.ControlledApplication.VersionName;
            var panel = application.CreateRibbonPanel("ENGworks");

            var pdata = new PushButtonData("BIMXchange v4.0", "BIMXchange v4.0",
                                           rootPath + @"\CDWKS.RevitAddon.BXC2011.dll", "CDWKS.RevitAddon.BXC2011.Command");
            var btnBrowser = (PushButton) panel.AddItem(pdata);
            if (File.Exists(rootPath + @"\Library_32.png"))
            {
                btnBrowser.LargeImage = new BitmapImage(new Uri(rootPath + @"\Library_32.png"));
            }
            if (File.Exists(rootPath + @"\Library_16.png"))
            {
                btnBrowser.Image = new BitmapImage(new Uri(rootPath + @"\Library_16.png"));
            }
            return Result.Succeeded;
        }

        #endregion
    }
}