namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OfficePcture : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Offices", "Picture", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Offices", "Picture");
        }
    }
}
