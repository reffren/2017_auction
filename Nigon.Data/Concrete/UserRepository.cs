using Nigon.Data.Abstract;
using Nigon.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Nigon.Data.Concrete
{
    public class UserRepository : IUserRepository
    {
        EFContext context = new EFContext();

        IQueryable<User> IUserRepository.Users
        {
            get { return context.Users; }
        }
    }
}