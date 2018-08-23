namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserChanges3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AppUsers", "ProfileEdited", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AppUsers", "ProfileEdited");
        }
    }
}
