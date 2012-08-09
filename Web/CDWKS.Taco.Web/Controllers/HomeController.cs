using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using CDWKS.BXC.Taco.Web.Models;
using CDWKS.BXC.Taco.Web.UserService;
using User = CDWKS.BXC.Taco.Web.Models.User;

namespace CDWKS.BXC.Taco.Web.Controllers
{
    public class HomeController : Controller
    {
        public bool LoggedIn
        {
            get { return Session["User"] != null; }
            set { Session["User"] = value; }
        }
        public string Alias
        {
            get { return Session["Alias"] != null ? Session["Alias"].ToString() : String.Empty; }
            set { Session["Alias"] = value; }
        }
        //
        // GET: /Home/
        #region Actions
        public ActionResult Index(string input)
        {
            var model = new IndexViewModel
            {
                UserInput = { MinEff = 50, Motor = SyncSpeedOptions._60hz },
                IsLoggedIn = false
            };
            if(input == "login")
            {
                return View("Index", model);
            }
            if (input == "register")
            {
                model.Register = true;
                return View("Index", model);
            }
            if(!String.IsNullOrWhiteSpace(input))
            {
                Alias = input;
            }
            var isLoggedIn = LoggedIn || (!string.IsNullOrWhiteSpace(input) && CheckIfUserNameAliasExits(input));
            model.IsLoggedIn = isLoggedIn;
            return View("Index", model);
        }
        [HttpPost]
        public ActionResult Index(IndexViewModel model)
        {
            if (!model.Register && CheckLogin(model.Email, model.Password))
            {
                LoggedIn = true;
                return RedirectToAction("Index");
            }
            if(model.Register)
            {
                var user = new User
                               {
                                   Email = model.Email,
                                   FirstName = model.FirstName,
                                   LastName = model.LastName,
                                   Company = model.Company,
                                   PhoneNumber = model.PhoneNumber,
                                   Password = model.Password
                               };
                if(RegisterUser(user))
                {
                    LoggedIn = true;
                    model.IsLoggedIn = true;
                    model.Register = false;
                }
                else
                {
                    model.Register = true;
                    model.Message = "Email already registered.  Please Try Again.";
                    model.IsLoggedIn = false;
                }
                return View("Index", model);
            }
            model.Message = "Email/Password not found!";
            model.LoginFail = true;
            return View("Index", model);
        }

        public ActionResult Results(IndexViewModel inputModel)
        {
            var inputs =Request.QueryString.Count < 5 ? inputModel.UserInput  : CreateInputsFromQuery(Request.QueryString);
            var results = GetResultsFromTaco(inputs);
            if (inputs.MinEff == 0) inputs.MinEff = 50;
            var sb = new StringBuilder();
            foreach(var result in results.results)
            {
                sb.Append(result.model_no + ",");
            }
            var prodNumbers = sb.ToString();
            prodNumbers.TrimEnd(',');
            var ENGworksResponse = GetENGworksResponse(prodNumbers);
            foreach(var result in results.results)
            {
                var ENGworks = (from p in ENGworksResponse.Objects where p.model_no == result.model_no select p).FirstOrDefault();
                if (ENGworks != null && ENGworks.FoundInENGworksDB)
                {
                    result.PreviewImage = ENGworks.RevitFamilyName;
                    result.DownloadGuid = ENGworks.URL;
                    result.isENGworksAvailable = true;
                }
                else
                {
                    result.PreviewImage = "Preview-Not-Available";
                    result.isENGworksAvailable = false;
                }
                
                result.QueryString = inputs.ToQueryString;
                result.head = inputs.TotalHead.ToString();
            }
            var model = new ResultsViewModel { Pumps = results.results };
            
            return View("Results", model);
        }

        private UserInput CreateInputsFromQuery(NameValueCollection queryString)
        {
            var input = new UserInput();
            input.FlowRate = Decimal.Parse(queryString["FlowRate"]);
            input.TotalHead = Decimal.Parse(queryString["TotalHead"]);
            input.MinEff = Decimal.Parse(queryString["MinEff"]);
            input.PumpTypes = new List<PumpTypes>();
            input.PumpSpeeds = PumpSpeeds.All;
            input.EngineeringUnits = queryString["EngineeringUnits"] == "English" ? EngineeringUnits.English : EngineeringUnits.Metric;
            input.Motor = queryString["Motor"] == "_50hz" ? SyncSpeedOptions._50hz : SyncSpeedOptions._60hz;
            return input;
        }

