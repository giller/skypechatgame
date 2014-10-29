namespace SkypeChatGame.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GameModels",
                c => new
                    {
                        GameId = c.Int(nullable: false, identity: true),
                        Score = c.Int(nullable: true),
                        Player_UserId = c.Int(),
                    })
                .PrimaryKey(t => t.GameId)
                .ForeignKey("dbo.UserProfile", t => t.Player_UserId)
                .Index(t => t.Player_UserId);
            
            CreateTable(
                "dbo.MessageModels",
                c => new
                    {
                        MessageId = c.Int(nullable: false, identity: true),
                        Author = c.String(),
                        Contents = c.String(),
                        Timestamp = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.MessageId);
            
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Email = c.String(),
                        HighScore = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.MessageModelGameModels",
                c => new
                    {
                        MessageModel_MessageId = c.Int(nullable: false),
                        GameModel_GameId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.MessageModel_MessageId, t.GameModel_GameId })
                .ForeignKey("dbo.MessageModels", t => t.MessageModel_MessageId, cascadeDelete: true)
                .ForeignKey("dbo.GameModels", t => t.GameModel_GameId, cascadeDelete: true)
                .Index(t => t.MessageModel_MessageId)
                .Index(t => t.GameModel_GameId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GameModels", "Player_UserId", "dbo.UserProfile");
            DropForeignKey("dbo.MessageModelGameModels", "GameModel_GameId", "dbo.GameModels");
            DropForeignKey("dbo.MessageModelGameModels", "MessageModel_MessageId", "dbo.MessageModels");
            DropIndex("dbo.MessageModelGameModels", new[] { "GameModel_GameId" });
            DropIndex("dbo.MessageModelGameModels", new[] { "MessageModel_MessageId" });
            DropIndex("dbo.GameModels", new[] { "Player_UserId" });
            DropTable("dbo.MessageModelGameModels");
            DropTable("dbo.UserProfile");
            DropTable("dbo.MessageModels");
            DropTable("dbo.GameModels");
        }
    }
}
