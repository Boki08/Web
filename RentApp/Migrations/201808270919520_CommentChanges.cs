namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommentChanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Components", "VehicleId", "dbo.Vehicles");
            DropForeignKey("dbo.Components", "Pricing_PricingId", "dbo.Pricings");
            DropForeignKey("dbo.Pricings", "RentServiceId", "dbo.RentServices");
            DropForeignKey("dbo.Pricings", "UserId", "dbo.AppUsers");
            DropForeignKey("dbo.Comments", "OrderId", "dbo.Orders");
            DropIndex("dbo.Comments", new[] { "OrderId" });
            DropIndex("dbo.Components", new[] { "VehicleId" });
            DropIndex("dbo.Components", new[] { "Pricing_PricingId" });
            DropIndex("dbo.Pricings", new[] { "RentServiceId" });
            DropIndex("dbo.Pricings", new[] { "UserId" });
            RenameColumn(table: "dbo.Comments", name: "OrderId", newName: "Order_OrderId");
            AddColumn("dbo.Comments", "RentServiceId", c => c.Int(nullable: false));
            AddColumn("dbo.Orders", "Price", c => c.Single(nullable: false));
            AlterColumn("dbo.Comments", "Order_OrderId", c => c.Int());
            CreateIndex("dbo.Comments", "RentServiceId");
            CreateIndex("dbo.Comments", "Order_OrderId");
            AddForeignKey("dbo.Comments", "RentServiceId", "dbo.RentServices", "RentServiceId", cascadeDelete: true);
            AddForeignKey("dbo.Comments", "Order_OrderId", "dbo.Orders", "OrderId");
            DropTable("dbo.Components");
            DropTable("dbo.Pricings");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Pricings",
                c => new
                    {
                        PricingId = c.Int(nullable: false, identity: true),
                        RentServiceId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PricingId);
            
            CreateTable(
                "dbo.Components",
                c => new
                    {
                        ComponentId = c.Int(nullable: false, identity: true),
                        VehicleId = c.Int(nullable: false),
                        HourlyPrice = c.Single(nullable: false),
                        Pricing_PricingId = c.Int(),
                    })
                .PrimaryKey(t => t.ComponentId);
            
            DropForeignKey("dbo.Comments", "Order_OrderId", "dbo.Orders");
            DropForeignKey("dbo.Comments", "RentServiceId", "dbo.RentServices");
            DropIndex("dbo.Comments", new[] { "Order_OrderId" });
            DropIndex("dbo.Comments", new[] { "RentServiceId" });
            AlterColumn("dbo.Comments", "Order_OrderId", c => c.Int(nullable: false));
            DropColumn("dbo.Orders", "Price");
            DropColumn("dbo.Comments", "RentServiceId");
            RenameColumn(table: "dbo.Comments", name: "Order_OrderId", newName: "OrderId");
            CreateIndex("dbo.Pricings", "UserId");
            CreateIndex("dbo.Pricings", "RentServiceId");
            CreateIndex("dbo.Components", "Pricing_PricingId");
            CreateIndex("dbo.Components", "VehicleId");
            CreateIndex("dbo.Comments", "OrderId");
            AddForeignKey("dbo.Comments", "OrderId", "dbo.Orders", "OrderId", cascadeDelete: true);
            AddForeignKey("dbo.Pricings", "UserId", "dbo.AppUsers", "UserId", cascadeDelete: true);
            AddForeignKey("dbo.Pricings", "RentServiceId", "dbo.RentServices", "RentServiceId", cascadeDelete: true);
            AddForeignKey("dbo.Components", "Pricing_PricingId", "dbo.Pricings", "PricingId");
            AddForeignKey("dbo.Components", "VehicleId", "dbo.Vehicles", "VehicleId", cascadeDelete: true);
        }
    }
}
