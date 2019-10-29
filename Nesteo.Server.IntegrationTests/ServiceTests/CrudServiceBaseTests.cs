using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Nesteo.Server.Data.Entities;
using Nesteo.Server.Services.Implementations;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Xunit;

namespace Nesteo.Server.IntegrationTests.ServiceTests
{
    public class CrudServiceBaseTests
    {
        [Fact]
        public async Task ManagesEntityWithGeneratedKeyCorrectly()
        {
            IMapper mapper = new MapperConfiguration(cfg => cfg.CreateMap<EntityWithGeneratedKey, Model>().ReverseMap()).CreateMapper();

            string connectionString = new MySqlConnectionStringBuilder { Server = "localhost", Database = $"test-{Guid.NewGuid()}", UserID = "root", Password = "Lzj8p5CNPcKg" }
                .ConnectionString;
            DbContextOptions<TestDbContext> dbContextOptions = new DbContextOptionsBuilder<TestDbContext>().UseMySql(connectionString,
                                                                                                                     mySqlOptions => {
                                                                                                                         mySqlOptions.ServerVersion(
                                                                                                                             new Version(10, 3),
                                                                                                                             ServerType.MariaDb);
                                                                                                                     }).Options;

            await using (var dbContext = new TestDbContext(dbContextOptions))
            {
                // Create database
                await dbContext.Database.EnsureCreatedAsync().ConfigureAwait(false);
            }

            await using (var dbContext = new TestDbContext(dbContextOptions))
            {
                var service = new ServiceForEntityWithGeneratedKey(dbContext, mapper);

                // Database should be empty
                Assert.False(await service.GetAllAsync().AnyAsync().ConfigureAwait(false));

                // ID should not exist yet
                Assert.Null(await service.FindByIdAsync("asdf").ConfigureAwait(false));

                // Update should fail when entry does not exist
                await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => service.InsertOrUpdateAsync(new Model { Id = "asdf", Name = "Test" })).ConfigureAwait(false);
            }

            Model insertedModel;
            await using (var dbContext = new TestDbContext(dbContextOptions))
            {
                var service = new ServiceForEntityWithGeneratedKey(dbContext, mapper);

                var newModel = new Model { Name = "Test" };

                // Add entry
                insertedModel = await service.InsertOrUpdateAsync(newModel).ConfigureAwait(false);
                Assert.NotNull(insertedModel);
                Assert.NotNull(insertedModel.Id);
                Assert.Equal(newModel.Name, insertedModel.Name);
                Assert.Equal(DateTime.UtcNow, insertedModel.LastUpdated, TimeSpan.FromMinutes(1));
            }

            string generatedId = insertedModel.Id;

            await using (var dbContext = new TestDbContext(dbContextOptions))
            {
                var service = new ServiceForEntityWithGeneratedKey(dbContext, mapper);

                // Entry should exist now
                Assert.NotNull(await service.FindByIdAsync(generatedId).ConfigureAwait(false));

                // Database should contain one entry now
                Assert.Equal(1, await service.GetAllAsync().CountAsync().ConfigureAwait(false));

                // Update entity
                insertedModel.Name = "Updated Test";
                Model updatedModel = await service.InsertOrUpdateAsync(insertedModel).ConfigureAwait(false);
                Assert.NotNull(updatedModel);
                Assert.Equal(insertedModel.Id, updatedModel.Id);
                Assert.Equal(insertedModel.Name, updatedModel.Name);
                Assert.NotEqual(insertedModel.LastUpdated, updatedModel.LastUpdated);
                Assert.Equal(DateTime.UtcNow, insertedModel.LastUpdated, TimeSpan.FromMinutes(1));
            }

            await using (var dbContext = new TestDbContext(dbContextOptions))
            {
                var service = new ServiceForEntityWithGeneratedKey(dbContext, mapper);

                // Doing the same update again should fail, because the last update time is wrong
                await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => service.InsertOrUpdateAsync(insertedModel)).ConfigureAwait(false);
            }
        }

        private class ServiceForEntityWithGeneratedKey : CrudServiceBase<EntityWithGeneratedKey, Model, string>
        {
            public ServiceForEntityWithGeneratedKey(DbContext dbContext, IMapper mapper) : base(dbContext, mapper) { }
        }

        private class EntityWithGeneratedKey : IEntity<string>
        {
            [Key]
            [Required]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public string Id { get; set; }

            [Required]
            public string Name { get; set; }

            [Required]
            [Timestamp]
            [DataType(DataType.DateTime)]
            public DateTime LastUpdated { get; set; }
        }

        private class Model
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public DateTime LastUpdated { get; set; }
        }

        private class TestDbContext : DbContext
        {
            public DbSet<EntityWithGeneratedKey> EntitiesWithGeneratedKey { get; set; }

            public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
        }
    }
}
