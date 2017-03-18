using Nigon.Data.Abstract;
using Nigon.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nigon.Data.Concrete
{
    public class CategoryRepository : ICategoryRepository
    {
        EFContext context = new EFContext();
        public IQueryable<Category> Categories
        {
            get { return context.Categories; }
        }
    }
}