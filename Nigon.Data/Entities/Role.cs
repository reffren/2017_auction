using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nigon.Data.Entities
{
    public class Role
    {
        [Key]
        public int RoleID { get; set; }

        [MaxLength(20)]
        public string RoleName { get; set; }
    }
}