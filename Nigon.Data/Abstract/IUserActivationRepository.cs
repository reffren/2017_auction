using Nigon.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nigon.Data.Abstract
{
    public interface IUserActivationRepository
    {
        IQueryable<UserActivation> UserActivations { get; }
        void SaveUserActivation(UserActivation userActivation);
        void DeleteUserActivation(UserActivation userActivation);
    }
}
