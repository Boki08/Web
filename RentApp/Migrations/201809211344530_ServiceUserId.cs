namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceUserId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RentServices", "AppUser_UserId", "dbo.AppUsers");
            DropIndex("dbo.RentServices", new[] { "AppUser_UserId" });
            RenameColumn(table: "dbo.RentServices", name: "AppUser_UserId", newName: "UserId");
            AlterColumn("dbo.RentServices", "UserId", c => c.Int(nullable: false));
            CreateIndex("dbo.RentServices", "UserId");
            AddForeignKey("dbo.RentServices", "UserId", "dbo.AppUsers", "UserId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RentServices", "UserId", "dbo.AppUsers");
            DropIndex("dbo.RentServices", new[] { "UserId" });
            AlterColumn("dbo.RentServices", "UserId", c => c.Int());
            RenameColumn(table: "dbo.RentServices", name: "UserId", newName: "AppUser_UserId");
            CreateIndex("dbo.RentServices", "AppUser_UserId");
            AddForeignKey("dbo.RentServices", "AppUser_UserId", "dbo.AppUsers", "UserId");
        }
    }
}
