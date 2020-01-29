using Microsoft.EntityFrameworkCore.Migrations;

namespace Nesteo.Server.Migrations
{
    public partial class AllowUnknownForMoreInspectionFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>("Occupied", "Inspections", nullable: true, oldClrType: typeof(bool), oldType: "bit");

            migrationBuilder.AlterColumn<int>("ChickCount", "Inspections", nullable: true, oldClrType: typeof(int), oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>("Occupied", "Inspections", "bit", nullable: false, oldClrType: typeof(bool), oldNullable: true);

            migrationBuilder.AlterColumn<int>("ChickCount", "Inspections", "int", nullable: false, oldClrType: typeof(int), oldNullable: true);
        }
    }
}
