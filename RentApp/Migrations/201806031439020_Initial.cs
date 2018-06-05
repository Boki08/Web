namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RentServices",
                c => new
                {
                    RentServiceId = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false),
                    email = c.String(nullable: false),
                    Logo = c.String(nullable: false),
                    Description = c.String(nullable: false),
                    ServiceId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.RentServiceId);

            CreateTable(
                "dbo.Comments",
                c => new
                {
                    CommentId = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false),
                    UserId = c.Int(nullable: false),
                    OrderId = c.Int(nullable: false),
                    Review = c.String(nullable: false),
                    Grade = c.Int(nullable: false)
                })
                 .PrimaryKey(t => t.CommentId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .ForeignKey("dbo.Orders", t => t.OrderId)
                .Index(t => t.UserId)
                .Index(t => t.OrderId);


            CreateTable(
                "dbo.Offices",
                c => new
                    {
                    OfficeId = c.Int(nullable: false),
                        RentServiceId = c.Int(nullable: false),
                        Address = c.String(nullable: false),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.OfficeId)
                .ForeignKey("dbo.RentServices", t => t.RentServiceId)
                .Index(t => t.RentServiceId);
            
            CreateTable(
                "dbo.OfficePictures",
                c => new
                    {
                    OfficePictureId = c.Int(nullable: false),
                        OfficeId = c.Int(nullable: false),
                        Data = c.String(nullable: false),
                    })
                  .PrimaryKey(t => t.OfficePictureId)
                .ForeignKey("dbo.Offices", t => t.OfficeId)
                .Index(t => t.OfficeId);

            CreateTable(
                "dbo.Orders",
                c => new
                    {

                       OrderId = c.Int(nullable: false),
                       VehicleId = c.Int(nullable: false),
                       UserId = c.Int(nullable: false),
                       DepartureOffice = c.String(nullable: false),
                       ReturnOffice = c.String(nullable: false),
                       DepartureDate = c.DateTime(nullable: false),
                       ReturnDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.OrderId)
                .ForeignKey("dbo.Vehicles", t => t.VehicleId)
                 .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.VehicleId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Pricings",
                c => new
                    {
                    PricingId = c.Int(nullable: false),
                        RentServiceId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),

                })
                .PrimaryKey(t => t.PricingId)
                .ForeignKey("dbo.RentServices", t => t.RentServiceId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.RentServiceId)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.Users",
                c => new
                {
                    UserId = c.Int(nullable: false),
                    Name = c.String(nullable: false),
                    Surname = c.String(nullable: false),
                    Email = c.String(nullable: false),
                    BirthDate = c.DateTime(nullable: false),
                    DocumentPicture = c.String(nullable: false),
                    Type = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.UserId);

            CreateTable(
               "dbo.Vehicles",
               c => new
               {

                   VehicleId = c.Int(nullable: false),
                    RentServiceId = c.Int(nullable: false),
                    Model = c.String(nullable: false),
                    YearOfManufacturing = c.Int(nullable: false),
                    Manufacturer = c.String(nullable: false),
                    Description = c.String(nullable: false),
                    Available = c.Boolean(nullable: false),

               })
               .PrimaryKey(t =>t.VehicleId)
               .ForeignKey("dbo.RentServices", t => t.RentServiceId)
               .Index(t => t.RentServiceId);



            CreateTable(
               "dbo.VehiclePictures",
               c => new
               {
                   VehiclePictureId = c.Int(nullable: false),
                   VehicleId = c.Int(nullable: false),
                   Data = c.String(nullable: false),
               })
               .PrimaryKey(t =>t.VehiclePictureId)
               .ForeignKey("dbo.Vehicles", t => t.VehicleId)
               .Index(t => t.VehicleId);

        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "AppUserId", "dbo.AppUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "AppUserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Services");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AppUsers");
        }
    }
}
