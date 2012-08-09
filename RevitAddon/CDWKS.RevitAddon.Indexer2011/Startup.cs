using System;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;

namespace CDWKS.RevitAddon.Indexer2011
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
            var rootPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\\Autodesk\\Revit\\Addins\\2011\\BIMXchangeAdmin.bundle";
            
            const string str = "CDWKS.RevitAddon.Indexer2011.dll";
            var ribbonPanel = application.CreateRibbonPanel("BIMXchange Admin");
            #region Content Indexer
            var pushButtonDataIndexer = new PushButtonData("Content Indexer", "Content Indexer", string.Format("{0}\\{1}", rootPath, str),
                                                     "CDWKS.RevitAddon.Indexer2011.IndexerCommand");
            var ribbonItemIndexer = ribbonPanel.AddItem(pushButtonDataIndexer);
            var pushButtonIndexer = ribbonItemIndexer as PushButton;

            pushButtonIndexer.LargeImage = (new BitmapImage(new Uri(string.Format("{0}\\Library_32.png", rootPath))));
            #endregion

            #region Content TreeCreator
            var pushButtonDataTreeCreator = new PushButtonData("Library Tree", "Library Tree", string.Format("{0}\\{1}", rootPath, str),
                                                     "CDWKS.RevitAddon.Indexer2011.TreeViewCreator");
            var ribbonItemTreeCreator = ribbonPanel.AddItem(pushButtonDataTreeCreator);
            var pushButtonTreeCreator = ribbonItemTreeCreator as PushButton;

            pushButtonTreeCreator.LargeImage = (new BitmapImage(new Uri(string.Format("{0}\\Library_32.png", rootPath))));
            #endregion
            return Result.Succeeded;
        }
        //var contextHelp = new ContextualHelp(
        //    ContextualHelpType.ChmFile,
        //    path.Replace(str, string.Empty).TrimEnd('\\') + @"\Contents\Resources\ENGworks.BIMXchange.htm");
        //pushButton.SetContextualHelp(contextHelp);
        #endregion
    }
}