using Microsoft.EntityFrameworkCore.Migrations;

namespace Nesteo.Server.Migrations
{
    public partial class InspectedByUserFieldIsNotRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_Inspections_Users_InspectedByUserId", "Inspections");

            migrationBuilder.AlterColumn<string>("InspectedByUserId", "Inspections", nullable: true, oldClrType: typeof(string), oldType: "varchar(255)");

            migrationBuilder.AddForeignKey("FK_Inspections_Users_InspectedByUserId", "Inspections", "InspectedByUserId", "Users", principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_Inspections_Users_InspectedByUserId", "Inspections");

            migrationBuilder.AlterColumn<string>("InspectedByUserId", "Inspections", "varchar(255)", nullable: false, oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddForeignKey("FK_Inspections_Users_InspectedByUserId", "Inspections", "InspectedByUserId", "Users", principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
