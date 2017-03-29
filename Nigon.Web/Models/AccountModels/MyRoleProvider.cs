using Nigon.Data.Abstract;
using Nigon.Data.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Nigon.Web.Models.AccountModels
{
    public class MyRoleProvider : RoleProvider
    {
        //private IUserRepository _repositoryUser;
        public override bool IsUserInRole(string username, string roleName)
        {
            using (var usersContext = new EFContext())
            {
                var user = usersContext.Users.SingleOrDefault(u => u.UserName == username);
                if (user == null)
                    return false;
                return user.UsersInRoles != null && usersContext.UsersInRoles.Select(
                     u => u.Role).Any(r => r.RoleName == roleName);

            }
        }

        public override string[] GetRolesForUser(string username)
        {
            using (var usersContext = new EFContext())
            {
                var user = usersContext.Users.SingleOrDefault(u => u.UserName == username);
                if (user == null)
                    return new string[] { };
                return user.UsersInRoles == null ? new string[] { } : user.UsersInRoles.Select(u => u.Role).Select(u => u.RoleName).ToArray();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            //using (var usersContext = new EFContext())
            //{
            //    return usersContext.Roles.Select(r => r.RoleName).ToArray();
            //}
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName { get; set; }
    }
}