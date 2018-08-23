namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RentServiceAdddedActivation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RentServices", "Activated", c => c.Boolean(nullable: false));
            AddColumn("dbo.RentServices", "ServiceEdited", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RentServices", "ServiceEdited");
            DropColumn("dbo.RentServices", "Activated");
        }
    }
}
