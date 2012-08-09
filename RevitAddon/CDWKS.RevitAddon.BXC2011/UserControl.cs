using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using Autodesk.Revit.UI;
using Ionic.Zip;

namespace CDWKS.RevitAddon.BXC2011
{
    public partial class UserControl : Form
    {
        public UserControl(UIDocument Doc)
        {
            InitializeComponent();
            Packages = new List<string>();
            RevitDoc = Doc;
            webBrowser1.DocumentTitleChanged += webBrowser1_DocumentTitleChanged;
            string UserName = Environment.UserDomainName + Environment.UserName;
            webBrowser1.Navigate("http://bxc.mepcontent.com/?r=1&u=" + UserName);
        }

        public List<string> Packages { get; set; }
        public UIDocument RevitDoc { get; set; }

        private void webBrowser1_DocumentTitleChanged(object sender, EventArgs e)
        {
            var browser = (WebBrowser) sender;
            string url = browser.DocumentTitle;
            if (url.Contains("dl="))
            {
                string[] parts = url.Split('=');
                string downloadFolder = parts[parts.Count() - 1];
                if (!Packages.Contains(downloadFolder))
                {
                    Packages.Add(downloadFolder);
                    GetZipPackage(downloadFolder);
                    UnZipPackage(downloadFolder);
                    MessageBox.Show(LoadFamilies(downloadFolder));
                }
            }
        }


        public bool GetZipPackage(string DownloadFolder)
        {
            string ENGworksDirectory = String.Format("{0}\\{1}",
                                                     Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                     "ENGworks");
            string SaveDirectory = String.Format("{0}\\{1}\\{2}",
                                                 Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                 "ENGworks", DownloadFolder);
            if (!Directory.Exists(ENGworksDirectory))
            {
                Directory.CreateDirectory(ENGworksDirectory);
            }

            if (!Directory.Exists(SaveDirectory))
            {
                Directory.CreateDirectory(SaveDirectory);
            }
            var wb = new WebClient();
            string downloadPath = String.Format("{0}/{1}/{2}", @"http://download.mepcontent.com", DownloadFolder,
                                                "package.zip");
            string savePath = String.Format("{0}\\{1}", SaveDirectory, "package.zip");
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
                string zipPath = String.Format("{0}\\{1}\\{2}",
                                               Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                               "ENGworks", DownloadFolder);
                string zipName = "package.zip";
                using (ZipFile zip1 = ZipFile.Read(zipPath + "\\" + zipName))
                {
                    foreach (ZipEntry e in zip1)
                    {
                        e.Extract(zipPath);
                    }
                }
                foreach (string file in Directory.GetFiles(zipPath, "*.rfa", SearchOption.AllDirectories))
                {
                    if (file.Contains("%20"))
                    {
                        try
                        {
                            File.Move(file, file.Replace("%20", " "));
                        }
                        catch
                        {
                        }
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
                    string family in
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
                    string[] file = family.Split('\\');
                    return "Content Loaded: " + file[file.Count() - 1];
                }
            }
            catch
            {
            }
            return "Oops, something went wrong while loading content";
        }
    }
}