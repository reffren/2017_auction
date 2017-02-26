using Nigon.Domain.Abstract;
using Nigon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Nigon.Domain.Concrete
{
    public class EFDbRateRepository:IRateRepository
    {
        EFDbContext context = new EFDbContext();
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
                context.Entry(rate).State = EntityState.Modified; //// Указать, что запись изменилась

            }
            context.SaveChanges();
        }

        public void UptadeRate(Rate rate)
        {
            context.Entry(rate).State = EntityState.Modified;
            context.SaveChanges();
        }
    }
}