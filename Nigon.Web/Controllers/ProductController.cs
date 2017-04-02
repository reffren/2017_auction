using Nigon.Data.Abstract;
using Nigon.Data.Entities;
using Nigon.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nigon.Web.Controllers
{
    public class ProductController : Controller
    {
        public int PageSize = 50;
        private IProductRepository _repositoryProducts;
        private ICategoryRepository _repositoryCategory;
        private IRateRepository _repositoryRate;
        private IUserRepository _repositoryUser;

        ProductsListViewModel viewModel;
        ProductViewModel productViewModel;
        public ProductController(IProductRepository productRepository, ICategoryRepository categoryParam, IRateRepository rateParam, IUserRepository repositoryUserParam)
        {
            _repositoryProducts = productRepository;
            _repositoryCategory = categoryParam;
            _repositoryRate = rateParam;
            _repositoryUser = repositoryUserParam;
        }
        public void SearchStrings()
        {
            var CategoryList = new List<string>();

            var CategoryRequest = _repositoryCategory.Categories.OrderBy(o => o.CategoryName).Select(s => s.CategoryName);
            CategoryList.AddRange(CategoryRequest.Distinct());
            ViewBag.category = new SelectList(CategoryList);
        }
        public ViewResult List(string subcategory, string searchCategory, string search, int page = 1)
        {

            viewModel = new ProductsListViewModel();

            viewModel.Products = _repositoryProducts.Products.Where(p => subcategory == null || p.SubCategory.SubCategoryUrl == subcategory).OrderBy(p => p.ProductID).Skip((page - 1) * PageSize).Take(PageSize).ToList();

            if (!String.IsNullOrEmpty(search))
                viewModel.Products = _repositoryProducts.Products.Where(w => w.Name == search || w.Description == search).OrderBy(p => p.ProductID).Skip((page - 1) * PageSize).Take(PageSize);
            if (!String.IsNullOrEmpty(searchCategory))
            {
                int categoryID = _repositoryCategory.Categories.Where(w => w.CategoryName == searchCategory).Select(s => s.CategoryID).Single();
            }
            viewModel.PagingInfo = new PagingInfo { CurrentPage = page, ItemsPerPage = PageSize, TotalItems = subcategory == null ? _repositoryProducts.Products.Count() : _repositoryProducts.Products.Where(e => e.SubCategory.SubCategoryUrl == subcategory).Count() };
            viewModel.CurrentCategory = subcategory;

            return View(viewModel);
        }

        public ActionResult ProductView(int productID)
        {
            productViewModel = new ProductViewModel();
            productViewModel.Products = _repositoryProducts.Products.FirstOrDefault(w => w.ProductID == productID);
            productViewModel.Rate = _repositoryRate.Rates.FirstOrDefault(f => f.ProductID == productViewModel.Products.ProductID);
            if (productViewModel.Rate != null)
                productViewModel.Leader = _repositoryUser.Users.FirstOrDefault(user => user.Id == productViewModel.Rate.UserID).UserName;
            if (productViewModel.Rate == null)
            {
                productViewModel.Rate = new Rate();
                productViewModel.Rate.RateCount = 0;
                productViewModel.Leader = "-";
            }

            return View(productViewModel);
        }

        [HttpPost]
        [Authorize]
        public ActionResult ProductView(ProductViewModel prod)
        {
            if (ModelState.IsValid)
            {
                prod.Rate.UserID = _repositoryUser.Users.Where(w => w.UserName == User.Identity.Name).Select(user => user.Id).Single();
                prod.Rate.RateCount = _repositoryRate.Rates.Where(w => w.ProductID == prod.Rate.ProductID).Select(s => s.RateCount).FirstOrDefault() + 1;
                _repositoryRate.SaveRate(prod.Rate);
            }
            else
            {
                return View();
            }
            return RedirectToAction("ProductView", "Product", new { prod.Rate.ProductID });
        }
    }
}