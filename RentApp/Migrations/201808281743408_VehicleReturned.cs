namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VehicleReturned : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "VehicleReturned", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "VehicleReturned");
        }
    }
}
