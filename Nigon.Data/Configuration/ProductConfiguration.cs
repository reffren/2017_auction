using Nigon.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Nigon.Data.Configuration
{
    public class ProductConfiguration : EntityTypeConfiguration<Product>
    {
        public ProductConfiguration()
        {
            HasKey(p => p.ProductID);
            Property(p => p.Name).HasMaxLength(150);
            Property(p => p.Description).HasMaxLength(500);
            Property(p => p.Price).HasPrecision(16, 2);
            Property(p => p.ImgPreview).HasMaxLength(150);
        }
    }
}