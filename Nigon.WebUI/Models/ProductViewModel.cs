using Nigon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nigon.WebUI.Models
{
    public class ProductViewModel
    {
        public ProductView productView {get; set;}
        public Product products { get; set; }
        public Category category { get; set; }
        public IEnumerable<SubCategory> subCategory { get; set; }
        public IEnumerable<ImgProduct> ImgProducts { get; set; }
        public Rate rate { get; set; }
        public string fileImg { get; set; }
        public string Leader { get; set; }
        public int SelectedOrderId { get; set; }

    }
}