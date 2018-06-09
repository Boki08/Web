namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserPass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AppUsers", "Password", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AppUsers", "Password");
        }
    }
}
