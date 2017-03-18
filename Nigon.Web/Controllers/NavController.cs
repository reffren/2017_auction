using Nigon.Data.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nigon.Web.Controllers
{
    public class NavController : Controller
    {
        private ICategoryRepository _repositoryCategory;

        public NavController(ICategoryRepository categoryParam)
        {
            _repositoryCategory = categoryParam;

        }
        public PartialViewResult Menu(string subcategory = null)
        {
            ViewBag.SelectedCategory = subcategory;
            var category = _repositoryCategory.Categories.ToList();

            return PartialView(category);
        }
    }
}