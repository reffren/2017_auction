using Nigon.Data.Abstract;
using Nigon.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Nigon.Data.Concrete
{
    public class UserRepository : DbContext, IUserRepository
    {
        EFContext context = new EFContext();
        public void AddUser(User user)
        {
            context.Users.Add(user);
            context.SaveChanges();
        }

        public User GetUser(string userName)
        {
            var user = context.Users.SingleOrDefault(u => u.UserName == userName);
            return user;
        }

        public User GetUser(string userName, string password)
        {
            var user = context.Users.SingleOrDefault(u => u.UserName == userName && u.Password == password);
            return user;
        }
        IQueryable<User> IUserRepository.Users
        {
            get { return context.Users; }
        }
    }
}