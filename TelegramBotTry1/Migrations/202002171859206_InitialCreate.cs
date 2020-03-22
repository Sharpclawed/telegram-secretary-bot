namespace TelegramBotTry1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "public.MessageDataSets",
                c => new
                    {
                        MessageDataSetId = c.Guid(nullable: false, identity: true),
                        MessageId = c.Long(nullable: false),
                        Message = c.String(),
                        Date = c.DateTime(nullable: false),
                        UserName = c.String(),
                        UserFirstName = c.String(),
                        UserLastName = c.String(),
                        UserId = c.Long(nullable: false),
                        ChatId = c.Long(nullable: false),
                        ChatName = c.String(),
                    })
                .PrimaryKey(t => t.MessageDataSetId);
            
        }
        
        public override void Down()
        {
            DropTable("public.MessageDataSets");
        }
    }
}
