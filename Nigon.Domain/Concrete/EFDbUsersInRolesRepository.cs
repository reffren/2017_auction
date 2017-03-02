using Nigon.Domain.Abstract;
using Nigon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Nigon.Domain.Concrete
{
    public class EFDbUsersInRolesRepository : DbContext, IUsersInRolesRepository
    {
        private EFDbContext context = new EFDbContext();
        public IQueryable<UsersInRoles> UsersInRoles
        {
            get { return context.UsersInRoles; }
        }
        public void AddUsersInRoles(UsersInRoles userInRoles)
        {
            context.UsersInRoles.Add(userInRoles);
            context.SaveChanges();
        }
    }
}