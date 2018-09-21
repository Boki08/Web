namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserServices : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RentServices", "AppUser_UserId", c => c.Int());
            CreateIndex("dbo.RentServices", "AppUser_UserId");
            AddForeignKey("dbo.RentServices", "AppUser_UserId", "dbo.AppUsers", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RentServices", "AppUser_UserId", "dbo.AppUsers");
            DropIndex("dbo.RentServices", new[] { "AppUser_UserId" });
            DropColumn("dbo.RentServices", "AppUser_UserId");
        }
    }
}
