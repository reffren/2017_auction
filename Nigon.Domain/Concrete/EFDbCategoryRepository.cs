using Nigon.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nigon.Domain.Concrete
{
    public class EFDbCategoryRepository : ICategoryRepository
    {
        EFDbContext context = new EFDbContext();
        public IQueryable<Entities.Category> Categories
        {
            get { return context.Categories; }
        }
    }
}