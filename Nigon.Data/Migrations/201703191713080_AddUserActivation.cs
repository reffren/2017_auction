namespace Nigon.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddUserActivation : DbMigration
    {
        public override void Up()
        {
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
        }
    }
}
