using Nigon.Data.Abstract;
using Nigon.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Nigon.Data.Concrete
{
    public class ProductViewRepository : IProductViewRepository
    {
        private EFContext context = new EFContext();
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
                context.Entry(productView).State = EntityState.Modified;

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