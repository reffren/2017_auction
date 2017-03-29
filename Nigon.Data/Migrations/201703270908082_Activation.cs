namespace Nigon.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Activation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserActivations", "UserName", c => c.String(maxLength: 50));
            AddColumn("dbo.UserActivations", "Password", c => c.String(maxLength: 50));
            AddColumn("dbo.UserActivations", "UserEmailAddress", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserActivations", "UserEmailAddress");
            DropColumn("dbo.UserActivations", "Password");
            DropColumn("dbo.UserActivations", "UserName");
        }
    }
}
