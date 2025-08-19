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
                columns: static table => new
                {
                    OutboxId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", value: true),
                    Payload = table.Column<string>(type: "TEXT", nullable: false),
                    SubscriptionName = table.Column<string>(type: "TEXT", nullable: false),
                    SubscriptionId = table.Column<long>(type: "INTEGER", nullable: true),
                    SentAtDateTimeOffset = table.Column<string>(type: "TEXT", nullable: true),
                },
                constraints: static table =>
                {
                    table.PrimaryKey("PK_Outbox", static x => x.OutboxId);
                });

            migrationBuilder.CreateTable(
                name: "Pvpc",
                columns: static table => new
                {
                    AtDateTimeOffset = table.Column<string>(type: "TEXT", nullable: false),
                    MWhPriceInEuros = table.Column<decimal>(type: "TEXT", nullable: false),
                },
                constraints: static table =>
                {
                    table.PrimaryKey("PK_Pvpc", static x => x.AtDateTimeOffset);
                });

            migrationBuilder.CreateTable(
                name: "Subscriber",
                columns: static table => new
                {
                    SubscriberId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", value: true),
                    Firstname = table.Column<string>(type: "TEXT", nullable: false),
                    TelegramUserId = table.Column<long>(type: "INTEGER", nullable: true),
                    MailAddress = table.Column<string>(type: "TEXT", nullable: true),
                },
                constraints: static table =>
                {
                    table.PrimaryKey("PK_Subscriber", static x => x.SubscriberId);
                });

            migrationBuilder.CreateTable(
                name: "Subscription",
                columns: static table => new
                {
                    SubscriptionId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", value: true),
                    SubscriptionName = table.Column<string>(type: "TEXT", nullable: false),
                },
                constraints: static table =>
                {
                    table.PrimaryKey("PK_Subscription", static x => x.SubscriptionId);
                });

            migrationBuilder.CreateTable(
                name: "TuyaDevice",
                columns: static table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false),
                    Version = table.Column<float>(type: "REAL", nullable: false),
                    LocalKey = table.Column<string>(type: "TEXT", nullable: false),
                },
                constraints: static table =>
                {
                    table.PrimaryKey("PK_TuyaDevice", static x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebData",
                columns: static table => new
                {
                    SubscriptionId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", value: true),
                    WebUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    CurrentWebContent = table.Column<string>(type: "TEXT", nullable: true),
                    SeenAtDateTimeOffset = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedAtDateTimeOffset = table.Column<string>(type: "TEXT", nullable: true),
                    IgnoreChangeWhen = table.Column<string>(type: "TEXT", nullable: true),
                    CssSelector = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "body"),
                    TakeAboveBelowLines = table.Column<long>(type: "INTEGER", nullable: false, defaultValue: 3L),
                    UseHttpClient = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                },
                constraints: static table =>
                {
                    table.PrimaryKey("PK_WebData", static x => x.SubscriptionId);
                });

            migrationBuilder.CreateTable(
                name: "SubscriberSubscription",
                columns: static table => new
                {
                    SubscriberId = table.Column<long>(type: "INTEGER", nullable: false),
                    SubscriptionId = table.Column<long>(type: "INTEGER", nullable: false),
                },
                constraints: static table =>
                {
                    table.PrimaryKey("PK_SubscriberSubscription", static x => new { x.SubscriberId, x.SubscriptionId });
                    table.ForeignKey(
                        name: "FK_SubscriberSubscription_Subscriber_SubscriberId",
                        column: static x => x.SubscriberId,
                        principalTable: "Subscriber",
                        principalColumn: "SubscriberId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriberSubscription_Subscription_SubscriptionId",
                        column: static x => x.SubscriptionId,
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
