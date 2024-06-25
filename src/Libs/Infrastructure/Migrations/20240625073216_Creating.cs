using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seedysoft.Libs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Creating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Outbox",
                columns: table => new
                {
                    OutboxId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Payload = table.Column<string>(type: "TEXT", nullable: false),
                    SubscriptionName = table.Column<string>(type: "TEXT", nullable: false),
                    SubscriptionId = table.Column<long>(type: "INTEGER", nullable: true),
                    SentAtDateTimeOffset = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outbox", x => x.OutboxId);
                });

            migrationBuilder.CreateTable(
                name: "Pvpc",
                columns: table => new
                {
                    AtDateTimeOffset = table.Column<string>(type: "TEXT", nullable: false),
                    MWhPriceInEuros = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pvpc", x => x.AtDateTimeOffset);
                });

            migrationBuilder.CreateTable(
                name: "Subscriber",
                columns: table => new
                {
                    SubscriberId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Firstname = table.Column<string>(type: "TEXT", nullable: false),
                    TelegramUserId = table.Column<long>(type: "INTEGER", nullable: true),
                    MailAddress = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriber", x => x.SubscriberId);
                });

            migrationBuilder.CreateTable(
                name: "Subscription",
                columns: table => new
                {
                    SubscriptionId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SubscriptionName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscription", x => x.SubscriptionId);
                });

            migrationBuilder.CreateTable(
                name: "TuyaDevice",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false),
                    Version = table.Column<float>(type: "REAL", nullable: false),
                    LocalKey = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TuyaDevice", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebData",
                columns: table => new
                {
                    SubscriptionId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WebUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    CurrentWebContent = table.Column<string>(type: "TEXT", nullable: true),
                    SeenAtDateTimeOffset = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedAtDateTimeOffset = table.Column<string>(type: "TEXT", nullable: true),
                    IgnoreChangeWhen = table.Column<string>(type: "TEXT", nullable: true),
                    CssSelector = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "body"),
                    TakeAboveBelowLines = table.Column<long>(type: "INTEGER", nullable: false, defaultValue: 3L),
                    UseHttpClient = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebData", x => x.SubscriptionId);
                });

            migrationBuilder.CreateTable(
                name: "SubscriberSubscription",
                columns: table => new
                {
                    SubscriberId = table.Column<long>(type: "INTEGER", nullable: false),
                    SubscriptionId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriberSubscription", x => new { x.SubscriberId, x.SubscriptionId });
                    table.ForeignKey(
                        name: "FK_SubscriberSubscription_Subscriber_SubscriberId",
                        column: x => x.SubscriberId,
                        principalTable: "Subscriber",
                        principalColumn: "SubscriberId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriberSubscription_Subscription_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscription",
                        principalColumn: "SubscriptionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubscriberSubscription_SubscriptionId",
                table: "SubscriberSubscription",
                column: "SubscriptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Outbox");

            migrationBuilder.DropTable(
                name: "Pvpc");

            migrationBuilder.DropTable(
                name: "SubscriberSubscription");

            migrationBuilder.DropTable(
                name: "TuyaDevice");

            migrationBuilder.DropTable(
                name: "WebData");

            migrationBuilder.DropTable(
                name: "Subscriber");

            migrationBuilder.DropTable(
                name: "Subscription");
        }
    }
}
