using Nigon.Domain.Abstract;
using Nigon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nigon.Domain.Concrete
{
    public class EFDbRolesRepository : IRolesRepository
    {
        private EFDbContext context = new EFDbContext();
        public IQueryable<Role> Roles
        {
            get { return context.Roles; }
        }
    }
}