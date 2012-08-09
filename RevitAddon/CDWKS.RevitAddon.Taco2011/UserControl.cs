using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using Autodesk.Revit.UI;
using Ionic.Zip;

namespace CDWKS.RevitAddon.Taco2011
{
    public partial class UserControl : Form
    {
        public UserControl(UIDocument doc)
        {
            InitializeComponent();
            Packages = new List<string>();
            RevitDoc = doc;
            webBrowser1.DocumentTitleChanged += webBrowser1_DocumentTitleChanged;
            var userName = Environment.UserDomainName + Environment.UserName;
            webBrowser1.Navigate("http://taco.mepcontent.com/?r=2011&u=" + userName);
        }

        public List<string> Packages { get; set; }
        public UIDocument RevitDoc { get; set; }

        private void webBrowser1_DocumentTitleChanged(object sender, EventArgs e)
        {
            var browser = (WebBrowser) sender;
            var url = browser.DocumentTitle;
            if (!url.Contains("dl=")) return;
            var parts = url.Split('=');
            var downloadFolder = parts[parts.Count() - 1];
            if (Packages.Contains(downloadFolder)) return;
            Packages.Add(downloadFolder);
            GetZipPackage(downloadFolder);
            UnZipPackage(downloadFolder);
            MessageBox.Show(LoadFamilies(downloadFolder));
        }


        public bool GetZipPackage(string downloadFolder)
        {
            var ENGworksDirectory = String.Format("{0}\\{1}",
                                                     Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                     "ENGworks");
            var saveDirectory = String.Format("{0}\\{1}\\{2}",
                                                 Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                 "ENGworks", downloadFolder);
            if (!Directory.Exists(ENGworksDirectory))
            {
                Directory.CreateDirectory(ENGworksDirectory);
            }

            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }
            var wb = new WebClient();
            var downloadPath = String.Format("{0}/{1}/{2}", @"http://download.mepcontent.com/CDS", downloadFolder,
                                                "package.zip");
            var savePath = String.Format("{0}\\{1}", saveDirectory, "package.zip");
            try
            {
                wb.DownloadFile(downloadPath, savePath);
                return true;
            }
            catch
            {
                MessageBox.Show("Unable to download file: " + downloadPath + " to " + savePath);
                return false;
            }
        }

        public bool UnZipPackage(string DownloadFolder)
        {
            try
            {
                var zipPath = String.Format("{0}\\{1}\\{2}",
                                               Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                               "ENGworks", DownloadFolder);
                const string zipName = "package.zip";
                using (var zip1 = ZipFile.Read(zipPath + "\\" + zipName))
                {
                    foreach (var e in zip1)
                    {
                        e.Extract(zipPath);
                    }
                }
                foreach (var file in Directory.GetFiles(zipPath, "*.rfa", SearchOption.AllDirectories).Where(file => file.Contains("%20")))
                {
                    try
                    {
                        File.Move(file, file.Replace("%20", " "));
                    }
                    catch (Exception)
                    {
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string LoadFamilies(string DownloadFolder)
        {
            try
            {
                foreach (
                    var family in
                        Directory.GetFiles(
                            String.Format("{0}\\{1}\\{2}",
                                          Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                          "ENGworks", DownloadFolder), "*.rfa", SearchOption.TopDirectoryOnly))
                {
                    label1.Text = "Loading " + family;
                    try
                    {
                        RevitDoc.Document.LoadFamily(family);
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                    var file = family.Split('\\');
                    return "Content Loaded: " + file[file.Count() - 1];
                }
            }
            catch
            {
            }
            return "Oops, something went wrong while loading content";
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void UserControl_Load(object sender, EventArgs e)
        {

        }
    }
}