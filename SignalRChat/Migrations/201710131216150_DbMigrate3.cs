namespace SignalRChat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DbMigrate3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Connections", "UserName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Connections", "UserName");
        }
    }
}
