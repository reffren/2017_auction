using Nigon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nigon.Domain.Abstract
{
    public interface IUserRepository
    {
        void AddUser(User user);
        User GetUser(string userName);
        User GetUser(string userName, string password);
        IQueryable<User> Users { get; }
    }
}
