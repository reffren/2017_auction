using Nigon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nigon.WebUI.Models
{
    public class ProductsListViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public IEnumerable<SubCategory> subCategory { get; set; }
        public IEnumerable<ImgProduct> ImgProducts { get; set; }
        public string CurrentCategory { get; set; }
    }
}