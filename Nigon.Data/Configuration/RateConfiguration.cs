using Nigon.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Nigon.Data.Configuration
{
    public class RateConfiguration : EntityTypeConfiguration<Rate>
    {
        public RateConfiguration()
        {
            HasKey(p => p.RateID);
            Property(p => p.SumRate).HasPrecision(16, 2);
        }
    }
}