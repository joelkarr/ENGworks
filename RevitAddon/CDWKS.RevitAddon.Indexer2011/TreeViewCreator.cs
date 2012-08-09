using System;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CDWKS.Model.Poco.Content;
using CDWKS.RevitAddon.Indexer2011.RevitFileServiceReference;

namespace CDWKS.RevitAddon.Indexer2011
{
    [Transaction(TransactionMode.Automatic)]
    [Regeneration(RegenerationOption.Manual)]
    public class TreeViewCreator : IExternalCommand
    {

        public BasicHttpBinding Binding { get; set; }
        public EndpointAddress Endpoint { get; set; }
        public string RevitVersion { get; set; }
        public string OwnerId { get; set; }
        public int LibraryId { get; set; }


        public void Initialize()
        {
            #region Binding

            Binding = new BasicHttpBinding(BasicHttpSecurityMode.None)
            {
                CloseTimeout = new TimeSpan(0, 0, 10, 0),
                OpenTimeout = new TimeSpan(0, 0, 10, 0),
                ReceiveTimeout = new TimeSpan(0, 0, 10, 0),
                SendTimeout = new TimeSpan(0, 0, 10, 0),
                AllowCookies = false,
                BypassProxyOnLocal = false,
                HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
                MaxBufferSize = 2147483647,
                MaxBufferPoolSize = 2147483647,
                MaxReceivedMessageSize = 2147483647,
                MessageEncoding = WSMessageEncoding.Text,
                TextEncoding = Encoding.UTF8,
                TransferMode = TransferMode.Streamed,
                UseDefaultWebProxy = true,
               
                ReaderQuotas =
                {
                    MaxDepth = 2147483647,
                    MaxStringContentLength = 2147483647,
                    MaxArrayLength = 2147483647,
                    MaxBytesPerRead = 2147483647,
                    MaxNameTableCharCount = 2147483647
                }
            };
            Binding.Security.Mode = BasicHttpSecurityMode.None;
            #endregion
            Endpoint = new EndpointAddress(Constants.WebserviceURL);
            RevitVersion = "2011";
            OwnerId = "1";
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            using (var login = new LoginForm())
            {
                login.ShowDialog();
                if (login.DialogResult == DialogResult.OK)
                {
                    Initialize();
                    var tree = CreateXMLFromDirectory();
                    var treePath = string.Format(
                        @"C:\ProgramData\Autodesk\Revit\Addins\2011\BIMXchangeAdmin.bundle\TreeArchive\tree_{0}_{1}.xml",
                        DateTime.Now.ToShortDateString().Replace(@"/", "").Replace(" ", ""),
                        DateTime.Now.ToShortTimeString().Replace(":", "").Replace("\\", "").Replace(" ", ""));

                    tree.Save(treePath);
                        UploadTreeToCloud(treePath);
                    MessageBox.Show("Tree View Updated!");
                }
            }
            return Result.Succeeded;
        }
        private void UploadTreeToCloud(string treePath)
        {
            FTPfile("TreeViews", treePath);
        }

        public XmlDocument  CreateXMLFromDirectory()
        {
            var doc = new XmlDocument();
            var tree = new XmlTree();
            //Get Library from Root Folder
            var fldBrowser = new FolderBrowserDialog { Description = "Select Root Directory to Sync with Tree" };
            var result = fldBrowser.ShowDialog();
            if (result == DialogResult.OK)
            {
                var library = fldBrowser.SelectedPath;
                var libName = GetLastFolder(library); 
                tree.Root = new Root{Name=libName};
                AddFolder(library, tree.Root);
                
            }
            var pathToXML = string.Format(@"C:\ProgramData\Autodesk\Revit\Addins\2011\BIMXchangeAdmin.bundle\TreeArchive\\tree_{0}_{1}.xml",
                                            DateTime.Now.ToShortDateString().Replace(@"/", "").Replace(" ", ""),
                                            DateTime.Now.ToShortTimeString().Replace(":", "").Replace("\\", "").Replace(
                                                " ", ""));
            var xmlSerializer = new XmlSerializer(typeof (XmlTree));
            var file = new StreamWriter(pathToXML);
            xmlSerializer.Serialize(file, tree);
            file.Close();
            doc.Load(pathToXML);
            return doc;


        }

        public Folder AddFolder(string folderPath, Folder folder)
        {
            var files = Directory.GetFiles(folderPath, "*.rfa");
            foreach (var file in files)
            {
                folder.Families.Add(new Model.Poco.Content.Family{Name=GetLastFolder(file.Replace(".rfa",string.Empty))});
            }
            var folders = Directory.GetDirectories(folderPath);
            foreach (var f in folders)
            {

                var nextFolder = new Folder {Name = GetLastFolder(f)};
                AddFolder(f,nextFolder);
                folder.Folders.Add(nextFolder);
            }
            return folder;
        }

        public string GetLastFolder(string path)
        {
            var pieces = path.Split('\\');
            return pieces[pieces.Count() - 1];
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

    }
}
