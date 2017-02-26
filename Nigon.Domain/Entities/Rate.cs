using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nigon.Domain.Entities
{
    public class Rate
    {
        [Key]
        public int RateID { get; set; }
        public int ProductID { get; set; }
        public int UserID { get; set; }
        public decimal SumRate { get; set; }
        public int RateCount { get; set; }
    }
}