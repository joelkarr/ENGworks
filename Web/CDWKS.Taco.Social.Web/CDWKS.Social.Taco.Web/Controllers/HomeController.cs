using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CDWKS.Social.Taco.Models;
using CDWKS.Social.Taco.UnitOfWork;
using Twitterizer;


namespace CDWKS.Social.Taco.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
           return View();
        }
        public class ProductFamily
        {
            public string product { get; set; }
            public string Name { get; set; }
        }

        [HttpPost]
        public ActionResult GetFamilies(string product)
        {
            var options = new List<ProductFamily>
                              {
                                  new ProductFamily {product = "Air Separators", Name = "4900"},
                                  new ProductFamily {product = "Air Separators", Name = "5900"},
                                  new ProductFamily {product = "Air Separators", Name = "ACT Tangential Air Separators"},
                                  new ProductFamily {product = "Air Separators", Name = "Air Separators (Tank Type)"},
                                  new ProductFamily {product = "Buffer Tanks", Name = "BTH Series"},
                                  new ProductFamily {product = "Buffer Tanks", Name = "BTL Series"},
                                  new ProductFamily {product = "Buffer Tanks", Name = "HBT Series"},
                                  new ProductFamily {product = "Expansion Tanks", Name = "CA Series"},
                                  new ProductFamily {product = "Expansion Tanks", Name = "CBX Series"},
                                  new ProductFamily {product = "Expansion Tanks", Name = "CX Series"},
                                  new ProductFamily {product = "Expansion Tanks", Name = "PAX Series"},
                                  new ProductFamily {product = "Expansion Tanks", Name = "Plain Steel"},
                                  new ProductFamily {product = "Heat Transfer Products", Name = "Plate and Frame"},
                                  new ProductFamily
                                      {
                                          product = "Heat Transfer Products",
                                          Name = "U and Straight Tube Heat Exchangers_Liquid to Liquid"
                                      },
                                  new ProductFamily
                                      {
                                          product = "Heat Transfer Products",
                                          Name = "U and Straight Tube Heat Exchangers_Steam to Steam"
                                      },
                                  new ProductFamily {product = "LoadMatch", Name = "Twin Tee"},
                                  new ProductFamily {product = "Systems", Name = "LOFlo"},
                                  new ProductFamily
                                      {
                                          product = "Base Mounted_Close Coupled Pumps",
                                          Name = "Frame-Mounted End Suction Pumps (FI Series)"
                                      },
                                  new ProductFamily
                                      {
                                          product = "Base Mounted_Close Coupled Pumps",
                                          Name = "Close-Coupled End Suction Pumps (CI Series)"
                                      },
                                  new ProductFamily {product = "Horizontal Split Case Pumps", Name = "GT Series"},
                                  new ProductFamily {product = "Horizontal Split Case Pumps", Name = "TA Series"},
                                  new ProductFamily {product = "In-Line Pumps", Name = "Series 1600"},
                                  new ProductFamily {product = "In-Line Pumps", Name = "Series 1900"},
                                  new ProductFamily
                                      {product = "Pump Accessories", Name = "Plus Two Multi-Purpose Valve_Angle"},
                                  new ProductFamily
                                      {product = "Pump Accessories", Name = "Plus Two Multi-Purpose Valve_Straight"},
                                  new ProductFamily {product = "Pump Accessories", Name = "Suction Diffuser (PSD)"},
                                  new ProductFamily {product = "Pump Accessories", Name = "Suction Diffuser (RSP)"},
                                  new ProductFamily
                                      {product = "Vertical Pumps", Name = "Vertical In-Line Pumps (KV Series)"},
                                  new ProductFamily
                                      {
                                          product = "Vertical Pumps",
                                          Name = "Vertical Split Coupled In-Line Pumps (KS Series)"
                                      },
                                  new ProductFamily
                                      {product = "Vertical Pumps", Name = "Vertical Split Case Pumps (TC Series)"},
                                  new ProductFamily
                                      {product = "Vertical Pumps", Name = "Vertical Turbine Pumps (VT Series)"}
                              };

            var selectedOptions = options.Where(f => f.product == product).ToList();
            var list = new object[selectedOptions.Count];
            var index = 0;
            foreach (var fam in selectedOptions)
            {
                list[index] = new {value = fam.Name, name = fam.Name};
                index++;
            }

            return Json(list);
        }

        [HttpPost]
        public ActionResult Index(SocialFeedbackForm model)
        {
            var context = new EFUnitOfWork();

            try
            {
                model.Timestamp = DateTime.Now;
                PostToTwitter(String.Format("{0} #TacoRevitFeedback", model.Comments));
                context.SocialFeedbackFormRespository.Add(model);
                context.Save();
            }
            finally
            {
                context.CloseConnection();
            }

            return RedirectToAction("Index", "Thanks");
        }
        private const string _consumerKey = "JolWl6IDqb8KPUr2apJS2Q";
        private const string _consumerSecret = "XKMIGgbQdXhObliyPCKDlOXMhwqVDzNHf9kDZhOVzE";
        private const string _accessToken = "465767808-68GW7SlJzpbtnNfr5shg3eSmjHP2EE3xwL5E7YI";
        private const string _accessTokenSecret = "Jxg5eOA52pKGFJTDG5Xf84vLhot0IBtXd8hebY8HZFY";
        private void PostToTwitter(string tweet)
        {
            var token = new OAuthTokens
                            {
                                AccessToken = _accessToken,
                                AccessTokenSecret = _accessTokenSecret,
                                ConsumerKey = _consumerKey,
                                ConsumerSecret = _consumerSecret
                            };
            var response = TwitterStatus.Update(token, tweet);
         
           
        }
    }
}
