using Microsoft.EntityFrameworkCore.Migrations;

namespace Nesteo.Server.Migrations
{
    public partial class NestingBoxRequiredIdsChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>("OldId", "NestingBoxes", maxLength: 100, nullable: true, oldClrType: typeof(string), oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>("ForeignId", "NestingBoxes", maxLength: 100, nullable: true, oldClrType: typeof(string), oldMaxLength: 100);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>("OldId", "NestingBoxes", maxLength: 100, nullable: false, oldClrType: typeof(string), oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>("ForeignId", "NestingBoxes", maxLength: 100, nullable: false, oldClrType: typeof(string), oldMaxLength: 100,
                oldNullable: true);
        }
    }
}
