using System;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;

namespace CDWKS.RevitAddon.Indexer2012
{
    public partial class LoginForm : Form
    {
        public BasicHttpBinding Binding { get; set; }
        public EndpointAddress Endpoint { get; set; }
       
        public LoginForm()
        {
            InitializeComponent();
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
            Endpoint = new EndpointAddress(Constants.UserServiceURL);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using(var service = new UserServiceReference.UserServiceClient(Binding,Endpoint))
            {
                var password = "password";
                if (!String.IsNullOrEmpty(txtPass.Text))
                {
                    password = Shared.Security.Crypto.EncryptStringAES(txtPass.Text, "CADworks");
                }
                if(service.IsUserValid(txtUser.Text, password, "BXC_Admin"))
                {
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    lblError.Text = "UserName/Password does not match.";
                }
            }
        }
    }
}