        public ActionResult Item(Pump pump)
        {
            var model = new ItemViewModel { Pump = pump, QueryString = pump.QueryString };
            return View(model);
        }

        public ActionResult Help()
        {
            return View();
        }

        public ActionResult Eula()
        {
            return View();
        }
        #endregion  

        #region Methods

        #region User Helpers

        private bool CheckLogin(string email, string password)
        {
            using(var model =
                new BXC_MasterControlEntities(new Uri("http://odata.bxc.mepcontent.com/userservice.svc")))
            {
                var user = (from u in model.Users where u.UserName == email && u.Password == password select u).FirstOrDefault();
                if (user == null) return false;
                var alias = user.ExtendedProperties.FirstOrDefault(p => p.PropertyName.Name == "Alias");
                if(alias == null && !String.IsNullOrWhiteSpace(Alias))
                {
                    user.ExtendedProperties.Add(CreateProperty("Alias", Alias, model, user.Id));
                    model.SaveChanges();
                }
                return true;
            }
        }

        private bool CheckIfUserNameAliasExits(string alias)
        {
            using (var model =
                 new BXC_MasterControlEntities(new Uri("http://odata.bxc.mepcontent.com/userservice.svc")))
            {
                var property = (from p in model.ExtendedProperties
                            where p.PropertyName.Name == "Alias" && p.PropertyValue.Value==alias
                            select p).FirstOrDefault();
                return property != null;
            }
        }

        private bool RegisterUser(User user)
        {
            using (var model =
                new BXC_MasterControlEntities(new Uri("http://odata.bxc.mepcontent.com/userservice.svc")))
            {
                var alreadyused =
                    (from u in model.Users where u.UserName == user.Email select u).FirstOrDefault();
                if (alreadyused == null)
                {
                    var newUser = new UserService.User {UserName = user.Email, Password = user.Password};
                    model.AddObject("Users", newUser);
                    model.SaveChanges();
                    //TODO: encrypt password
                    model.AddObject("ExtendedProperties", CreateProperty("FirstName", user.FirstName, model, newUser.Id));
                    model.AddObject("ExtendedProperties", CreateProperty("LastName", user.LastName, model, newUser.Id));
                    model.AddObject("ExtendedProperties", CreateProperty("PhoneNumber", user.PhoneNumber, model, newUser.Id));
                    model.AddObject("ExtendedProperties", CreateProperty("Company", user.Company, model, newUser.Id));
                    if(!String.IsNullOrWhiteSpace(Alias))
                    {
                        model.AddObject("ExtendedProperties", CreateProperty("Alias", Alias, model, newUser.Id)); 
                    }
                    model.SaveChanges();
                }
                else
                {
                    return false;
                }
                return true;
            }
        }

        public static ExtendedProperty CreateProperty(string defName, string value, BXC_MasterControlEntities entity, int userId)
        {
            var prop = new ExtendedProperty {User_Id = userId};
            var name = (from n in entity.PropertyNames where n.Name == defName select n).FirstOrDefault();
            if (name != null)
            {
                prop.PropertyNames_Id = name.Id;
            }
            else
            {
                var n = new PropertyName {Name = defName};
                entity.AddObject("PropertyNames", n);
                entity.SaveChanges();
                prop.PropertyNames_Id = n.Id;
            }
            var propValue = (from v in entity.PropertyValues where v.Value == value select v).FirstOrDefault();
            if (propValue != null)
            {
                prop.PropertyValues_Id = propValue.Id;
            }
            else
            {
                var v = new PropertyValue {Value = value};
                entity.AddObject("Propertyvalues", v);
                entity.SaveChanges();
                prop.PropertyValues_Id = v.Id;
            }
            return prop;
        }

        #endregion

        #region API Helpers
        private ResponseCollectionObject GetENGworksResponse(string prodNumbers)
        {
            var request = WebRequest.Create(string.Format("http://connects.mepcontent.com/CDSService/collection/?domain=taco&pn={0}&apikey=bda11d91-7ade-4da1-855d-24adfe39d174", prodNumbers));
            var ws = request.GetResponse();
            var connectResponse = new StreamReader(ws.GetResponseStream()).ReadToEnd();
            var serializer = new JavaScriptSerializer();
            return serializer.Deserialize<ResponseCollectionObject>(connectResponse);
        }

