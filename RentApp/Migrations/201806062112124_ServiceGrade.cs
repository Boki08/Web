namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceGrade : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RentServices", "Grade", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RentServices", "Grade");
        }
    }
}
