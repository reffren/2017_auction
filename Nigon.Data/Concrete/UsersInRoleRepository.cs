using Nigon.Data.Abstract;
using Nigon.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Nigon.Data.Concrete
{
    public class UsersInRoleRepository : DbContext, IUsersInRoleRepository
    {
        EFContext context = new EFContext();
        public IQueryable<UsersInRole> UsersInRoles
        {
            get { return context.UsersInRoles; }
        }
        public void AddUsersInRoles(UsersInRole userInRoles)
        {
            context.UsersInRoles.Add(userInRoles);
            context.SaveChanges();
        }
    }
}