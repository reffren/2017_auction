using Nigon.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nigon.Domain.Concrete
{
    public class EFDbSubCategoryRepository : ISubCategoryRepository
    {
        EFDbContext context = new EFDbContext();

        public IQueryable<Entities.SubCategory> SubCategories
        {
            get { return context.SubCategories; }
        }
    }
}