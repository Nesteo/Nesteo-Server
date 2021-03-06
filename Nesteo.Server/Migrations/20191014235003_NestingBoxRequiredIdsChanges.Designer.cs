﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Nesteo.Server.Data;

namespace Nesteo.Server.Migrations
{
    [DbContext(typeof(NesteoDbContext))]
    [Migration("20191014235003_NestingBoxRequiredIdsChanges")]
    partial class NestingBoxRequiredIdsChanges
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("RoleId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Value")
                        .HasColumnType("longtext");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UserTokens");
                });

            modelBuilder.Entity("Nesteo.Server.Data.Entities.Identity.RoleEntity", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnType("varchar(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Nesteo.Server.Data.Entities.Identity.UserEntity", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .HasColumnType("varchar(256)")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("varchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("varchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("longtext");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("longtext");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("longtext");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasColumnType("varchar(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Nesteo.Server.Data.Entities.InspectionEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("AgeInDays")
                        .HasColumnType("int");

                    b.Property<int>("ChickCount")
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .HasColumnType("longtext");

                    b.Property<int>("Condition")
                        .HasColumnType("int");

                    b.Property<bool>("ContainsEggs")
                        .HasColumnType("bit");

                    b.Property<int?>("EggCount")
                        .HasColumnType("int");

                    b.Property<int>("FemaleParentBirdDiscovery")
                        .HasColumnType("int");

                    b.Property<bool>("HasBeenCleaned")
                        .HasColumnType("bit");

                    b.Property<string>("ImageFileName")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("InspectedByUserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime>("InspectionDate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("JustRepaired")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdated")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime(6)");

                    b.Property<int>("MaleParentBirdDiscovery")
                        .HasColumnType("int");

                    b.Property<string>("NestingBoxId")
                        .IsRequired()
                        .HasColumnType("varchar(6)");

                    b.Property<bool>("Occupied")
                        .HasColumnType("bit");

                    b.Property<int>("RingedChickCount")
                        .HasColumnType("int");

                    b.Property<int?>("SpeciesId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("InspectedByUserId");

                    b.HasIndex("NestingBoxId");

                    b.HasIndex("SpeciesId");

                    b.ToTable("Inspections");
                });

            modelBuilder.Entity("Nesteo.Server.Data.Entities.NestingBoxEntity", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(6)")
                        .HasMaxLength(6);

                    b.Property<string>("Comment")
                        .HasColumnType("longtext");

                    b.Property<double?>("CoordinateLatitude")
                        .HasColumnType("double");

                    b.Property<double?>("CoordinateLongitude")
                        .HasColumnType("double");

                    b.Property<string>("ForeignId")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<DateTime?>("HangUpDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("HangUpUserId")
                        .HasColumnType("varchar(255)");

                    b.Property<int>("HoleSize")
                        .HasColumnType("int");

                    b.Property<string>("ImageFileName")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<DateTime>("LastUpdated")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Material")
                        .HasColumnType("int");

                    b.Property<string>("OldId")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<int>("OwnerId")
                        .HasColumnType("int");

                    b.Property<int>("RegionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("HangUpUserId");

                    b.HasIndex("OwnerId");

                    b.HasIndex("RegionId");

                    b.ToTable("NestingBoxes");
                });

            modelBuilder.Entity("Nesteo.Server.Data.Entities.OwnerEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("Owners");
                });

            modelBuilder.Entity("Nesteo.Server.Data.Entities.RegionEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasMaxLength(255);

                    b.Property<string>("NestingBoxIdPrefix")
                        .HasColumnType("varchar(10)")
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.ToTable("Regions");
                });

            modelBuilder.Entity("Nesteo.Server.Data.Entities.ReservedIdSpaceEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("FirstNestingBoxIdWithoutPrefix")
                        .HasColumnType("int");

                    b.Property<int>("LastNestingBoxIdWithoutPrefix")
                        .HasColumnType("int");

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<int>("RegionId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ReservationDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.HasIndex("RegionId");

                    b.ToTable("ReservedIdSpaces");
                });

            modelBuilder.Entity("Nesteo.Server.Data.Entities.SpeciesEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("Species");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Nesteo.Server.Data.Entities.Identity.RoleEntity", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Nesteo.Server.Data.Entities.Identity.UserEntity", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Nesteo.Server.Data.Entities.Identity.UserEntity", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Nesteo.Server.Data.Entities.Identity.RoleEntity", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Nesteo.Server.Data.Entities.Identity.UserEntity", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Nesteo.Server.Data.Entities.Identity.UserEntity", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Nesteo.Server.Data.Entities.InspectionEntity", b =>
                {
                    b.HasOne("Nesteo.Server.Data.Entities.Identity.UserEntity", "InspectedByUser")
                        .WithMany()
                        .HasForeignKey("InspectedByUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Nesteo.Server.Data.Entities.NestingBoxEntity", "NestingBox")
                        .WithMany("Inspections")
                        .HasForeignKey("NestingBoxId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Nesteo.Server.Data.Entities.SpeciesEntity", "Species")
                        .WithMany("Inspections")
                        .HasForeignKey("SpeciesId");
                });

            modelBuilder.Entity("Nesteo.Server.Data.Entities.NestingBoxEntity", b =>
                {
                    b.HasOne("Nesteo.Server.Data.Entities.Identity.UserEntity", "HangUpUser")
                        .WithMany()
                        .HasForeignKey("HangUpUserId");

                    b.HasOne("Nesteo.Server.Data.Entities.OwnerEntity", "Owner")
                        .WithMany("NestingBoxes")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Nesteo.Server.Data.Entities.RegionEntity", "Region")
                        .WithMany("NestingBoxes")
                        .HasForeignKey("RegionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Nesteo.Server.Data.Entities.ReservedIdSpaceEntity", b =>
                {
                    b.HasOne("Nesteo.Server.Data.Entities.Identity.UserEntity", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Nesteo.Server.Data.Entities.RegionEntity", "Region")
                        .WithMany()
                        .HasForeignKey("RegionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
