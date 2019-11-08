using System;
using System.Data;
using System.Drawing.Printing;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
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
            public string nistkastenNummer { get; set; }

            [Index(1)]
            public string nummerFremd { get; set; }

            [Index(2)]
            public string ort { get; set; }

            [Index(3)]
            public string ortDetailliert { get; set; }

            [Index(4)]
            public string utmHoch { get; set; }

            [Index(5)]
            public string utmRechts { get; set; }

            [Index(6)]
            public string aufhangDatum { get; set; }

            [Index(7)]
            public string eigentumer{ get; set; }

            [Index(8)]
            public string material { get; set; }

            [Index(9)]
            public string loch { get; set; }

            [Index(10)]
            public string bemerkungen { get; set; }

            [Index(11)]
            public string aktualisiertDatum { get; set; }
        }

        public class Kontrolldaten
        {
            [Index(0)]
            public string nistkastenNummer { get; set; }

            [Index(1)]
            public DateTime datum { get; set; }

            [Index(2)]
            public  string zustandKasten { get; set; }

            [Index(3)]
            public string gereinigt { get; set; }

            [Index (4)]
            public string besetzt { get; set; }

            [Index(5)]
            public string anzahlEier { get; set; }

            [Index(6)]
            public int anzahlJungvogel { get; set; }

            [Index(7)]
            public int alterJungvogel { get; set; }

            [Index(8)]
            public string vogelart { get; set; }

            [Index(9)]
            public int berignt { get; set; }

            [Index(10)]
            public string bemerkungen { get; set; }
        }

        public static async Task Main(string[] args)
        {

            // Create host
            IHost host = Server.Program.CreateHostBuilder(args).Build();
            await Server.Program.PrepareHostAsync(host).ConfigureAwait(false);

            // Request database context
            NesteoDbContext dbContext = host.Services.GetRequiredService<NesteoDbContext>();

            // Safety check
//            Console.Write("Do you want to fill the database with sample data? Please type 'yes': ");
//            if (Console.ReadLine()?.ToLower() != "yes")
//                return;
//            Console.Write("Do you really want to do this? This will DELETE all existing data! Please type 'yes': ");
//            if (Console.ReadLine()?.ToLower() != "yes")
//                return;

            // Clear database
//            await ClearDatabase(dbContext);

            // Generate regions
//            await GenerateRegions(dbContext);

            // Generate owners
//            await GenerateOwners(dbContext);

            // Generate species
//            await GenerateSpecies(dbContext);


            var random = new Random();
            UserEntity user = await dbContext.Users.FirstOrDefaultAsync().ConfigureAwait(false);
            if (user == null)
            {
                Console.WriteLine("Please run the server first to make sure that at least one user exists.");
                return;
            }



            using (var reader = new StreamReader("/home/randy/Nesteo-Server/Nesteo.Server.DataImport/Data/Stammdaten-9.csv"))
            using (var csv = new CsvReader(reader))
            {

                var records = csv.GetRecords<Stammdaten>();
                foreach (var record in records)
                {
                    // Search for ownerId
//                    if (record.eigentumer != " ")
//                    {
//                        OwnerEntity ownerEntity = (from o in dbContext.Owners where o.Name == record.eigentumer select o).FirstOrDefault();
//                        Console.WriteLine(ownerEntity.Id);
//                    }
//                    else
//                    {
//                        Console.WriteLine("Blank");
//                    }

                    // Search for regionId
                    if (record.ort != " ")
                    {
                        RegionEntity regionEntity = (from r in dbContext.Regions where r.Name == record.ort select r).FirstOrDefault();

                        Console.WriteLine(regionEntity.Id);
                    }
                    else
                    {
                        Console.WriteLine("Blank");
                    }


                }

//                foreach (var record in records)
//                {
////                    GenerateRegions(dbContext, record);
//                    GenerateNestingBoxes(dbContext, record, random, user);
//                }
            }

//             ReadKontrollDaten(dbContext, random, user);


            Console.WriteLine("saving");
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
            Console.WriteLine("Done.");
        }

        private static void ReadKontrollDaten(NesteoDbContext dbContext, Random random, UserEntity user)
        {
            using (var reader = new StreamReader("/home/randy/Nesteo-Server/Nesteo.Server.DataImport/Data/Kontrolldaten.csv"))
            using (var csv = new CsvReader(reader))
            {
                Console.WriteLine("Generating inspections...");
                var records = csv.GetRecords<Kontrolldaten>();
                foreach (var record in records)
                {
                    GenerateInspections(dbContext, record, random, user);
                }
            }
        }

        private static async Task ClearDatabase(NesteoDbContext dbContext)
        {
            Console.WriteLine("Clearing database...");

            dbContext.Regions.RemoveRange(dbContext.Regions);
            dbContext.Owners.RemoveRange(dbContext.Owners);
            dbContext.Species.RemoveRange(dbContext.Species);
            dbContext.NestingBoxes.RemoveRange(dbContext.NestingBoxes);
            dbContext.ReservedIdSpaces.RemoveRange(dbContext.ReservedIdSpaces);
            dbContext.Inspections.RemoveRange(dbContext.Inspections);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

        }

        private static async Task GenerateRegions(NesteoDbContext dbContext)
        {
//            Console.WriteLine("Generating regions...");
//            bool contains = dbContext.Regions.AsEnumerable().Any(row => record.ort == row.Name);//("Name = '" + record.ort + "'");
////            var existing = dbContext.Regions.Find(record.ort);
//            if (contains)
//            {
//                Console.WriteLine("Old");
////                dbContext.Regions.Add(new RegionEntity { Name = record.ort, NestingBoxIdPrefix = "X" });
//            }
//            else
//            {
//                Console.WriteLine("New");
////                Console.WriteLine("Old");//dbContext.Regions.Update(new RegionEntity { NestingBoxIdPrefix = "Y" });
//            }

//            dbContext.Regions.Add(new RegionEntity { Name = "oythe", NestingBoxIdPrefix = "X" });
//            dbContext.Regions.Add(new RegionEntity{Name = "Bergstrup", NestingBoxIdPrefix = "X"});
//            dbContext.Regions.Add(new RegionEntity{Name = "Holzhausen", NestingBoxIdPrefix = "X"});
//            dbContext.Regions.Add(new RegionEntity{Name = "Vechta", NestingBoxIdPrefix = "X"});
//            dbContext.Regions.Add(new RegionEntity{Name = "Stoppelmarkt", NestingBoxIdPrefix = "X"});
//            dbContext.Regions.Add(new RegionEntity{Name = "Langförden", NestingBoxIdPrefix = "X"});
//            dbContext.Regions.Add(new RegionEntity{Name = "Holtrup", NestingBoxIdPrefix = "X"});
//            dbContext.Regions.Add(new RegionEntity{Name = "Langförden-Nord", NestingBoxIdPrefix = "X"});
        }

        private static async Task GenerateOwners(NesteoDbContext dbContext)
        {
            Console.WriteLine("Generating owners...");
            int nextId = 136;
            dbContext.Owners.Add(new OwnerEntity{Id = nextId, Name = "NABU Vechta"});
            nextId++;
            dbContext.Owners.Add(new OwnerEntity{Id = nextId, Name = "unbekannt"});
            nextId++;
            dbContext.Owners.Add(new OwnerEntity{Id = nextId, Name = "Kreisjägerschaft Vechta"});
            nextId++;
            dbContext.Owners.Add(new OwnerEntity{Id = nextId, Name = "Ludger Nerkamp"});
            nextId++;
            dbContext.Owners.Add(new OwnerEntity{Id = nextId, Name = "Daniel Cobold"});
            nextId++;
            dbContext.Owners.Add(new OwnerEntity{Id = nextId, Name = "Wilhelm Rieken"});
            nextId++;
            dbContext.Owners.Add(new OwnerEntity{Id = nextId, Name = "Ursula Wilmering"});
            nextId++;
            dbContext.Owners.Add(new OwnerEntity{Id = nextId, Name = "Kolleg St. Thomas"});
            nextId++;
            dbContext.Owners.Add(new OwnerEntity{Id = nextId, Name = "Beringergemeinschaft Landkreis Vechta"});
            nextId++;
            dbContext.Owners.Add(new OwnerEntity{Id = nextId, Name = "Beringergemeinschaft LK Vechta"});
            nextId++;
            dbContext.Owners.Add(new OwnerEntity{Id = nextId, Name = "NABU Lohne"});
            nextId++;
            dbContext.Owners.Add(new OwnerEntity{Id = nextId, Name = "von Frydag"});
            nextId++;
            dbContext.Owners.Add(new OwnerEntity{Id = nextId, Name = "Ludger Ellert"});
    }

        private static async Task GenerateSpecies(NesteoDbContext dbContext)
        {
            Console.WriteLine("Generating species...");
            await dbContext.Species
                           .AddRangeAsync(Enumerable.Range(1, 50).Select(i => new SpeciesEntity {Name = $"Species {i}"}))
                           .ConfigureAwait(false);
        }

        private static async Task GenerateNestingBoxes(NesteoDbContext dbContext, Stammdaten record, Random random, UserEntity user)
        {
            Console.WriteLine("Generating nesting boxes...");
//            var existing = dbContext.NestingBoxes.Find(record.nistkastenNummer);
//            OwnerEntity owner = dbContext.Owners.Local.GetRandomOrDefault();
            RegionEntity region = dbContext.Regions.Local.GetRandomOrDefault();
            NestingBoxEntity nb = new NestingBoxEntity {
                Id = $"{record.nistkastenNummer}",
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
//                new NestingBoxEntity {
//                    Id = $"{record.nistkastenNummer}",
//                Region = region,
//                OldId = null,
//                ForeignId = null,//$"{record.nummerFremd}",
//                CoordinateLongitude = null,
//                CoordinateLatitude = null,
//                HangUpDate = null,//Convert.ToDateTime(record.aufhangDatum),
//                HangUpUser = user,
//                Owner = dbContext.Owners.Local.GetRandomOrDefault(),
//                Material = (Material)random.Next(4),
//                HoleSize = (HoleSize)random.Next(7),
//                ImageFileName = null,
//                Comment = null//record.bemerkungen
//            };
            dbContext.NestingBoxes.Add(nb);
        }

        private static async Task GenerateInspections(NesteoDbContext dbContext, Kontrolldaten record, Random random, UserEntity user)
        {
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
