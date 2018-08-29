namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Order_Comment : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Comments", "Order_OrderId", "dbo.Orders");
            DropForeignKey("dbo.Comments", "RentServiceId", "dbo.RentServices");
            DropIndex("dbo.Comments", new[] { "RentServiceId" });
            DropIndex("dbo.Comments", new[] { "Order_OrderId" });
            RenameColumn(table: "dbo.Comments", name: "Order_OrderId", newName: "OrderId");
            RenameColumn(table: "dbo.Comments", name: "RentServiceId", newName: "RentService_RentServiceId");
            AlterColumn("dbo.Comments", "RentService_RentServiceId", c => c.Int());
            AlterColumn("dbo.Comments", "OrderId", c => c.Int(nullable: false));
            CreateIndex("dbo.Comments", "OrderId");
            CreateIndex("dbo.Comments", "RentService_RentServiceId");
            AddForeignKey("dbo.Comments", "OrderId", "dbo.Orders", "OrderId", cascadeDelete: true);
            AddForeignKey("dbo.Comments", "RentService_RentServiceId", "dbo.RentServices", "RentServiceId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "RentService_RentServiceId", "dbo.RentServices");
            DropForeignKey("dbo.Comments", "OrderId", "dbo.Orders");
            DropIndex("dbo.Comments", new[] { "RentService_RentServiceId" });
            DropIndex("dbo.Comments", new[] { "OrderId" });
            AlterColumn("dbo.Comments", "OrderId", c => c.Int());
            AlterColumn("dbo.Comments", "RentService_RentServiceId", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.Comments", name: "RentService_RentServiceId", newName: "RentServiceId");
            RenameColumn(table: "dbo.Comments", name: "OrderId", newName: "Order_OrderId");
            CreateIndex("dbo.Comments", "Order_OrderId");
            CreateIndex("dbo.Comments", "RentServiceId");
            AddForeignKey("dbo.Comments", "RentServiceId", "dbo.RentServices", "RentServiceId", cascadeDelete: true);
            AddForeignKey("dbo.Comments", "Order_OrderId", "dbo.Orders", "OrderId");
        }
    }
}
