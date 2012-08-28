using System;
using System.Collections.Generic;
using System.Web.Mvc;

using CDWKS.BIMXchange.Web.Controllers.Base;
using CDWKS.BIMXchange.Web.Models;
using CDWKS.BIMXchange.Web.Models.Partial;
using CDWKS.BIMXchange.Web.Providers;
using CDWKS.Business.SearchManager;
using CDWKS.Shared.ObjectFactory;

using Ninject;

namespace CDWKS.BIMXchange.Web.Controllers
{
    public class HomeController : BaseController
    {
        #region Actions

        public ActionResult Index()
        {
            var manage = Construction.StandardKernel.Get<ISearchManager>();
            var model = new HomeViewModel { CurrentLibrary = "CADworks Electrical", Pagination = new PaginationViewModel() };

            #region Pagination

            if (HttpContext.Request.QueryString["page"] == null)
            {
                model.Pagination.CurrentQuery = HttpContext.Request.QueryString + "page=1";
                model.Pagination.CurrentPage = 1;
            }
            else
            {
                model.Pagination.CurrentPage = Int32.Parse(HttpContext.Request.QueryString["page"]);
                model.Pagination.CurrentQuery = HttpContext.Request.QueryString.ToString();

            }

            #endregion

            model.Items = new List<ItemSummaryViewModel>();

            //var searchResult = manage.Search("Drain", 5, model.Pagination.CurrentPage);

            //foreach (var result in searchResult.Results)
            //{
            //    var viewModel = new ItemSummaryViewModel();
            //    model.Items.Add(viewModel.Populate(result));
            //}

            //model.Pagination.TotalPages = (Int32)Math.Ceiling((double)searchResult.TotalCount / 5);

            return View("Index", model);
        }

        #endregion
        
        #region Methods

        public List<ItemSummaryViewModel> GetItems()
        {
            var result = new List<ItemSummaryViewModel>();
            for (var i = 1; i < 6; i++)
            {
                var item = new ItemSummaryViewModel
                {
                    FamilyName = "Joel's Family",
                    Name = "Type " + i,
                    HasTypeCatalog = i % 2 == 0,
                    FeaturedAttributes = new Dictionary<string, string> { { "rating", "cool" }, { "level", i.ToString() } }
                };
                result.Add(item);


            }

            return result;
        }

        #endregion
    }
}
