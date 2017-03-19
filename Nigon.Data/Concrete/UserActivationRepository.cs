using Nigon.Data.Abstract;
using Nigon.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nigon.Data.Concrete
{
    public class UserActivationRepository : IUserActivationRepository
    {
        EFContext context = new EFContext();
        public IQueryable<UserActivation> UserActivations
        {
            get { return context.UserActivations; }
        }
        public void SaveUserActivation(UserActivation userActivation)
        {
            if (userActivation != null)
                context.UserActivations.Add(userActivation);
            context.SaveChanges();
        }

        public void DeleteUserActivation(UserActivation userActivation)
        {
            if (userActivation != null)
                context.UserActivations.Remove(userActivation);
            context.SaveChanges();
        }
    }
}