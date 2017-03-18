using Nigon.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nigon.Web.Models
{
    public class ProductViewModel
    {
        public Product Products { get; set; }
        public IEnumerable<SubCategory> SabCategories { get; set; }
        public Rate Rate { get; set; }
        public string fileImg { get; set; }
        public string Leader { get; set; }

    }
}