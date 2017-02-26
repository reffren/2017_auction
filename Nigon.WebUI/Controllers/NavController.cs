using Nigon.Domain.Abstract;
using Nigon.Domain.Entities;
using Nigon.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nigon.WebUI.Controllers
{
    public class NavController : Controller
    {
        private IProductsRepository repository;
        private ICategoryRepository categoryRepository;
        private ISubCategoryRepository subCategory;
        NavModel navModel;

        public NavController(IProductsRepository repo, ICategoryRepository categoryParam, ISubCategoryRepository subCategoryParam)
        {
            repository = repo;
            categoryRepository = categoryParam;
            subCategory = subCategoryParam;
        }
        public PartialViewResult Menu(string subcategory = null)
        {
            ViewBag.SelectedCategory = subcategory;
            navModel = new NavModel();
            navModel.category = categoryRepository.Categories;
            navModel.subCategory = subCategory.SubCategories;
            return PartialView(navModel);
        }
    }
}