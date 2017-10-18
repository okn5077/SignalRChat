namespace SignalRChat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DbMigrate5 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Connections");
            CreateTable(
                "dbo.ConversationLogs",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        FromUser = c.String(),
                        ToUser = c.String(),
                        Message = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        ClientIp = c.String(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            AddColumn("dbo.Connections", "Id", c => c.Guid(nullable: false, identity: true));
            AlterColumn("dbo.Connections", "ConnectionID", c => c.String());
            AddPrimaryKey("dbo.Connections", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ConversationLogs", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.ConversationLogs", new[] { "ApplicationUser_Id" });
            DropPrimaryKey("dbo.Connections");
            AlterColumn("dbo.Connections", "ConnectionID", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.Connections", "Id");
            DropTable("dbo.ConversationLogs");
            AddPrimaryKey("dbo.Connections", "ConnectionID");
        }
    }
}
