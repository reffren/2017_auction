using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nigon.Data.Entities
{
    public class UserActivation
    {
        [Key]
        public int UserActivationID { get; set; }
        public int UserID { get; set; }

        [MaxLength(100)]
        public string ActivationCode { get; set; }
    }
}