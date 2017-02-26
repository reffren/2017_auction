using Nigon.Domain.Abstract;
using Nigon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Nigon.Domain.Concrete
{
    public class EFDbImgProductRepository : IImgProductRepository
    {
        EFDbContext context = new EFDbContext();
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
                context.Entry(imgProduct).State = EntityState.Modified; //// Указать, что запись изменилась

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