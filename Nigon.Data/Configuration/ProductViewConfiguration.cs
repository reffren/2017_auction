using Nigon.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Nigon.Data.Configuration
{
    public class ProductViewConfiguration : EntityTypeConfiguration<ProductView>
    {
        public ProductViewConfiguration()
        {
            HasKey(p => p.ProductViewID);
            Property(p => p.StartPrice).HasPrecision(16, 2);
            Property(p => p.StepPrice).HasPrecision(16, 2);
            Property(p => p.StateOfProduct).HasMaxLength(100);
            Property(p => p.StartOfAuction).HasColumnType("datetime2");
            Property(p => p.EndOfAuction).HasColumnType("datetime2");
            Property(p => p.Location).HasMaxLength(100);
        }
    }
}