using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nigon.Domain.Entities
{
    public class SubCategory
    {
        public int SubCategoryID { get; set; }
        public int CategoryID { get; set; }
        public string SubCategoryUrl { get; set; }
        public string SubCategoryName { get; set; }

    }
}