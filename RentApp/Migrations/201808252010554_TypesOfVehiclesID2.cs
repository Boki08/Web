namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TypesOfVehiclesID2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Vehicles", "TypeId", c => c.Int(nullable: false));
            CreateIndex("dbo.Vehicles", "TypeId");
            AddForeignKey("dbo.Vehicles", "TypeId", "dbo.TypeOfVehicles", "TypeId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Vehicles", "TypeId", "dbo.TypeOfVehicles");
            DropIndex("dbo.Vehicles", new[] { "TypeId" });
            DropColumn("dbo.Vehicles", "TypeId");
        }
    }
}
