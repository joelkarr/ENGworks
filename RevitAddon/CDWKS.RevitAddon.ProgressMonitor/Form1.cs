using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using CDWKS.Model.EF.FileQueue;

namespace CDWKS.RevitAddOn.ProgressMonitor
{
    public partial class Form1 : Form
    {
         public Guid InstanceId { get; set; }
        public string RevitVersion { get; set; }
        public string ConnectionString { get; set; }

        public Form1(Guid instanceId, string revitVersion)
        {
            InitializeComponent();
            InstanceId = instanceId;
            RevitVersion = revitVersion;
            var path = Assembly.GetExecutingAssembly().Location;
            const string str = "CDWKS.BXC.Indexer2012.dll";
            var str1 = path.Substring(0, path.Length - str.Length);
            ConnectionString = @"metadata=res://*/FileModel.csdl|res://*/FileModel.ssdl|res://*/FileModel.msl;provider=System.Data.SqlServerCe.3.5;provider connection string='DataSource=C:\ProgramData\Autodesk\Revit\Addins\2012\BIMXchangeAdmin.bundle\FileQueueDB.sdf'";
         
            //Check if last time there were incomplete records
            var lastInstanceId = GetLastInstanceID();
            if(DoesExistIncompleteRecords(lastInstanceId))
            {
                
                var result = MessageBox.Show("Some files did not successfully complete Indexing during last session, would you like to retry those files?", "", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    ReQueueFailed(lastInstanceId);
                }
            }


            timer3.Interval = 10000;
            timer3.Start();
            timer2.Interval = 1000;
            timer2.Start();
                    
        }

        private bool DoesExistIncompleteRecords(Guid lastInstanceId)
        {
            using (var model = new FileQueueEntities(ConnectionString))
            {
                Func<FileQueue, bool> incomplete = f =>
                                                   f.InstanceId == lastInstanceId &&
                                                   (f.Status != Enums.IndexStatus.Succeeded.ToString()
                                                    || f.Status != Enums.IndexStatus.ContentUploaded.ToString());
                var failed = model.FileQueues.Where(incomplete);
                return failed.Any();
            }
        }

        private Guid GetLastInstanceID()
        {
            using (var model = new FileQueueEntities(ConnectionString))
            {

                Func<IndexerInstance, bool> lastInstance = i => i.Id != InstanceId;
                var last =
                    model.IndexerInstances.Where(lastInstance)
                        .OrderByDescending(i => i.DateTime)
                        .FirstOrDefault();
                if (last != null) return last.Id;
            }
            return Guid.NewGuid();
        }

        #region Methods
        public void ReQueueFailed(Guid lastInstanceId)
        {
            using (var model = new FileQueueEntities(ConnectionString))
            {

                Func<FileQueue, bool> incomplete = f =>
                                                  f.InstanceId == lastInstanceId &&
                                                  (f.Status != Enums.IndexStatus.Succeeded.ToString()
                                                   || f.Status != Enums.IndexStatus.ContentUploaded.ToString());
                var failed = model.FileQueues.Where(incomplete);
                foreach (var f in failed)
                {
                    f.Status = Enums.IndexStatus.Queued.ToString();
                    f.InstanceId = InstanceId;
                }
                model.SaveChanges();
            }
            UpdateGrid();
        }

        public void AddTask(Enums.TaskAction action)
        {
            using (var model = new FileQueueEntities(ConnectionString))
            {
                var newTask = new IndexerTask
                                  {Id = Guid.NewGuid(), 
                                      Action = action.ToString(), 
                                      DateTime = DateTime.Now, 
                                      InstanceId = InstanceId};
                model.AddToIndexerTasks(newTask);
                model.SaveChanges();
            }
        }

        #endregion

