using Nigon.Data.Abstract;
using Nigon.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Nigon.Data.Concrete
{
    public class ImgProductRepository : IImgProductRepository
    {
        EFContext context = new EFContext();
        public IQueryable<Entities.ImgProduct> ImgProducts
        {
            get { return context.ImgProducts; }
        }

        public void SaveImage(ImgProduct imgProduct)
        {
            if (imgProduct.ImgProductID == 0)
            {
                context.ImgProducts.Add(imgProduct);
            }
            else
            {
                context.Entry(imgProduct).State = EntityState.Modified; 

            }
            context.SaveChanges();
        }
        public void DeleteImgProduct(ImgProduct imgProduct)
        {
            context.ImgProducts.Remove(imgProduct);
            context.SaveChanges();
        }
    }
}