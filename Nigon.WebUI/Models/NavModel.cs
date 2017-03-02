using Nigon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nigon.WebUI.Models
{
    public class NavModel
    {
        public IEnumerable<Category> category {get; set;}
        public IEnumerable<SubCategory> subCategory { get; set; }
    }
}