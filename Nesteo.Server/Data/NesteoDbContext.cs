using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nesteo.Server.Data.Entities;
using Nesteo.Server.Data.Entities.Identity;

namespace Nesteo.Server.Data
{
    public class NesteoDbContext : IdentityDbContext<UserEntity, RoleEntity, string>
    {
        public DbSet<RegionEntity> Regions { get; set; }
        public DbSet<OwnerEntity> Owners { get; set; }
        public DbSet<SpeciesEntity> Species { get; set; }
        public DbSet<NestingBoxEntity> NestingBoxes { get; set; }
        public DbSet<ReservedIdSpaceEntity> ReservedIdSpaces { get; set; }
        public DbSet<InspectionEntity> Inspections { get; set; }

        public NesteoDbContext(DbContextOptions<NesteoDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Rename identity tables
            builder.Entity<UserEntity>().ToTable("Users");
            builder.Entity<RoleEntity>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
        }
    }
}
