using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTargetGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_NotificationTargetGroups_TargetGroupId",
                table: "Notifications");

            migrationBuilder.DropTable(
                name: "NotificationTargetGroups");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_TargetGroupId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "TargetGroupId",
                table: "Notifications");

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "Notifications",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "Notifications",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TargetGroupId",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "NotificationTargetGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TargetGroup = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTargetGroups", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TargetGroupId",
                table: "Notifications",
                column: "TargetGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_NotificationTargetGroups_TargetGroupId",
                table: "Notifications",
                column: "TargetGroupId",
                principalTable: "NotificationTargetGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
