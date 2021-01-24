using Microsoft.EntityFrameworkCore.Migrations;

namespace MVCTest.Migrations
{
    public partial class Logging3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "logId",
                table: "Message",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Message_logId",
                table: "Message",
                column: "logId");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Log_logId",
                table: "Message",
                column: "logId",
                principalTable: "Log",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_Log_logId",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_Message_logId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "logId",
                table: "Message");
        }
    }
}
