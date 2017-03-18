using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nigon.Data.Entities
{
    public class ImgProduct
    {
        [Key]
        public int ImgProductID { get; set; }

        [MaxLength(100)]
        public string PathImg { get; set; }
        public int ProductViewID { get; set; }
        public virtual ProductView ProductView { get; set; }
    }
}