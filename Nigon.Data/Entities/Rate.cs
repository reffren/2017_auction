using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nigon.Data.Entities
{
    public class Rate
    {
        public int RateID { get; set; }
        public decimal SumRate { get; set; }
        public int RateCount { get; set; }
        public string UserID { get; set; }
        public int ProductID { get; set; }
        public virtual Product Product { get; set; }
    }
}