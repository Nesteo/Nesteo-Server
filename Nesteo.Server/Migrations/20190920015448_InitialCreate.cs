using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Nesteo.Server.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable("Owners", table => new {
                Id = table.Column<int>(nullable: false).Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                Name = table.Column<string>(maxLength: 255, nullable: false)
            }, constraints: table => {
                table.PrimaryKey("PK_Owners", x => x.Id);
            });

            migrationBuilder.CreateTable("Regions", table => new {
                Id = table.Column<int>(nullable: false).Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                Name = table.Column<string>(maxLength: 255, nullable: false),
                NestingBoxIdPrefix = table.Column<string>(maxLength: 10, nullable: true)
            }, constraints: table => {
                table.PrimaryKey("PK_Regions", x => x.Id);
            });

            migrationBuilder.CreateTable("Roles", table => new {
                Id = table.Column<string>(nullable: false),
                Name = table.Column<string>(maxLength: 256, nullable: true),
                NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                ConcurrencyStamp = table.Column<string>(nullable: true)
            }, constraints: table => {
                table.PrimaryKey("PK_Roles", x => x.Id);
            });

            migrationBuilder.CreateTable("Species", table => new {
                Id = table.Column<int>(nullable: false).Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                Name = table.Column<string>(maxLength: 255, nullable: false)
            }, constraints: table => {
                table.PrimaryKey("PK_Species", x => x.Id);
            });

            migrationBuilder.CreateTable("Users", table => new {
                Id = table.Column<string>(nullable: false),
                UserName = table.Column<string>(maxLength: 256, nullable: true),
                NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                Email = table.Column<string>(maxLength: 256, nullable: true),
                NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                EmailConfirmed = table.Column<bool>(nullable: false),
                PasswordHash = table.Column<string>(nullable: true),
                SecurityStamp = table.Column<string>(nullable: true),
                ConcurrencyStamp = table.Column<string>(nullable: true),
                PhoneNumber = table.Column<string>(nullable: true),
                PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                TwoFactorEnabled = table.Column<bool>(nullable: false),
                LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                LockoutEnabled = table.Column<bool>(nullable: false),
                AccessFailedCount = table.Column<int>(nullable: false),
                FirstName = table.Column<string>(nullable: false),
                LastName = table.Column<string>(nullable: false)
            }, constraints: table => {
                table.PrimaryKey("PK_Users", x => x.Id);
            });

            migrationBuilder.CreateTable("RoleClaims", table => new {
                Id = table.Column<int>(nullable: false).Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                RoleId = table.Column<string>(nullable: false),
                ClaimType = table.Column<string>(nullable: true),
                ClaimValue = table.Column<string>(nullable: true)
            }, constraints: table => {
                table.PrimaryKey("PK_RoleClaims", x => x.Id);
                table.ForeignKey("FK_RoleClaims_Roles_RoleId", x => x.RoleId, "Roles", "Id", onDelete: ReferentialAction.Cascade);
            });

            migrationBuilder.CreateTable("NestingBoxes", table => new {
                Id = table.Column<string>(maxLength: 6, nullable: false),
                RegionId = table.Column<int>(nullable: false),
                OldId = table.Column<string>(maxLength: 100, nullable: false),
                ForeignId = table.Column<string>(maxLength: 100, nullable: false),
                CoordinateLongitude = table.Column<double>(nullable: true),
                CoordinateLatitude = table.Column<double>(nullable: true),
                HangUpDate = table.Column<DateTime>(nullable: true),
                HangUpUserId = table.Column<string>(nullable: true),
                OwnerId = table.Column<int>(nullable: false),
                Material = table.Column<int>(nullable: false),
                HoleSize = table.Column<int>(nullable: false),
                ImageFileName = table.Column<string>(maxLength: 100, nullable: true),
                Comment = table.Column<string>(nullable: true),
                LastUpdated = table.Column<DateTime>(nullable: false).Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
            }, constraints: table => {
                table.PrimaryKey("PK_NestingBoxes", x => x.Id);
                table.ForeignKey("FK_NestingBoxes_Users_HangUpUserId", x => x.HangUpUserId, "Users", "Id", onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_NestingBoxes_Owners_OwnerId", x => x.OwnerId, "Owners", "Id", onDelete: ReferentialAction.Cascade);
                table.ForeignKey("FK_NestingBoxes_Regions_RegionId", x => x.RegionId, "Regions", "Id", onDelete: ReferentialAction.Cascade);
            });

            migrationBuilder.CreateTable("ReservedIdSpaces", table => new {
                Id = table.Column<int>(nullable: false).Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                RegionId = table.Column<int>(nullable: false),
                OwnerId = table.Column<string>(nullable: false),
                ReservationDate = table.Column<DateTime>(nullable: false),
                FirstNestingBoxIdWithoutPrefix = table.Column<int>(nullable: false),
                LastNestingBoxIdWithoutPrefix = table.Column<int>(nullable: false)
            }, constraints: table => {
                table.PrimaryKey("PK_ReservedIdSpaces", x => x.Id);
                table.ForeignKey("FK_ReservedIdSpaces_Users_OwnerId", x => x.OwnerId, "Users", "Id", onDelete: ReferentialAction.Cascade);
                table.ForeignKey("FK_ReservedIdSpaces_Regions_RegionId", x => x.RegionId, "Regions", "Id", onDelete: ReferentialAction.Cascade);
            });

            migrationBuilder.CreateTable("UserClaims", table => new {
                Id = table.Column<int>(nullable: false).Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                UserId = table.Column<string>(nullable: false),
                ClaimType = table.Column<string>(nullable: true),
                ClaimValue = table.Column<string>(nullable: true)
            }, constraints: table => {
                table.PrimaryKey("PK_UserClaims", x => x.Id);
                table.ForeignKey("FK_UserClaims_Users_UserId", x => x.UserId, "Users", "Id", onDelete: ReferentialAction.Cascade);
            });

            migrationBuilder.CreateTable("UserLogins", table => new {
                LoginProvider = table.Column<string>(nullable: false),
                ProviderKey = table.Column<string>(nullable: false),
                ProviderDisplayName = table.Column<string>(nullable: true),
                UserId = table.Column<string>(nullable: false)
            }, constraints: table => {
                table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                table.ForeignKey("FK_UserLogins_Users_UserId", x => x.UserId, "Users", "Id", onDelete: ReferentialAction.Cascade);
            });

            migrationBuilder.CreateTable("UserRoles", table => new {
                UserId = table.Column<string>(nullable: false),
                RoleId = table.Column<string>(nullable: false)
            }, constraints: table => {
                table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                table.ForeignKey("FK_UserRoles_Roles_RoleId", x => x.RoleId, "Roles", "Id", onDelete: ReferentialAction.Cascade);
                table.ForeignKey("FK_UserRoles_Users_UserId", x => x.UserId, "Users", "Id", onDelete: ReferentialAction.Cascade);
            });

            migrationBuilder.CreateTable("UserTokens", table => new {
                UserId = table.Column<string>(nullable: false),
                LoginProvider = table.Column<string>(nullable: false),
                Name = table.Column<string>(nullable: false),
                Value = table.Column<string>(nullable: true)
            }, constraints: table => {
                table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                table.ForeignKey("FK_UserTokens_Users_UserId", x => x.UserId, "Users", "Id", onDelete: ReferentialAction.Cascade);
            });

            migrationBuilder.CreateTable("Inspections", table => new {
                Id = table.Column<int>(nullable: false).Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                NestingBoxId = table.Column<string>(nullable: false),
                InspectionDate = table.Column<DateTime>(nullable: false),
                InspectedByUserId = table.Column<string>(nullable: false),
                HasBeenCleaned = table.Column<bool>(nullable: false),
                Condition = table.Column<int>(nullable: false),
                JustRepaired = table.Column<bool>(nullable: false),
                Occupied = table.Column<bool>(nullable: false),
                ContainsEggs = table.Column<bool>(nullable: false),
                EggCount = table.Column<int>(nullable: true),
                ChickCount = table.Column<int>(nullable: false),
                RingedChickCount = table.Column<int>(nullable: false),
                AgeInDays = table.Column<int>(nullable: true),
                FemaleParentBirdDiscovery = table.Column<int>(nullable: false),
                MaleParentBirdDiscovery = table.Column<int>(nullable: false),
                SpeciesId = table.Column<int>(nullable: true),
                ImageFileName = table.Column<string>(maxLength: 100, nullable: true),
                Comment = table.Column<string>(nullable: true),
                LastUpdated = table.Column<DateTime>(nullable: false).Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
            }, constraints: table => {
                table.PrimaryKey("PK_Inspections", x => x.Id);
                table.ForeignKey("FK_Inspections_Users_InspectedByUserId", x => x.InspectedByUserId, "Users", "Id", onDelete: ReferentialAction.Cascade);
                table.ForeignKey("FK_Inspections_NestingBoxes_NestingBoxId", x => x.NestingBoxId, "NestingBoxes", "Id", onDelete: ReferentialAction.Cascade);
                table.ForeignKey("FK_Inspections_Species_SpeciesId", x => x.SpeciesId, "Species", "Id", onDelete: ReferentialAction.Restrict);
            });

            migrationBuilder.CreateIndex("IX_Inspections_InspectedByUserId", "Inspections", "InspectedByUserId");

            migrationBuilder.CreateIndex("IX_Inspections_NestingBoxId", "Inspections", "NestingBoxId");

            migrationBuilder.CreateIndex("IX_Inspections_SpeciesId", "Inspections", "SpeciesId");

            migrationBuilder.CreateIndex("IX_NestingBoxes_HangUpUserId", "NestingBoxes", "HangUpUserId");

            migrationBuilder.CreateIndex("IX_NestingBoxes_OwnerId", "NestingBoxes", "OwnerId");

            migrationBuilder.CreateIndex("IX_NestingBoxes_RegionId", "NestingBoxes", "RegionId");

            migrationBuilder.CreateIndex("IX_ReservedIdSpaces_OwnerId", "ReservedIdSpaces", "OwnerId");

            migrationBuilder.CreateIndex("IX_ReservedIdSpaces_RegionId", "ReservedIdSpaces", "RegionId");

            migrationBuilder.CreateIndex("IX_RoleClaims_RoleId", "RoleClaims", "RoleId");

            migrationBuilder.CreateIndex("RoleNameIndex", "Roles", "NormalizedName", unique: true);

            migrationBuilder.CreateIndex("IX_UserClaims_UserId", "UserClaims", "UserId");

            migrationBuilder.CreateIndex("IX_UserLogins_UserId", "UserLogins", "UserId");

            migrationBuilder.CreateIndex("IX_UserRoles_RoleId", "UserRoles", "RoleId");

            migrationBuilder.CreateIndex("EmailIndex", "Users", "NormalizedEmail");

            migrationBuilder.CreateIndex("UserNameIndex", "Users", "NormalizedUserName", unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("Inspections");

            migrationBuilder.DropTable("ReservedIdSpaces");

            migrationBuilder.DropTable("RoleClaims");

            migrationBuilder.DropTable("UserClaims");

            migrationBuilder.DropTable("UserLogins");

            migrationBuilder.DropTable("UserRoles");

            migrationBuilder.DropTable("UserTokens");

            migrationBuilder.DropTable("NestingBoxes");

            migrationBuilder.DropTable("Species");

            migrationBuilder.DropTable("Roles");

            migrationBuilder.DropTable("Users");

            migrationBuilder.DropTable("Owners");

            migrationBuilder.DropTable("Regions");
        }
    }
}
