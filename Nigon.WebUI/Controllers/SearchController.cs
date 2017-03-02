using Nigon.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nigon.WebUI.Controllers
{
    public class SearchController : Controller
    {
        private ICategoryRepository categoryRepository;

        public SearchController(ICategoryRepository catRepositoryParam)
        {
            categoryRepository = catRepositoryParam;
        }
        public PartialViewResult Index()
        {
            var CategoryList = new List<string>();
            var CategoryRequest = categoryRepository.Categories.OrderBy(o => o.CategoryName).Select(s => s.CategoryName);
            CategoryList.AddRange(CategoryRequest.Distinct());
            return PartialView(CategoryList);
        }
	}
}