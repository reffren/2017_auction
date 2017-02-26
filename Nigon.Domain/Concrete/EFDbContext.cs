using Nigon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Nigon.Domain.Concrete
{
    public class EFDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<UsersInRoles> UsersInRoles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<ProductView> ProductViews { get; set; }
        public DbSet<ImgProduct> ImgProducts { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Rate> Rates { get; set; }
    }
}