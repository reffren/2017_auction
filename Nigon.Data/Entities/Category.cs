using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nigon.Data.Entities
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [MaxLength(100)]
        public string CategoryName { get; set; }
        public virtual ICollection<SubCategory> SubCategory { get; set; }
    }
}