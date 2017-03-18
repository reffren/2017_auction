using Nigon.Data.Abstract;
using Nigon.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nigon.Data.Concrete
{
    public class RoleRepository : IRoleRepository
    {
        private EFContext context = new EFContext();
        public IQueryable<Role> Roles
        {
            get { return context.Roles; }
        }
    }
}