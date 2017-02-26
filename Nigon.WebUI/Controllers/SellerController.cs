using Nigon.Domain.Abstract;
using Nigon.Domain.Entities;
using Nigon.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Nigon.WebUI.Controllers
{
    [Authorize]
    public class SellerController : Controller
    {
        private IProductsRepository repositoryProducts;
        private IProductViewRepository repositoryProductView;
        private IImgProductRepository repositoryImgProducts;
        private IUserRepository repositoryUser;
        private ProductViewModel productModel;
        private ISubCategoryRepository repositorySubCategory;
        private IRateRepository repositoryRate;

        string nameOfPhoto = "";
        string PathImgPart = "~/Content/img_for_gallery/";
        string SmallImgPart = "~/Content/img_for_gallery/img_main_page/";

        public SellerController(IProductsRepository productsParam, IProductViewRepository productViewParam, IImgProductRepository imgProductsParam, IUserRepository repositoryUserParam, ISubCategoryRepository SubCategoryParam, IRateRepository rateParam)
        {
            repositoryProducts = productsParam;
            repositoryProductView = productViewParam;
            repositoryImgProducts = imgProductsParam;
            repositoryUser = repositoryUserParam;
            repositorySubCategory = SubCategoryParam;
            repositoryRate = rateParam;
        }

        [HttpPost]

        public ActionResult Create(ProductViewModel productModel)
        {
            repositoryProductView.SaveProductView(productModel.productView);

            int productViewId = repositoryProductView.ProductViews.OrderByDescending(p => p.ProductViewID).Select(s => s.ProductViewID).First(); // после записи данных в таблицу ProductViews извлекаем из нее ProductViewID и записываем в таблицу Products (для совпадения ProductViewID в обоих таблицах)
            productModel.products.ProductViewID = productViewId;
            productModel.products.SubCategoryID = productModel.SelectedOrderId; //сохраняем выбранную подкатегорию из dropdownlist
            productModel.products.UserID = repositoryUser.Users.Where(w => w.UserName == User.Identity.Name).Select(s => s.UserID).Single();
            repositoryProducts.SaveProduct(productModel.products);

            string filesImg = productModel.fileImg; //получаем имена файлов
            if (filesImg != null && filesImg != "undefined")
            {
                string[] files = filesImg.Split('?'); // разделяем их
                string imgMainPageName = files[0]; //извлекаем имя файла для превью (первый в массиве)

                if (files != null && files.Count() < 8) // и сохраняем имена в бд(но не больше 8)
                    try
                    {
                        foreach (var file in files)
                        {
                            nameOfPhoto += file;

                            ImgProduct imgProduct = new ImgProduct
                            {
                                PathImg = RenameFiles(nameOfPhoto, productViewId), //передаем текущее имя файла и id ProductView в метод RenameFiles (переименовываем файл, добавляя id в имя файла)
                                ProductID = productModel.products.ProductID

                            };
                            repositoryImgProducts.SaveImage(imgProduct); // сохраняем имя файла изображения в бд
                            nameOfPhoto = "";

                        }
                        ViewBag.Message = "Ваше изображение успешно загружено.";
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "Ошибка:" + ex.Message.ToString();
                    }
                else
                {
                    ViewBag.Message = "Вы загрузили неверный формат файла.";
                }
                productModel.products.ImgPreview = RenameFiles(imgMainPageName, productViewId, "img_main_page"); //сохраняем картинку лота для главной страницы, передаем в метод имя файла, id ProductView и имя папки для файла превью
            }
            repositoryProducts.SaveProduct(productModel.products);

            productModel.ImgProducts = repositoryImgProducts.ImgProducts.Where(w => w.ProductID == productModel.products.ProductID); // загружаем фоты в модель для последующего отображения в окне создания товара

            return RedirectToAction("ProductView", "Product", new { productModel.products.ProductID });
        }
            
        public ViewResult Create()
        {
            productModel = new ProductViewModel();
            productModel.subCategory = repositorySubCategory.SubCategories; //for dropdownlist

            return View(productModel);
        }

        public ViewResult Edit(string subCategory, int ProductId)
        {
            int ProductViewId = repositoryProducts.Products.Where(w => w.ProductID == ProductId).Select(s => s.ProductViewID).First();
            productModel = new ProductViewModel();
            productModel.products = repositoryProducts.Products.FirstOrDefault(w => w.ProductID == ProductId);
            productModel.ImgProducts = repositoryImgProducts.ImgProducts.Where(w => w.ProductID == ProductId);
            productModel.productView = repositoryProductView.ProductViews.FirstOrDefault(w => w.ProductViewID == ProductViewId);
            productModel.rate = repositoryRate.Rates.FirstOrDefault(f => f.ProductID == ProductId);
            productModel.subCategory = repositorySubCategory.SubCategories; //for dropdownlist
            if (productModel.rate == null)
            {
                productModel.rate = new Rate();
                productModel.rate.RateCount = 0;
            }

            return View(productModel);
        }

        public ViewResult MyProducts()
        {
            int userId = repositoryUser.Users.Where(w => w.UserName == User.Identity.Name).Select(s => s.UserID).Single();
            ProductsListViewModel listView = new ProductsListViewModel();
            listView.Products = repositoryProducts.Products.Where(w => w.UserID == userId);
            listView.subCategory = repositorySubCategory.SubCategories.ToList();
            return View(listView);
        }

        [HttpPost]
        public ActionResult Delete(int productId)
        {
            Product prod = repositoryProducts.Products.FirstOrDefault(p => p.ProductID == productId);
            if (prod != null)
            {
                repositoryProducts.DeleteProduct(prod);
                TempData["message"] = string.Format("{0} успешно удален", prod.Name);
            }
            return RedirectToAction("MyProducts");
        }

        [HttpPost]
        public ActionResult DeleteImage(int id)
        {
            ImgProduct imgProduct = repositoryImgProducts.ImgProducts.FirstOrDefault(f => f.ImgProductID == id);
            repositoryImgProducts.DeleteImgProduct(imgProduct);
            string path = imgProduct.PathImg;
            string fullPath = Request.MapPath(path);
            try
            {
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            catch
            {
            }
            return Content("Edit");
        }

        [HttpPost]
        public ActionResult Upload(string id)  //сохраняем файлы изображений с помощью AJAX
        {
            if (HttpContext.Request.Files.AllKeys.Any())
            {
                for (int i = 0; i <= HttpContext.Request.Files.Count; i++)
                {
                    var file = HttpContext.Request.Files["files" + i];
                    if (file != null)
                    {
                        var fileSavePath = Path.Combine(Server.MapPath(PathImgPart), file.FileName); // для галереи
                        var ResizedFilePath = Path.Combine(Server.MapPath(SmallImgPart), file.FileName); //для превью главной страницы (preview)

                        try
                        {
                            file.SaveAs(fileSavePath); // Сохраняем картинку со 100% качеством
                            file.InputStream.Close();
                            file.InputStream.Dispose();
                        }
                        catch { }

                        if (i == 0) //сохраняем в превью первое изображение в коллекции
                        {
                            Image img;
                            using (var bmpTemp = new Bitmap(fileSavePath)) // получаем free file locked (изображение не занято процессом) 
                            {                                             
                                img = new Bitmap(bmpTemp);
                            }
                            // Сохраняем картинку с качеством 50%
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

            if (folder != "string" && folder != null) //если папка существует, то добавляем переименованные файлы в эту папку
                PathImgPart = PathImgPart + folder + "/";

            Random random = new Random();
            int randomNumber = random.Next(0, 100);

            pathOldFileName = PathImgPart + oldFileName.Trim(); //  путь к файлу
            pathNewFileName = PathImgPart + id.ToString() + "-" + randomNumber.ToString() + oldFileName.Trim(); //добавляем id ProductView, тире и рандомные цифры к именю файла, дабы избежать одинаковых имен
          
            string oldFile = Request.MapPath(pathOldFileName); // полный путь к файлу
            string newFile = Request.MapPath(pathNewFileName); // полный путь к файлу

            try
            {
                using (StreamReader reader = new StreamReader(@oldFile))
                {
                    System.IO.File.Copy(oldFile, newFile); //копируем старый файл и присваиваем ему новое имя
                    reader.Close();
                }
                        
            }
            catch { }
            try
            {
                System.IO.File.Delete(oldFile); //и удаляем старый файл

            }
            catch { } 
            return pathNewFileName;
        }
    }
}