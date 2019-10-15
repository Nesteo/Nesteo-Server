using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nesteo.Server.Data;
using Nesteo.Server.Data.Entities;
using Nesteo.Server.Data.Entities.Identity;
using Nesteo.Server.Data.Enums;

namespace Nesteo.Server.SampleDataGenerator
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            // Create host
            IHost host = Server.Program.CreateHostBuilder(args).Build();
            await Server.Program.PrepareHostAsync(host).ConfigureAwait(false);

            // Request database context
            NesteoDbContext dbContext = host.Services.GetRequiredService<NesteoDbContext>();

            // Safety check
            Console.Write("Do you want to fill the database with sample data? Please type 'yes': ");
            if (Console.ReadLine()?.ToLower() != "yes")
                return;
            Console.Write("Do you really want to do this? This will DELETE all existing data! Please type 'yes': ");
            if (Console.ReadLine()?.ToLower() != "yes")
                return;

            Console.WriteLine("Clearing database...");

            // Clear database
            dbContext.Regions.RemoveRange(dbContext.Regions);
            dbContext.Owners.RemoveRange(dbContext.Owners);
            dbContext.Species.RemoveRange(dbContext.Species);
            dbContext.NestingBoxes.RemoveRange(dbContext.NestingBoxes);
            dbContext.ReservedIdSpaces.RemoveRange(dbContext.ReservedIdSpaces);
            dbContext.Inspections.RemoveRange(dbContext.Inspections);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            Console.WriteLine("Generating sample data...");

            var random = new Random();
            UserEntity user = await dbContext.Users.FirstOrDefaultAsync().ConfigureAwait(false);
            if (user == null)
            {
                Console.WriteLine("Please run the server first to make sure that at least one user exists.");
                return;
            }

            // Generate regions
            Console.WriteLine("Generating regions...");
            await dbContext.Regions.AddRangeAsync(
                               Enumerable.Range(1, 20).Select(i => new RegionEntity { Name = $"Owner {i}", NestingBoxIdPrefix = ((char)('A' + (i - 1))).ToString() }))
                           .ConfigureAwait(false);

            // Generate owners
            Console.WriteLine("Generating owners...");
            await dbContext.Owners.AddRangeAsync(Enumerable.Range(1, 15).Select(i => new OwnerEntity { Name = $"Owner {i}" })).ConfigureAwait(false);

            // Generate species
            Console.WriteLine("Generating species...");
            await dbContext.Species.AddRangeAsync(Enumerable.Range(1, 50).Select(i => new SpeciesEntity { Name = $"Species {i}" })).ConfigureAwait(false);

            // Generate nesting boxes
            Console.WriteLine("Generating nesting boxes...");
            await dbContext.NestingBoxes.AddRangeAsync(Enumerable.Range(1, 400).Select(i => {
                RegionEntity region = dbContext.Regions.Local.GetRandomOrDefault();

                return new NestingBoxEntity {
                    Id = $"{region.NestingBoxIdPrefix}{i:00000}",
                    Region = region,
                    OldId = random.Next(10) >= 5 ? "Old ID" : null,
                    ForeignId = random.Next(10) >= 5 ? "Foreign ID" : null,
                    CoordinateLongitude = 9.724372 + region.Id + (random.NextDouble() - 0.5) / 100,
                    CoordinateLatitude = 52.353092 + (random.NextDouble() - 0.5) / 100,
                    HangUpDate = DateTime.UtcNow.AddMonths(-6 - random.Next(24)),
                    HangUpUser = user,
                    Owner = dbContext.Owners.Local.GetRandomOrDefault(),
                    Material = (Material)random.Next(4),
                    HoleSize = (HoleSize)random.Next(7),
                    ImageFileName = null,
                    Comment = "This is a generated nesting box for testing purposes."
                };
            })).ConfigureAwait(false);

            // Generate inspections
            Console.WriteLine("Generating inspections...");
            await dbContext.Inspections.AddRangeAsync(Enumerable.Range(1, 2000).Select(i => {
                NestingBoxEntity nestingBox = dbContext.NestingBoxes.Local.GetRandomOrDefault();

                return new InspectionEntity {
                    NestingBox = nestingBox,
                    InspectionDate = nestingBox.HangUpDate.GetValueOrDefault().AddMonths(random.Next(6)),
                    InspectedByUser = user,
                    HasBeenCleaned = false,
                    Condition = (Condition)random.Next(3),
                    JustRepaired = false,
                    Occupied = true,
                    ContainsEggs = true,
                    EggCount = random.Next(6),
                    ChickCount = random.Next(1, 4),
                    RingedChickCount = 1,
                    AgeInDays = random.Next(6),
                    FemaleParentBirdDiscovery = (ParentBirdDiscovery)random.Next(4),
                    MaleParentBirdDiscovery = (ParentBirdDiscovery)random.Next(4),
                    Species = dbContext.Species.Local.GetRandomOrDefault(),
                    ImageFileName = null,
                    Comment = "This is a generated inspection for testing purposes."
                };
            })).ConfigureAwait(false);

            Console.WriteLine("Saving sample data...");

            // Save changes
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            Console.WriteLine("Done.");
        }

        private static T GetRandomOrDefault<T>(this LocalView<T> localView) where T : class => localView.OrderBy(r => Guid.NewGuid()).FirstOrDefault();
    }
}