        #region Actions
        private void btnIndex_Click(object sender, EventArgs e)
        {
            var fldBrowser = new FolderBrowserDialog { Description = "Select Root Directory to Index (All Sub Folders will be indexed)" };
            var result = fldBrowser.ShowDialog();
            if (result == DialogResult.OK)
            {
                var paths = Directory.GetFiles(fldBrowser.SelectedPath, "*.rfa", SearchOption.AllDirectories);

                using (var jobModel = new FileQueueEntities(ConnectionString))
                {
                    var indexType = Enums.IndexType.Full;
                    if (rbNewOnly.Checked) indexType = Enums.IndexType.NewOnly;
                    if (rbNewAndVersion.Checked) indexType = Enums.IndexType.NewAndVersionUpdate;
                    var library = fldBrowser.RootFolder.ToString();
                    foreach (var filequeue in paths.Select(path =>
                                                           new FileQueue
                                                               {
                                                                   Id = Guid.NewGuid(),
                                                                   InstanceId = InstanceId,
                                                                   Library = library,
                                                                   FilePath = path,
                                                                   Attempts = 0,
                                                                   Status = Enums.IndexStatus.Queued.ToString(),
                                                                   RevitVersion = RevitVersion,
                                                                   IndexType = indexType.ToString(),
                                                                   AddedDateTime = DateTime.Now,
                                                                   FileName = path.Split('/')[path.Split('/').Count() -1]
                                                               }))
                    {
                        jobModel.AddObject("FileQueues", filequeue);
                        jobModel.SaveChanges();

                    }
                }
            }
        }


        #endregion

        private void timer3_Tick(object sender, EventArgs e)
        {
            UpdateGrid();

        }

        private void UpdateGrid()
        {
            using (var jobModel = new FileQueueEntities(ConnectionString))
            {
                lblTimeStamp.Text = String.Format("Last Updated: {0}", DateTime.Now.ToShortTimeString());
                var files = jobModel.FileQueues.Where(f => f.InstanceId == InstanceId);
                var strSuccess = Enums.IndexStatus.Succeeded.ToString();
                var strDataFailed = Enums.IndexStatus.DataSyncFailed.ToString();
                var strUploadFailed = Enums.IndexStatus.ContentUploadFailed.ToString();
                var strQueued = Enums.IndexStatus.Queued.ToString();
                var strDataSynced = Enums.IndexStatus.DataSynced.ToString();
                var strUploadSuccess = Enums.IndexStatus.ContentUploaded.ToString();
                lblDataSynced.Text = String.Format("DataSynced: {0}", files.Count(f => f.Status == strDataSynced));
                lblFailed.Text = String.Format("Failed: {0}", files.Count(f => f.Status == strDataFailed
                                                                             || f.Status == strUploadFailed));
                lblQueued.Text = String.Format("Queued: {0}", files.Count(f => f.Status == strQueued));
                lblSynced.Text = String.Format("Success: {0}", files.Count(f => f.Status == strSuccess
                    || f.Status == strUploadSuccess));
                dataGridView1.DataSource = files;
            }
        }
       private void UploadNext()
        {
            using (var model = new FileQueueEntities(ConnectionString))
            {


                Func<FileQueue, bool> filter = f => f.Status == Enums.IndexStatus.DataSynced.ToString()
                                                    & f.InstanceId == InstanceId;
                var filequeue = model.FileQueues.Where(filter).FirstOrDefault();
                if (filequeue == null)
                {
                    //wait a second before checking again if none found yet.
                    Thread.Sleep(1000);
                    return;
                }
                try
                {
                    FTPfile("BIMXchange Content", filequeue.FilePath);
                    FTPfile(@"inetpub/wwwroot/CADworks BIMXchange/Images/FileImages", filequeue.FilePath.Replace(".rfa", ".gif"));
                    filequeue.Status = Enums.IndexStatus.ContentUploaded.ToString();
                }
                catch (Exception)
                {
                    filequeue.Status = Enums.IndexStatus.ContentUploadFailed.ToString();
                }


                model.SaveChanges();
            }
        }

        public static void FTPfile(string ftpfilepath, string inputfilepath)
        {
            const string ftphost = "upload.mepcontent.com";
            //here correct hostname or IP of the ftp server to be given
            var parts = inputfilepath.Split('\\');
            var fileName = parts[parts.Count() - 1];
            var ftpfullpath = String.Format("ftp://{0}/{1}/{2}", ftphost, ftpfilepath, fileName);
            var ftp = (FtpWebRequest)WebRequest.Create(ftpfullpath);
            //ftp.Credentials = new NetworkCredential("userid", "password");
            //userid and password for the ftp server to given

            ftp.KeepAlive = true;
            ftp.UseBinary = true;
            ftp.Method = WebRequestMethods.Ftp.UploadFile;
            var fs = File.OpenRead(inputfilepath);
            var buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();
            var ftpstream = ftp.GetRequestStream();
            ftpstream.Write(buffer, 0, buffer.Length);
            ftpstream.Close();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            UploadNext();
        }


    }
}
