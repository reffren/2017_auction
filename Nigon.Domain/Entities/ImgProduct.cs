using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nigon.Domain.Entities
{
    public class ImgProduct
    {
        public int ImgProductID { get; set; }
        public string PathImg { get; set; }
        public int ProductID { get; set; }
    }
}