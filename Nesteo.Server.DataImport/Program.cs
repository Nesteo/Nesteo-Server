using System;
using System.IO;
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
using CsvHelper;
using CsvHelper.Configuration.Attributes;

namespace Nesteo.Server.DataImport
{
    public static class Program
    {

        public class Stammdaten
        {
            [Index(0)]
            public int nistkasten_nummer { get; set; }

            [Index(1)]
            public int nummer_fremd { get; set; }

            [Index(2)]
            public string ort { get; set; }

            [Index(3)]
            public string ort_detailliert { get; set; }

            [Index(4)]
            public int v0 { get; set; }

            [Index(5)]
            public int v1 { get; set; }

            [Index(6)]
            public int v2 { get; set; }

            [Index(7)]
            public int v3 { get; set; }

            [Index(8)]
            public int v4 { get; set; }

            [Index(9)]
            public int v5 { get; set; }

            [Index(10)]
            public int v6 { get; set; }

            [Index(11)]
            public int v7 { get; set; }

            [Index(12)]
            public string aufhang_datum { get; set; }

            [Index(13)]
            public string elgentumer { get; set; }

            [Index(14)]
            public string material { get; set; }

            [Index(15)]
            public string loch { get; set; }

            [Index(16)]
            public string bemerkungen { get; set; }

            [Index(17)]
            public string nistkasten_nummer_alt { get; set; }
        }

        public static async Task Main(string[] args)
        {

            using (var reader = new StreamReader("Data\\Stammdaten-2016_gesamt_0.csv"))
            using (var csv = new CsvReader(reader))
            {
                csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.ToLower();
                var records = csv.GetRecords<Stammdaten>();
            }
            /*
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

            // Clear database
            await clearDatabase(dbContext);

            var random = new Random();
            UserEntity user = await dbContext.Users.FirstOrDefaultAsync().ConfigureAwait(false);
            if (user == null)
            {
                Console.WriteLine("Please run the server first to make sure that at least one user exists.");
                return;
            }

            // Generate regions
            await generateRegions(dbContext);

            // Generate owners
            await generateOwners(dbContext);

            // Generate species
            await generateSpecies(dbContext);

            // Generate nesting boxes
            await generateNestingBoxes(dbContext, random, user);

            // Generate inspections
            await generateInspections(dbContext, random, user);

            Console.WriteLine("Saving sample data...");

            // Save changes
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            Console.WriteLine("Done.");
            */
        }

        private static async Task clearDatabase(NesteoDbContext dbContext)
        {
            Console.WriteLine("Clearing database...");

            dbContext.Regions.RemoveRange(dbContext.Regions);
            dbContext.Owners.RemoveRange(dbContext.Owners);
            dbContext.Species.RemoveRange(dbContext.Species);
            dbContext.NestingBoxes.RemoveRange(dbContext.NestingBoxes);
            dbContext.ReservedIdSpaces.RemoveRange(dbContext.ReservedIdSpaces);
            dbContext.Inspections.RemoveRange(dbContext.Inspections);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            Console.WriteLine("Generating sample data...");
        }

        private static async Task generateRegions(NesteoDbContext dbContext)
        {
            Console.WriteLine("Generating regions...");
            await dbContext.Regions.AddRangeAsync(
                               Enumerable.Range(1, 20).Select(i => new RegionEntity
                                                                  {Name = $"Owner {i}", NestingBoxIdPrefix = ((char) ('A' + (i - 1))).ToString()}))
                           .ConfigureAwait(false);
        }

        private static async Task generateOwners(NesteoDbContext dbContext)
        {
            Console.WriteLine("Generating owners...");
            await dbContext.Owners.AddRangeAsync(Enumerable.Range(1, 15).Select(i => new OwnerEntity {Name = $"Owner {i}"}))
                           .ConfigureAwait(false);
        }

        private static async Task generateSpecies(NesteoDbContext dbContext)
        {
            Console.WriteLine("Generating species...");
            await dbContext.Species
                           .AddRangeAsync(Enumerable.Range(1, 50).Select(i => new SpeciesEntity {Name = $"Species {i}"}))
                           .ConfigureAwait(false);
        }

        private static async Task generateNestingBoxes(NesteoDbContext dbContext, Random random, UserEntity user)
        {
            Console.WriteLine("Generating nesting boxes...");
            await dbContext.NestingBoxes.AddRangeAsync(Enumerable.Range(1, 400).Select(i =>
            {
                RegionEntity region = dbContext.Regions.Local.GetRandomOrDefault();

                return new NestingBoxEntity
                {
                    Id = $"{region.NestingBoxIdPrefix}{i:00000}",
                    Region = region,
                    OldId = random.Next(10) >= 5 ? "Old ID" : null,
                    ForeignId = random.Next(10) >= 5 ? "Foreign ID" : null,
                    CoordinateLongitude = 9.724372 + region.Id + (random.NextDouble() - 0.5) / 100,
                    CoordinateLatitude = 52.353092 + (random.NextDouble() - 0.5) / 100,
                    HangUpDate = DateTime.UtcNow.AddMonths(-6 - random.Next(24)),
                    HangUpUser = user,
                    Owner = dbContext.Owners.Local.GetRandomOrDefault(),
                    Material = (Material) random.Next(4),
                    HoleSize = (HoleSize) random.Next(7),
                    ImageFileName = null,
                    Comment = "This is a generated nesting box for testing purposes."
                };
            })).ConfigureAwait(false);
        }

        private static async Task generateInspections(NesteoDbContext dbContext, Random random, UserEntity user)
        {
            Console.WriteLine("Generating inspections...");
            await dbContext.Inspections.AddRangeAsync(Enumerable.Range(1, 2000).Select(i =>
            {
                NestingBoxEntity nestingBox = dbContext.NestingBoxes.Local.GetRandomOrDefault();

                return new InspectionEntity
                {
                    NestingBox = nestingBox,
                    InspectionDate = nestingBox.HangUpDate.GetValueOrDefault().AddMonths(random.Next(6)),
                    InspectedByUser = user,
                    HasBeenCleaned = false,
                    Condition = (Condition) random.Next(3),
                    JustRepaired = false,
                    Occupied = true,
                    ContainsEggs = true,
                    EggCount = random.Next(6),
                    ChickCount = random.Next(1, 4),
                    RingedChickCount = 1,
                    AgeInDays = random.Next(6),
                    FemaleParentBirdDiscovery = (ParentBirdDiscovery) random.Next(4),
                    MaleParentBirdDiscovery = (ParentBirdDiscovery) random.Next(4),
                    Species = dbContext.Species.Local.GetRandomOrDefault(),
                    ImageFileName = null,
                    Comment = "This is a generated inspection for testing purposes."
                };
            })).ConfigureAwait(false);
        }

        private static T GetRandomOrDefault<T>(this LocalView<T> localView) where T : class => localView.OrderBy(r => Guid.NewGuid()).FirstOrDefault();
    }
}
