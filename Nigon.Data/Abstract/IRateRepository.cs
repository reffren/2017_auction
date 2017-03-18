using Nigon.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nigon.Data.Abstract
{
    public interface IRateRepository
    {
        IQueryable<Rate> Rates { get; }
        void SaveRate(Rate rate);
    }
}
