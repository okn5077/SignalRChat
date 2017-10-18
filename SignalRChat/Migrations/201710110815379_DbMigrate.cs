namespace SignalRChat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DbMigrate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Representative_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.AspNetUsers", "Representative_Id");
            AddForeignKey("dbo.AspNetUsers", "Representative_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "Representative_Id", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetUsers", new[] { "Representative_Id" });
            DropColumn("dbo.AspNetUsers", "Representative_Id");
        }
    }
}
