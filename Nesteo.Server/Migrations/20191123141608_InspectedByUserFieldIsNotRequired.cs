using Microsoft.EntityFrameworkCore.Migrations;

namespace Nesteo.Server.Migrations
{
    public partial class InspectedByUserFieldIsNotRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inspections_Users_InspectedByUserId",
                table: "Inspections");

            migrationBuilder.AlterColumn<string>(
                name: "InspectedByUserId",
                table: "Inspections",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.AddForeignKey(
                name: "FK_Inspections_Users_InspectedByUserId",
                table: "Inspections",
                column: "InspectedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inspections_Users_InspectedByUserId",
                table: "Inspections");

            migrationBuilder.AlterColumn<string>(
                name: "InspectedByUserId",
                table: "Inspections",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Inspections_Users_InspectedByUserId",
                table: "Inspections",
                column: "InspectedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
