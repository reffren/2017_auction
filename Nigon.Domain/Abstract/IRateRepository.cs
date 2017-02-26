using Nigon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nigon.Domain.Abstract
{
    public interface IRateRepository
    {
        IQueryable<Rate> Rates { get; }
        void SaveRate(Rate rate);
        void UptadeRate(Rate rate);
    }
}
