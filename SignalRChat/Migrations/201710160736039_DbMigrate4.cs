namespace SignalRChat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DbMigrate4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Connections", "Connected", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Connections", "Connected", c => c.Boolean(nullable: false));
        }
    }
}
