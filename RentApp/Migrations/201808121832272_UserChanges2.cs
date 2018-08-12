namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserChanges2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AppUsers", "FullName", c => c.String(nullable: false));
            AddColumn("dbo.AppUsers", "Activated", c => c.Boolean(nullable: false));
            AddColumn("dbo.Comments", "PostedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Orders", "DepartureOffice_OfficeId", c => c.Int());
            AddColumn("dbo.Orders", "ReturnOffice_OfficeId", c => c.Int());
            CreateIndex("dbo.Orders", "DepartureOffice_OfficeId");
            CreateIndex("dbo.Orders", "ReturnOffice_OfficeId");
            AddForeignKey("dbo.Orders", "DepartureOffice_OfficeId", "dbo.Offices", "OfficeId");
            AddForeignKey("dbo.Orders", "ReturnOffice_OfficeId", "dbo.Offices", "OfficeId");
            DropColumn("dbo.AppUsers", "Name");
            DropColumn("dbo.AppUsers", "Surname");
            DropColumn("dbo.AppUsers", "Password");
            DropColumn("dbo.AppUsers", "Type");
            DropColumn("dbo.Orders", "DepartureOffice");
            DropColumn("dbo.Orders", "ReturnOffice");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "ReturnOffice", c => c.String(nullable: false));
            AddColumn("dbo.Orders", "DepartureOffice", c => c.String(nullable: false));
            AddColumn("dbo.AppUsers", "Type", c => c.String(nullable: false));
            AddColumn("dbo.AppUsers", "Password", c => c.String(nullable: false));
            AddColumn("dbo.AppUsers", "Surname", c => c.String(nullable: false));
            AddColumn("dbo.AppUsers", "Name", c => c.String(nullable: false));
            DropForeignKey("dbo.Orders", "ReturnOffice_OfficeId", "dbo.Offices");
            DropForeignKey("dbo.Orders", "DepartureOffice_OfficeId", "dbo.Offices");
            DropIndex("dbo.Orders", new[] { "ReturnOffice_OfficeId" });
            DropIndex("dbo.Orders", new[] { "DepartureOffice_OfficeId" });
            DropColumn("dbo.Orders", "ReturnOffice_OfficeId");
            DropColumn("dbo.Orders", "DepartureOffice_OfficeId");
            DropColumn("dbo.Comments", "PostedDate");
            DropColumn("dbo.AppUsers", "Activated");
            DropColumn("dbo.AppUsers", "FullName");
        }
    }
}
