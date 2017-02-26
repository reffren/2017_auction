using Nigon.Domain.Abstract;
using Nigon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Nigon.Domain.Concrete
{
    public class EFDbProductViewRepository : IProductViewRepository
    {
        private EFDbContext context = new EFDbContext();
        public IQueryable<ProductView> ProductViews
        {
            get { return context.ProductViews; }
        }


        public void SaveProductView(ProductView productView)
        {
            if (productView.ProductViewID == 0)
            {
                context.ProductViews.Add(productView);
            }
            else
            {
                context.Entry(productView).State = EntityState.Modified; //// Указать, что запись изменилась

            }
            context.SaveChanges();
        }

        public void DeleteProductView(ProductView productView)
        {
            context.ProductViews.Remove(productView);
            context.SaveChanges();
        }
    }
}