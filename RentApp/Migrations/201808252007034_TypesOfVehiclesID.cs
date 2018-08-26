namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TypesOfVehiclesID : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TypeOfVehicles",
                c => new
                    {
                        TypeId = c.Int(nullable: false, identity: true),
                        Type = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.TypeId);
            
            AddColumn("dbo.Vehicles", "HourlyPrice", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Vehicles", "HourlyPrice");
            DropTable("dbo.TypeOfVehicles");
        }
    }
}
