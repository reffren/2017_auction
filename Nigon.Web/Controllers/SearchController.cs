using Nigon.Data.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nigon.Web.Controllers
{
    public class SearchController : Controller
    {
        private ICategoryRepository _repositoryCategory;

        public SearchController(ICategoryRepository catRepositoryParam)
        {
            _repositoryCategory = catRepositoryParam;
        }
        public PartialViewResult Index()
        {
            var CategoryList = new List<string>();
            var CategoryRequest = _repositoryCategory.Categories.OrderBy(o => o.CategoryName).Select(s => s.CategoryName);
            CategoryList.AddRange(CategoryRequest.Distinct());
            return PartialView(CategoryList);
        }
    }
}