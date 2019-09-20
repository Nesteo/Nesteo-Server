using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nesteo.Server.Data.Entities;
using Nesteo.Server.Data.Identity;

namespace Nesteo.Server.Data
{
    public class NesteoDbContext : IdentityDbContext<NesteoUser, NesteoRole, string>
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
            builder.Entity<NesteoUser>().ToTable("Users");
            builder.Entity<NesteoRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
        }
    }
}
