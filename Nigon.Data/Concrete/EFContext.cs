using Nigon.Data.Configuration;
using Nigon.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Nigon.Data.Concrete
{
    public class EFContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<UsersInRole> UsersInRoles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<ProductView> ProductViews { get; set; }
        public DbSet<ImgProduct> ImgProducts { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Rate> Rates { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.Add(new ProductViewConfiguration());
            modelBuilder.Configurations.Add(new ProductConfiguration());
            modelBuilder.Configurations.Add(new RateConfiguration());
        }
    }
}