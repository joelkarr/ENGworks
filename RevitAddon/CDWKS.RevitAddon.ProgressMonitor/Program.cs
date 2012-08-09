using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CDWKS.RevitAddOn.ProgressMonitor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var instanceId = args.Any() ? args[0] : Guid.NewGuid().ToString();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(new Guid(instanceId),"2012"));
        }
    }
}
