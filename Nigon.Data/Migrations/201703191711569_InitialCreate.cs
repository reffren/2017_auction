namespace Nigon.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryID = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.CategoryID);
            
            CreateTable(
                "dbo.SubCategories",
                c => new
                    {
                        SubCategoryID = c.Int(nullable: false, identity: true),
                        SubCategoryUrl = c.String(maxLength: 50),
                        SubCategoryName = c.String(maxLength: 50),
                        CategoryID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SubCategoryID)
                .ForeignKey("dbo.Categories", t => t.CategoryID, cascadeDelete: true)
                .Index(t => t.CategoryID);
            
            CreateTable(
                "dbo.ImgProducts",
                c => new
                    {
                        ImgProductID = c.Int(nullable: false, identity: true),
                        PathImg = c.String(maxLength: 100),
                        ProductViewID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ImgProductID)
                .ForeignKey("dbo.ProductViews", t => t.ProductViewID, cascadeDelete: true)
                .Index(t => t.ProductViewID);
            
            CreateTable(
                "dbo.ProductViews",
                c => new
                    {
                        ProductViewID = c.Int(nullable: false, identity: true),
                        StartPrice = c.Decimal(nullable: false, precision: 16, scale: 2),
                        StepPrice = c.Decimal(nullable: false, precision: 16, scale: 2),
                        StateOfProduct = c.String(nullable: false, maxLength: 100),
                        StartOfAuction = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        EndOfAuction = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Location = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.ProductViewID);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 150),
                        Description = c.String(nullable: false, maxLength: 500),
                        Price = c.Decimal(nullable: false, precision: 16, scale: 2),
                        ImgPreview = c.String(maxLength: 150),
                        UserID = c.Int(nullable: false),
                        ProductViewID = c.Int(nullable: false),
                        SubCategoryID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductID)
                .ForeignKey("dbo.ProductViews", t => t.ProductViewID, cascadeDelete: true)
                .ForeignKey("dbo.SubCategories", t => t.SubCategoryID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.ProductViewID)
                .Index(t => t.SubCategoryID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        UserName = c.String(maxLength: 50),
                        Password = c.String(maxLength: 50),
                        UserEmailAddress = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.UserID);
            
            CreateTable(
                "dbo.UsersInRoles",
                c => new
                    {
                        UsersInRolesID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        RoleID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UsersInRolesID)
                .ForeignKey("dbo.Roles", t => t.RoleID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.RoleID);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        RoleID = c.Int(nullable: false, identity: true),
                        RoleName = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.RoleID);
            
            CreateTable(
                "dbo.Rates",
                c => new
                    {
                        RateID = c.Int(nullable: false, identity: true),
                        SumRate = c.Decimal(nullable: false, precision: 16, scale: 2),
                        RateCount = c.Int(nullable: false),
                        UserID = c.Int(),
                        ProductID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RateID)
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserID)
                .Index(t => t.UserID)
                .Index(t => t.ProductID);
            
            CreateTable(
                "dbo.UserActivations",
                c => new
                    {
                        UserActivationID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        ActivationCode = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.UserActivationID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Rates", "UserID", "dbo.Users");
            DropForeignKey("dbo.Rates", "ProductID", "dbo.Products");
            DropForeignKey("dbo.Products", "UserID", "dbo.Users");
            DropForeignKey("dbo.UsersInRoles", "UserID", "dbo.Users");
            DropForeignKey("dbo.UsersInRoles", "RoleID", "dbo.Roles");
            DropForeignKey("dbo.Products", "SubCategoryID", "dbo.SubCategories");
            DropForeignKey("dbo.Products", "ProductViewID", "dbo.ProductViews");
            DropForeignKey("dbo.ImgProducts", "ProductViewID", "dbo.ProductViews");
            DropForeignKey("dbo.SubCategories", "CategoryID", "dbo.Categories");
            DropIndex("dbo.Rates", new[] { "ProductID" });
            DropIndex("dbo.Rates", new[] { "UserID" });
            DropIndex("dbo.UsersInRoles", new[] { "RoleID" });
            DropIndex("dbo.UsersInRoles", new[] { "UserID" });
            DropIndex("dbo.Products", new[] { "SubCategoryID" });
            DropIndex("dbo.Products", new[] { "ProductViewID" });
            DropIndex("dbo.Products", new[] { "UserID" });
            DropIndex("dbo.ImgProducts", new[] { "ProductViewID" });
            DropIndex("dbo.SubCategories", new[] { "CategoryID" });
            DropTable("dbo.UserActivations");
            DropTable("dbo.Rates");
            DropTable("dbo.Roles");
            DropTable("dbo.UsersInRoles");
            DropTable("dbo.Users");
            DropTable("dbo.Products");
            DropTable("dbo.ProductViews");
            DropTable("dbo.ImgProducts");
            DropTable("dbo.SubCategories");
            DropTable("dbo.Categories");
        }
    }
}
