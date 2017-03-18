using Nigon.Data.Abstract;
using Nigon.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Nigon.Data.Concrete
{
    public class RateRepository : IRateRepository
    {
        EFContext context = new EFContext();
        public IQueryable<Rate> Rates
        {
            get { return context.Rates; }
        }

        public void SaveRate(Rate rate)
        {
            if (rate.RateID == 0)
            {
                context.Rates.Add(rate);
            }
            else
            {
                context.Entry(rate).State = EntityState.Modified;
            }
            context.SaveChanges();
        }
    }
}