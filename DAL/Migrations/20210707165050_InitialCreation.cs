using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class InitialCreation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "AdminDataSets",
                schema: "public",
                columns: table => new
                {
                    AdminDataSetId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    AddedUserId = table.Column<long>(type: "bigint", nullable: false),
                    AddedUserName = table.Column<string>(type: "text", nullable: true),
                    AddTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeletedUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletedUserName = table.Column<string>(type: "text", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminDataSets", x => x.AdminDataSetId);
                });

            migrationBuilder.CreateTable(
                name: "BookkeeperDataSets",
                schema: "public",
                columns: table => new
                {
                    BookkeeperDataSetId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    UserFirstName = table.Column<string>(type: "text", nullable: true),
                    UserLastName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookkeeperDataSets", x => x.BookkeeperDataSetId);
                });

            migrationBuilder.CreateTable(
                name: "MessageDataSets",
                schema: "public",
                columns: table => new
                {
                    MessageDataSetId = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageId = table.Column<long>(type: "bigint", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    UserFirstName = table.Column<string>(type: "text", nullable: true),
                    UserLastName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    ChatName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageDataSets", x => x.MessageDataSetId);
                });

            migrationBuilder.CreateTable(
                name: "OnetimeChatDataSets",
                schema: "public",
                columns: table => new
                {
                    OnetimeChatDataSetId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    ChatName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnetimeChatDataSets", x => x.OnetimeChatDataSetId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminDataSets",
                schema: "public");

            migrationBuilder.DropTable(
                name: "BookkeeperDataSets",
                schema: "public");

            migrationBuilder.DropTable(
                name: "MessageDataSets",
                schema: "public");

            migrationBuilder.DropTable(
                name: "OnetimeChatDataSets",
                schema: "public");
        }
    }
}
