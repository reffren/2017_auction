using Nigon.Data.Abstract;
using Nigon.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nigon.Data.Concrete
{
    public class SubCategoryRepository : ISubCategoryRepository
    {
        EFContext context = new EFContext();
        public IQueryable<SubCategory> SubCategories
        {
            get { return context.SubCategories; }
        }
    }
}