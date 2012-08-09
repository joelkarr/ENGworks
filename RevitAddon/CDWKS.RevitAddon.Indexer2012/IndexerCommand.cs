using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CDWKS.Model.EF.FileQueue;
using CDWKS.RevitAddon.Indexer2012.RevitFileServiceReference;
using CDWKS.Utility.Revit2012;


namespace CDWKS.RevitAddon.Indexer2012
{
    [Transaction(TransactionMode.Automatic)]
    [Regeneration(RegenerationOption.Manual)]
    public class IndexerCommand : IExternalCommand
    {

        public string ConnectionString { get; set; }
        public BasicHttpBinding Binding { get; set; }
        public EndpointAddress Endpoint { get; set; }
        public ExternalCommandData CommandData { get; set; }
        public RevitFamilyIndexer Indexer { get; set; }
        public Guid InstanceId { get; set; }

        public void Initialize()
        {
            #region Binding

            Binding = new BasicHttpBinding(BasicHttpSecurityMode.None)
                          {
                              CloseTimeout = new TimeSpan(0, 0, 1, 0),
                              OpenTimeout = new TimeSpan(0, 0, 1, 0),
                              ReceiveTimeout = new TimeSpan(0, 0, 10, 0),
                              SendTimeout = new TimeSpan(0, 0, 1, 0),
                              AllowCookies = false,
                              BypassProxyOnLocal = false,
                              HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
                              MaxBufferSize = 200000000,
                              MaxBufferPoolSize = 200000000,
                              MaxReceivedMessageSize = 200000000,
                              MessageEncoding = WSMessageEncoding.Text,
                              TextEncoding = Encoding.UTF8,
                              TransferMode = TransferMode.Buffered,
                              UseDefaultWebProxy = true,
                              ReaderQuotas =
                                  {
                                      MaxDepth = 32,
                                      MaxStringContentLength = 200000000,
                                      MaxArrayLength = 200000000,
                                      MaxBytesPerRead = 4096,
                                      MaxNameTableCharCount = 16384
                                  }
                          };
            Binding.Security.Mode = BasicHttpSecurityMode.None;

            #endregion

            var path = Assembly.GetExecutingAssembly().Location;
            const string str = "CDWKS.RevitAddon.Indexer2012.dll";
            var str1 = path.Substring(0, path.Length - str.Length);
            ConnectionString =
                @"metadata=res://*/FileModel.csdl|res://*/FileModel.ssdl|res://*/FileModel.msl;provider=System.Data.SqlServerCe.3.5;provider connection string='DataSource=C:\ProgramData\Autodesk\Revit\Addins\2012\BIMXchangeAdmin.bundle\FileQueueDB.sdf'";
            Endpoint = new EndpointAddress(Constants.WebserviceURL);
            InstanceId = CreateNewInstance();
            AddTask(Enums.TaskAction.Index);
        }

        public void AddTask(Enums.TaskAction action)
        {
            using (var model = new FileQueueEntities(ConnectionString))
            {
                var newTask = new IndexerTask
                {
                    Id = Guid.NewGuid(),
                    Action = action.ToString(),
                    DateTime = DateTime.Now,
                    InstanceId = InstanceId
                };
                model.AddToIndexerTasks(newTask);
                model.SaveChanges();
            }
        }


        #region IExternalCommand Members

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Initialize();
            CommandData = commandData;
            //Login
            using(var login = new LoginForm())
            {
                login.ShowDialog();
                if(login.DialogResult == DialogResult.OK)
                {
                    Indexer = new RevitFamilyIndexer(CommandData);
                    var path = Assembly.GetExecutingAssembly().Location;
                    const string str = "CDWKS.RevitAddon.Indexer2012.dll";
                    var str1 = path.Substring(0, path.Length - str.Length);
                    var args = InstanceId.ToString() + " 2012";
                    var oProcess = Process.Start(str1 + "CDWKS.RevitAddOn.ProgressMonitor.exe", args);

                    while (oProcess != null && !oProcess.HasExited)
                    {
                        if (CanIndex())
                        {
                            IndexNext();
                        }
                    }
                }
            }

          

            return Result.Succeeded;
        }

        #endregion

        #region Indexer

        private bool CanIndex()
        {
            using (var model = new FileQueueEntities(ConnectionString))
            {
                Func<IndexerTask, bool> lastTask = t => t.InstanceId == InstanceId;
                var task = model.IndexerTasks.Where(lastTask).OrderByDescending(t => t.DateTime).FirstOrDefault();
                return task != null;
            }
        }

        private void IndexNext()
        {
            using (var model = new FileQueueEntities(ConnectionString))
            {

                Func<FileQueue, bool> filter = f => f.Status == Enums.IndexStatus.Queued.ToString()
                                                    & f.InstanceId == InstanceId;
                var filequeue = model.FileQueues.Where(filter).FirstOrDefault();
                if (filequeue == null) return;
                filequeue.Status =  IndexFile(filequeue);
                filequeue.Attempts = filequeue.Attempts + 1;
                model.SaveChanges();
            }
        }

       


    private string IndexFile(FileQueue filequeue)
        {
            try
            {
                using (var client = new AutodeskFileServiceClient(Binding, Endpoint))
                {

                    var indexedRevitFamily = Indexer.GetFile(filequeue.FilePath);
                    indexedRevitFamily.MC_OwnerId = Constants.OwnerId;
                    var request = new IndexAutodeskFileRequest(indexedRevitFamily, true);
                    var items = indexedRevitFamily.Items;
                    indexedRevitFamily.Items = null;
                    var result = client.IndexAutodeskFile(request);
                    foreach (var itemRequest in
                        items.Select(item => new AddTypeToFileRequest(item, indexedRevitFamily.Name, Constants.OwnerId)))
                    {
                       var typeResponse = client.AddTypeToFile(itemRequest);
                    }
                    return result.IndexAutodeskFileResult;
                }
            }
            catch (Exception)
            {
                return Enums.IndexStatus.DataSyncFailed.ToString();
            }
        }

  

        #endregion

        #region Helpers

      
        private Guid CreateNewInstance()
        {
            using (var model = new FileQueueEntities(ConnectionString))
            {
                var instance = new IndexerInstance {DateTime = DateTime.Now, Id = Guid.NewGuid()};
                model.AddToIndexerInstances(instance);
                model.SaveChanges();
                return instance.Id;
            }
        }

        #endregion
    }


}
