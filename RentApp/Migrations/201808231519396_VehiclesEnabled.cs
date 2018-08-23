namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VehiclesEnabled : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Vehicles", "Enabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Vehicles", "Enabled");
        }
    }
}
