namespace SignalRChat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DbMigrate1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "RepId", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "RepId");
        }
    }
}
