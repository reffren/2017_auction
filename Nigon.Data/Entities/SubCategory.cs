using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nigon.Data.Entities
{
    public class SubCategory
    {
        [Key]
        public int SubCategoryID { get; set; }

        [MaxLength(50)]
        public string SubCategoryUrl { get; set; }

        [MaxLength(50)]
        public string SubCategoryName { get; set; }
        public int CategoryID { get; set; }
        public virtual Category Category { get; set; }
    }
}