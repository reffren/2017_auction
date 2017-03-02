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
    public class ProductController : Controller
    {
        public int PageSize = 50;
        private IProductsRepository repository;
        private ISubCategoryRepository repositorySubCategory;
        private IProductViewRepository productView;
        private IImgProductRepository imgProducts;
        private ICategoryRepository repositoryCategory;
        private IRateRepository repositoryRate;
        private IUserRepository repositoryUser;

        ProductsListViewModel viewModel;
        ProductViewModel productViewModel;
        public ProductController(IProductsRepository productRepository, ISubCategoryRepository SubCategoryParam, IProductViewRepository productViewParam, IImgProductRepository imgProductsParam, ICategoryRepository categoryParam, IRateRepository rateParam, IUserRepository repositoryUserParam)
        {
            repository = productRepository;
            repositorySubCategory = SubCategoryParam;
            productView = productViewParam;
            imgProducts = imgProductsParam;
            repositoryCategory = categoryParam;
            repositoryRate = rateParam;
            repositoryUser = repositoryUserParam;
        }
        public void SearchStrings()
        {
            var CategoryList = new List<string>();

            var CategoryRequest = repositoryCategory.Categories.OrderBy(o => o.CategoryName).Select(s => s.CategoryName);
            CategoryList.AddRange(CategoryRequest.Distinct());
            ViewBag.category = new SelectList(CategoryList);
        }
        public ViewResult List(string subcategory, string searchCategory, string search, int page = 1)
        {
            int subCategoryID=0;
            string category="";

          //  SearchStrings();

            if (subcategory != null)
            {
                subCategoryID = repositorySubCategory.SubCategories.Where(w => w.SubCategoryUrl == subcategory).Select(s => s.SubCategoryID).Single();
                category = repositorySubCategory.SubCategories.Where(w => w.SubCategoryUrl == subcategory).Select(s => s.SubCategoryName).Single();
            }

                viewModel = new ProductsListViewModel();
                viewModel.Products = repository.Products.Where(p => subCategoryID == 0 || p.SubCategoryID == subCategoryID).OrderBy(p => p.ProductID).Skip((page - 1) * PageSize).Take(PageSize);
            if (!String.IsNullOrEmpty(search))
                viewModel.Products = repository.Products.Where(w => w.Name == search || w.Description == search).OrderBy(p => p.ProductID).Skip((page - 1) * PageSize).Take(PageSize);
            if (!String.IsNullOrEmpty(searchCategory))
            {
                int categoryID = repositoryCategory.Categories.Where(w => w.CategoryName == searchCategory).Select(s => s.CategoryID).Single();
            }
            viewModel.PagingInfo = new PagingInfo { CurrentPage = page, ItemsPerPage = PageSize, TotalItems = category == null ? repository.Products.Count() : repository.Products.Where(e => e.SubCategoryID == subCategoryID).Count() };
            viewModel.CurrentCategory = category;
            viewModel.subCategory = repositorySubCategory.SubCategories.ToList();
            return View(viewModel);
        }

        public ActionResult ProductView(int productID)
        {
            //SearchStrings();
            productViewModel = new ProductViewModel();
            productViewModel.products = repository.Products.Where(w => w.ProductID == productID).Single();
            productViewModel.productView = productView.ProductViews.Where(w => w.ProductViewID == productViewModel.products.ProductViewID).Single();
            productViewModel.ImgProducts = imgProducts.ImgProducts.Where(w => w.ProductID == productID);
            productViewModel.rate = repositoryRate.Rates.FirstOrDefault(f => f.RateID == productViewModel.productView.RateID);
            if (productViewModel.rate != null)
            productViewModel.Leader = repositoryUser.Users.FirstOrDefault(f => f.UserID == productViewModel.rate.UserID).UserName;
            if(productViewModel.rate == null)
            {
                productViewModel.rate = new Rate();
                productViewModel.rate.RateCount = 0;
                productViewModel.Leader = "-";
            }

            return View(productViewModel);
        }

        [HttpPost]
        [Authorize]
        public ActionResult ProductView(ProductViewModel prod) {

            decimal rate = prod.rate.SumRate;
            prod.rate.SumRate = rate;
            prod.rate.UserID = repositoryUser.Users.Where(w => w.UserName == User.Identity.Name).Select(s => s.UserID).Single();
            try
            {
                prod.rate.RateCount = repositoryRate.Rates.Where(w => w.ProductID == prod.rate.ProductID).Select(s => s.RateCount).First() + 1;
            }
            catch
            {
                prod.rate.RateCount = 1;
            }
            repositoryRate.SaveRate(prod.rate);
            prod.productView = productView.ProductViews.FirstOrDefault(f => f.ProductViewID == prod.productView.ProductViewID);
            prod.productView.RateID = repositoryRate.Rates.OrderByDescending(o => o.RateID).Select(s => s.RateID).First();
            productView.SaveProductView(prod.productView);

            return RedirectToAction("ProductView", "Product", new { prod.rate.ProductID }); 
        }
    }
}