        public string CurveImageURL(string curveImageId, string flow, string head)
        {
            #region Curve Image API
            //        API url: http://www.taco-hvac.com/siteapi/PumpWizard/curve_image.php
	
            //GET Variables:
            //    calltype = 'ENGworks'
            //    curve_image_id = id returned from search xml output
            //    flow = entered flow input
            //    head = entered head input
            //    img_type = 'thm' for thumbnail (125x75), 'med' for medium sized image (540x400), 'lg' full size image 
		
            //Output:
            //    Curve image JPEG format.  
            #endregion
            // this is what we are sending
            var post_data = "calltype=ENGworks&img_type='med'"
                + "&curve_image_id=" + curveImageId
                + "&head=" + head
                + "&flow=" + flow;

            const string uri = "http://www.taco-hvac.com/siteapi/PumpWizard/curve_image.php";

            return GetResponseFromAPI(uri, post_data);

        }

        public PumpResults GetResultsFromTaco(UserInput inputs)
        {
            #region API Notes
                //        API url: http://www.taco-hvac.com/siteapi/PumpWizard/index.php

                //POST Variables:
                //    calltype = 'ENGworks' this is to indicate the request is coming from you.
                //    display_thumbs = 'YES' to output curve image id, later called curve image call.
                //    fr = Flow rate
                //    h = head
                //    rpm = RPM: All, 1160, 1760, or 3500
                //    eff = efficiency percent, default is 50
                //    units = values: 0 = US, 1 = SI
                //    motor = values: 0 = 60hz, 1 = 50hz
                //    series = 'All' for every series, 'Custom' for choosing certain ones using fields below
                //        seriesMCI = value: 1 to select CI series
                //        seriesMFI = value: 1 to select FI series
                //        seriesMKV = value: 1 to select KV series
                //        seriesMKS = value: 1 to select KS series
                //        seriesMTA = value: 1 to select TA/GT series
                //        seriesMTC = value: 1 to select TC series
                //        seriesM1600 = value: 1 to select 1600 series
                //        seriesM1900 = value: 1 to select 1900 series
                //        seriesM2400 = value: 1 to select 2400 series
                //        seriesM100 = value: 1 to select 100 series
                //        seriesM00 = value: 1 to select 00 series
                //        seriesMLM = value: 1 to select Loadmatch series
            #endregion

            // this is what we are sending
            var post_data = "calltype=ENGworks&display_thumbs=YES&fr=" + inputs.FlowRate
                            + "&h=" + inputs.TotalHead
                            + "&rpm=" + inputs.PumpSpeeds.ToString().Replace("s",String.Empty)
                +"&eff=" +inputs.MinEff
                +"&units=" + (inputs.EngineeringUnits == EngineeringUnits.English ? 0:1)
                + "&motor=" + (inputs.Motor == SyncSpeedOptions._50hz ? 1:0)
                + "&series=All";

            // this is where we will send it
            const string uri = "http://www.taco-hvac.com/siteapi/PumpWizard/index.php";

            var results = GetResponseFromAPI(uri, post_data) ;
            if(results.Contains("Error"))
            {
                return new PumpResults();
            }
            results = results.Replace("\"", "'").Replace("\n", "").Replace("\t", "")
                .Replace("<results>", "<Pumps><results>").Replace("</results>", "</results></Pumps>");
            // Create an instance of the XmlSerializer specifying type and namespace.
            var serializer = new
            XmlSerializer(typeof(PumpResults));

            // A FileStream is needed to read the XML document.
            var byteArray = Encoding.ASCII.GetBytes( results );
            var stream = new MemoryStream( byteArray );

            // Declare an object variable of the type to be deserialized.

            // Use the Deserialize method to restore the object's state.
            return (PumpResults)serializer.Deserialize(stream);

        }

        private string GetResponseFromAPI(string uri, string post_data)
        {
            // create a request
            var request = (HttpWebRequest)
            WebRequest.Create(uri); request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";

            // turn our request string into a byte stream
            var postBytes = Encoding.ASCII.GetBytes(post_data);

            // this is important - make sure you specify type this way
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postBytes.Length;
            var requestStream = request.GetRequestStream();

            // now send it
            requestStream.Write(postBytes, 0, postBytes.Length);
            requestStream.Close();

            // grab the response and print it out to the console along with the status code
            request.Timeout = 5000;
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                return new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception)
            {
                return "Error";
            }

        }

        #endregion

        #endregion

    }
}
