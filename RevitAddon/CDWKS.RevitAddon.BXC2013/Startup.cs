using System;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;

namespace CDWKS.RevitAddon.BXC2013
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
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\\Autodesk\\Revit\\Addins\\2013\\BIMXchange.bundle";
            
            const string str = "CDWKS.RevitAddon.BXC2013.dll";
            var ribbonPanel = application.CreateRibbonPanel("ENGworks");
            var pushButtonDatum = new PushButtonData("BIMXchange v4.0", "BIMXchange v4.0", string.Format("{0}\\{1}",path,str),
                                                     "CDWKS.RevitAddon.BXC2013.Command");
            var ribbonItem = ribbonPanel.AddItem(pushButtonDatum);
            var pushButton = ribbonItem as PushButton;
            var contextHelp = new ContextualHelp(
                ContextualHelpType.ChmFile,
                path + @"\Contents\Resources\ENGworks.BIMXchange.htm");
            pushButton.SetContextualHelp(contextHelp);
            var str1 = path.Substring(0, path.Length - str.Length);
            var uri = new Uri(string.Concat(path, "\\Library_32.png"));
            pushButton.LargeImage = (new BitmapImage(uri));
            return Result.Succeeded;
        }

        #endregion
    }
}