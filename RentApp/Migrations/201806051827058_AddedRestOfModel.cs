namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRestOfModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        CommentId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        OrderId = c.Int(nullable: false),
                        Review = c.String(nullable: false),
                        Grade = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CommentId)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderId = c.Int(nullable: false, identity: true),
                        VehicleId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        DepartureOffice = c.String(nullable: false),
                        ReturnOffice = c.String(nullable: false),
                        DepartureDate = c.DateTime(nullable: false),
                        ReturnDate = c.DateTime(nullable: false),
                        RentService_RentServiceId = c.Int(),
                    })
                .PrimaryKey(t => t.OrderId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.RentServices", t => t.RentService_RentServiceId)
                .ForeignKey("dbo.Vehicles", t => t.VehicleId, cascadeDelete: true)
                .Index(t => t.VehicleId)
                .Index(t => t.UserId)
                .Index(t => t.RentService_RentServiceId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Surname = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        BirthDate = c.DateTime(nullable: false),
                        DocumentPicture = c.String(nullable: false),
                        Type = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.Vehicles",
                c => new
                    {
                        VehicleId = c.Int(nullable: false, identity: true),
                        RentServiceId = c.Int(nullable: false),
                        Model = c.String(nullable: false),
                        YearOfManufacturing = c.String(nullable: false),
                        Manufacturer = c.String(nullable: false),
                        Pictures = c.String(),
                        Description = c.String(nullable: false),
                        Available = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.VehicleId)
                .ForeignKey("dbo.RentServices", t => t.RentServiceId, cascadeDelete: true)
                .Index(t => t.RentServiceId);
            
            CreateTable(
                "dbo.RentServices",
                c => new
                    {
                        RentServiceId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Logo = c.String(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.RentServiceId);
            
            CreateTable(
                "dbo.Offices",
                c => new
                    {
                        OfficeId = c.Int(nullable: false, identity: true),
                        RentServiceId = c.Int(nullable: false),
                        Address = c.String(nullable: false),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.OfficeId)
                .ForeignKey("dbo.RentServices", t => t.RentServiceId, cascadeDelete: true)
                .Index(t => t.RentServiceId);
            
            CreateTable(
                "dbo.Components",
                c => new
                    {
                        ComponentId = c.Int(nullable: false, identity: true),
                        VehicleId = c.Int(nullable: false),
                        HourlyPrice = c.Single(nullable: false),
                        Pricing_PricingId = c.Int(),
                    })
                .PrimaryKey(t => t.ComponentId)
                .ForeignKey("dbo.Vehicles", t => t.VehicleId, cascadeDelete: true)
                .ForeignKey("dbo.Pricings", t => t.Pricing_PricingId)
                .Index(t => t.VehicleId)
                .Index(t => t.Pricing_PricingId);
            
            CreateTable(
                "dbo.OfficePictures",
                c => new
                    {
                        OfficePictureId = c.Int(nullable: false, identity: true),
                        OfficeId = c.Int(nullable: false),
                        Data = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.OfficePictureId)
                .ForeignKey("dbo.Offices", t => t.OfficeId, cascadeDelete: true)
                .Index(t => t.OfficeId);
            
            CreateTable(
                "dbo.Pricings",
                c => new
                    {
                        PricingId = c.Int(nullable: false, identity: true),
                        RentServiceId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PricingId)
                .ForeignKey("dbo.RentServices", t => t.RentServiceId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.RentServiceId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.VehiclePictures",
                c => new
                    {
                        VehiclePictureId = c.Int(nullable: false, identity: true),
                        VehicleId = c.Int(nullable: false),
                        Data = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.VehiclePictureId)
                .ForeignKey("dbo.Vehicles", t => t.VehicleId, cascadeDelete: true)
                .Index(t => t.VehicleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VehiclePictures", "VehicleId", "dbo.Vehicles");
            DropForeignKey("dbo.Pricings", "UserId", "dbo.Users");
            DropForeignKey("dbo.Pricings", "RentServiceId", "dbo.RentServices");
            DropForeignKey("dbo.Components", "Pricing_PricingId", "dbo.Pricings");
            DropForeignKey("dbo.OfficePictures", "OfficeId", "dbo.Offices");
            DropForeignKey("dbo.Components", "VehicleId", "dbo.Vehicles");
            DropForeignKey("dbo.Comments", "UserId", "dbo.Users");
            DropForeignKey("dbo.Comments", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Orders", "VehicleId", "dbo.Vehicles");
            DropForeignKey("dbo.Vehicles", "RentServiceId", "dbo.RentServices");
            DropForeignKey("dbo.Orders", "RentService_RentServiceId", "dbo.RentServices");
            DropForeignKey("dbo.Offices", "RentServiceId", "dbo.RentServices");
            DropForeignKey("dbo.Orders", "UserId", "dbo.Users");
            DropIndex("dbo.VehiclePictures", new[] { "VehicleId" });
            DropIndex("dbo.Pricings", new[] { "UserId" });
            DropIndex("dbo.Pricings", new[] { "RentServiceId" });
            DropIndex("dbo.OfficePictures", new[] { "OfficeId" });
            DropIndex("dbo.Components", new[] { "Pricing_PricingId" });
            DropIndex("dbo.Components", new[] { "VehicleId" });
            DropIndex("dbo.Offices", new[] { "RentServiceId" });
            DropIndex("dbo.Vehicles", new[] { "RentServiceId" });
            DropIndex("dbo.Orders", new[] { "RentService_RentServiceId" });
            DropIndex("dbo.Orders", new[] { "UserId" });
            DropIndex("dbo.Orders", new[] { "VehicleId" });
            DropIndex("dbo.Comments", new[] { "OrderId" });
            DropIndex("dbo.Comments", new[] { "UserId" });
            DropTable("dbo.VehiclePictures");
            DropTable("dbo.Pricings");
            DropTable("dbo.OfficePictures");
            DropTable("dbo.Components");
            DropTable("dbo.Offices");
            DropTable("dbo.RentServices");
            DropTable("dbo.Vehicles");
            DropTable("dbo.Users");
            DropTable("dbo.Orders");
            DropTable("dbo.Comments");
        }
    }
}
