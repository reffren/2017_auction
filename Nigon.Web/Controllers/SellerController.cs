﻿using Nigon.Data.Abstract;
using Nigon.Data.Entities;
using Nigon.Web.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nigon.Web.Controllers
{
    [Authorize]
    public class SellerController : Controller
    {
        private IProductRepository _repositoryProducts;
        private IProductViewRepository _repositoryProductView;
        private IImgProductRepository _repositoryImgProducts;
        private IUserRepository _repositoryUser;
        private ISubCategoryRepository _repositorySubCategory;

        private ProductViewModel productModel;

        string nameOfPhoto = "";
        string PathImgPart = "~/Content/img_for_gallery/";
        string SmallImgPart = "~/Content/img_for_gallery/img_main_page/";

        public SellerController(IProductRepository productsParam, IProductViewRepository productViewParam, IImgProductRepository imgProductsParam, IUserRepository repositoryUserParam, ISubCategoryRepository SubCategoryParam)
        {
            _repositoryProducts = productsParam;
            _repositoryProductView = productViewParam;
            _repositoryImgProducts = imgProductsParam;
            _repositoryUser = repositoryUserParam;
            _repositorySubCategory = SubCategoryParam;
        }

        [HttpPost]

        public ActionResult Create(ProductViewModel productModel)
        {
            if (ModelState.IsValid)
            {
                _repositoryProductView.SaveProductView(productModel.Products.ProductView); //save or update data

                int productViewId = productModel.Products.ProductView.ProductViewID; // get ProductViewID for photo name and
                productModel.Products.ProductViewID = productViewId; //for initializing ProductViewID of Product Model
                productModel.Products.ProductView = null; //after get ProductViewID we drop ProductView model that avoid double record in ProductView table when we will save productModel.Products

                string filesImg = productModel.fileImg; //get names of images
                if (filesImg != null && filesImg != "undefined")
                {
                    string[] files = filesImg.Split('?'); // separate them
                    string imgMainPageName = files[0]; //get name of image for preview(first in array)

                    if (files != null && files.Count() < 8) // save names of images to the database(but no more than 8)
                        try
                        {
                            foreach (var file in files)
                            {
                                nameOfPhoto += file;

                                ImgProduct imgProduct = new ImgProduct
                                {
                                    PathImg = RenameFiles(nameOfPhoto, productViewId), //pass current name of image and id of ProductView to method RenameFiles (rename the file and add "id" in name of file)
                                    ProductViewID = productViewId
                                };
                                _repositoryImgProducts.SaveImage(imgProduct); // save name of image in database
                                nameOfPhoto = "";
                            }
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "Ошибка:" + ex.Message.ToString();
                            return View();
                        }
                    else
                    {
                        ViewBag.Message = "Вы загрузили неверный формат файла.";
                        return View();
                    }
                    productModel.Products.ImgPreview = RenameFiles(imgMainPageName, productViewId, "img_main_page"); //save the image of lot for main page, send to method name of file, "id" of ProductView and name of folder for preview image
                }
                productModel.Products.UserID = _repositoryUser.Users.Where(w => w.UserName == User.Identity.Name).Select(s => s.Id).Single();

                _repositoryProducts.SaveProduct(productModel.Products);
            }
            else
            {
                return View();
            }

            return RedirectToAction("ProductView", "Product", new { productModel.Products.ProductID });
        }

        public ViewResult Create()
        {
            productModel = new ProductViewModel();

            productModel.SabCategories = _repositorySubCategory.SubCategories.ToList(); //for dropdownlist

            return View(productModel);
        }

        public ViewResult Edit(string subCategory, int ProductId)
        {
            int ProductViewId = _repositoryProducts.Products.Where(w => w.ProductID == ProductId).Select(s => s.ProductViewID).First();
            productModel = new ProductViewModel();
            productModel.Products = _repositoryProducts.Products.FirstOrDefault(w => w.ProductID == ProductId);

            productModel.SabCategories = _repositorySubCategory.SubCategories.ToList(); //for dropdownlist
            if (productModel.Rate == null)
            {
                productModel.Rate = new Rate();
                productModel.Rate.RateCount = 0;
            }

            return View(productModel);
        }

        [Authorize]
        public ViewResult MyProducts()
        {
            var product = _repositoryProducts.Products.ToList();

            return View(product);
        }

        [HttpPost]
        public ActionResult Delete(int productId)
        {
            Product prod = _repositoryProducts.Products.FirstOrDefault(p => p.ProductID == productId);
            if (prod != null)
            {
                _repositoryProducts.DeleteProduct(prod);
                TempData["message"] = string.Format("{0} успешно удален", prod.Name);
            }
            return RedirectToAction("MyProducts");
        }

        [HttpPost]
        public ActionResult DeleteImage(int id)
        {
            ImgProduct imgProduct = _repositoryImgProducts.ImgProducts.FirstOrDefault(f => f.ImgProductID == id);
            _repositoryImgProducts.DeleteImgProduct(imgProduct);
            string path = imgProduct.PathImg;
            string fullPath = Request.MapPath(path);

            if (System.IO.File.Exists(fullPath))
            {
                try
                {
                    System.IO.File.Delete(fullPath);
                }
                catch (Exception e)
                {
                    throw new Exception("error with deleting image", e);
                }
            }

            return Content("Edit");
        }

        [HttpPost]
        public ActionResult Upload(string id)  //save images via AJAX
        {
            if (HttpContext.Request.Files.AllKeys.Any())
            {
                for (int i = 0; i <= HttpContext.Request.Files.Count; i++)
                {
                    var file = HttpContext.Request.Files["files" + i];
                    if (file != null)
                    {
                        var fileSavePath = Path.Combine(Server.MapPath(PathImgPart), file.FileName); // fo gallery
                        var ResizedFilePath = Path.Combine(Server.MapPath(SmallImgPart), file.FileName); //for preview on main page(preview)

                        try
                        {
                            file.SaveAs(fileSavePath); // save a image with quality 100%
                            file.InputStream.Close();
                            file.InputStream.Dispose();
                        }
                        catch (Exception e)
                        {
                            throw new Exception("error with saving image into gallery", e);
                        }

                        if (i == 0) //save first image in collection for the preview
                        {
                            Image img;
                            using (var bmpTemp = new Bitmap(fileSavePath)) // get free file locked (image isn't lock) 
                            {
                                img = new Bitmap(bmpTemp);
                            }
                            // save a image with quality 50%
                            SaveJpeg(ResizedFilePath, img, 50);
                        }
                    }
                }
            }
            return View("Create");
        }

        public static void SaveJpeg(string path, Image img, int quality) //Resize images
        {
            if (quality < 0 || quality > 100)
                throw new ArgumentOutOfRangeException("quality must be between 0 and 100.");

            // Encoder parameter for image quality 
            EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, quality);
            // JPEG image codec 
            ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;

            img.Save(path, jpegCodec, encoderParams);
        }

        /// <summary> 
        /// Returns the image codec with the given mime type 
        /// </summary> 
        private static ImageCodecInfo GetEncoderInfo(string mimeType) //Resize images
        {
            // Get image codecs for all image formats 
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec 
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];

            return null;
        }

        public string RenameFiles(string oldFileName, int id, string folder = "string")
        {
            string pathOldFileName;
            string pathNewFileName;

            if (folder != "string" && folder != null) //if the folder exist we add a renamed files to that folder
                PathImgPart = PathImgPart + folder + "/";

            Random random = new Random();
            int randomNumber = random.Next(0, 100);

            pathOldFileName = PathImgPart + oldFileName.Trim(); // the path to a file
            pathNewFileName = PathImgPart + id.ToString() + "-" + randomNumber.ToString() + oldFileName.Trim(); //add id ProductView, then dash(-) and random numbers that avoid the the same names

            string oldFile = Request.MapPath(pathOldFileName); // the whole path to a file
            string newFile = Request.MapPath(pathNewFileName); // the whole path to a file

            try
            {
                using (StreamReader reader = new StreamReader(@oldFile))
                {
                    System.IO.File.Copy(oldFile, newFile); //copy new file and assign it new name
                    reader.Close();
                }

            }
            catch (Exception e)
            {
                throw new Exception("Ошибка при загрузке файла, пожалуйста, перезагрузите страницу и попробуйте снова", e);
            }
            try
            {
                System.IO.File.Delete(oldFile); // delete old file

            }
            catch (Exception ex)
            {
                string filePath = Request.MapPath("~/Errors/"); // save exception in txt file

                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                       "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                }
            }
            return pathNewFileName;
        }
    }
}