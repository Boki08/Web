namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderOfficesId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "DepartureOfficeId", c => c.Int(nullable: false));
            AddColumn("dbo.Orders", "ReturnOfficeId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "ReturnOfficeId");
            DropColumn("dbo.Orders", "DepartureOfficeId");
        }
    }
}
