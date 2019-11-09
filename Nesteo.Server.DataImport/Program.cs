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
using Nesteo.Server.Models;

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

//            ReadStammDaten(dbContext, user);

//            ReadKontrollDaten(dbContext, random, user);


            Console.WriteLine("saving");
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
            Console.WriteLine("Done.");
        }

        private static void ReadStammDaten(NesteoDbContext dbContext, UserEntity user)
        {
            using (var reader = new StreamReader("/home/randy/Nesteo-Server/Nesteo.Server.DataImport/Data/Stammdaten-9.csv"))
            using (var csv = new CsvReader(reader))
            {
                var records = csv.GetRecords<Stammdaten>();

                foreach (var record in records)
                {
                    GenerateNestingBoxes(dbContext, record, user);
                }
            }
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

            dbContext.Regions.Add(new RegionEntity{Name = "oythe", NestingBoxIdPrefix = "X" });
            dbContext.Regions.Add(new RegionEntity{Name = "Bergstrup", NestingBoxIdPrefix = "X"});
            dbContext.Regions.Add(new RegionEntity{Name = "Holzhausen", NestingBoxIdPrefix = "X"});
            dbContext.Regions.Add(new RegionEntity{Name = "Vechta", NestingBoxIdPrefix = "X"});
            dbContext.Regions.Add(new RegionEntity{Name = "Stoppelmarkt", NestingBoxIdPrefix = "X"});
            dbContext.Regions.Add(new RegionEntity{Name = "Langförden", NestingBoxIdPrefix = "X"});
            dbContext.Regions.Add(new RegionEntity{Name = "Holtrup", NestingBoxIdPrefix = "X"});
            dbContext.Regions.Add(new RegionEntity{Name = "Langförden-Nord", NestingBoxIdPrefix = "X"});
        }

        private static async Task GenerateOwners(NesteoDbContext dbContext)
        {
            Console.WriteLine("Generating owners...");
            dbContext.Owners.Add(new OwnerEntity{Name = "NABU Vechta"});
            dbContext.Owners.Add(new OwnerEntity{Name = "unbekannt"});
            dbContext.Owners.Add(new OwnerEntity{Name = "Kreisjägerschaft Vechta"});
            dbContext.Owners.Add(new OwnerEntity{Name = "Ludger Nerkamp"});
            dbContext.Owners.Add(new OwnerEntity{Name = "Daniel Cobold"});
            dbContext.Owners.Add(new OwnerEntity{Name = "Wilhelm Rieken"});
            dbContext.Owners.Add(new OwnerEntity{Name = "Ursula Wilmering"});
            dbContext.Owners.Add(new OwnerEntity{Name = "Kolleg St. Thomas"});
            dbContext.Owners.Add(new OwnerEntity{Name = "Beringergemeinschaft Landkreis Vechta"});
            dbContext.Owners.Add(new OwnerEntity{Name = "Beringergemeinschaft LK Vechta"});
            dbContext.Owners.Add(new OwnerEntity{Name = "NABU Lohne"});
            dbContext.Owners.Add(new OwnerEntity{Name = "von Frydag"});
            dbContext.Owners.Add(new OwnerEntity{Name = "Ludger Ellert"});
    }

        private static async Task GenerateSpecies(NesteoDbContext dbContext)
        {
            Console.WriteLine("Generating species...");
            dbContext.Species.Add(new SpeciesEntity { Name = "Amsel" });
            dbContext.Species.Add(new SpeciesEntity { Name = "Bachstelze" });
            dbContext.Species.Add(new SpeciesEntity { Name = "Baumläufer" });
            dbContext.Species.Add(new SpeciesEntity { Name = "Blaumeise" });
            dbContext.Species.Add(new SpeciesEntity { Name = "Buntspecht" });
            dbContext.Species.Add(new SpeciesEntity { Name = "Dohle" });
            dbContext.Species.Add(new SpeciesEntity { Name = "Feldsperling" });
            dbContext.Species.Add(new SpeciesEntity { Name = "Gartenbaumläufer" });
            dbContext.Species.Add(new SpeciesEntity { Name = "Gartenrotschwanz" });
            dbContext.Species.Add(new SpeciesEntity { Name = "Grauschnäpper" });
            dbContext.Species.Add(new SpeciesEntity { Name = "Haussperling" });
            dbContext.Species.Add(new SpeciesEntity { Name = "Hohltaube" });
            dbContext.Species.Add(new SpeciesEntity { Name = "Kleiber" });
            dbContext.Species.Add(new SpeciesEntity { Name = "Kohlmeise" });
            dbContext.Species.Add(new SpeciesEntity { Name = "Kohlmeise x Blaumeise" });
            dbContext.Species.Add(new SpeciesEntity { Name = "Meise" });
            dbContext.Species.Add(new SpeciesEntity { Name = "Meise unbestimmt" });
            dbContext.Species.Add(new SpeciesEntity { Name = "Rotkehlchen" });
            dbContext.Species.Add(new SpeciesEntity { Name = "Star" });
            dbContext.Species.Add(new SpeciesEntity { Name = "Sumpfmeise" });
            dbContext.Species.Add(new SpeciesEntity { Name = "Tannenmeise" });
            dbContext.Species.Add(new SpeciesEntity { Name = "Trauerschnäpper" });
            dbContext.Species.Add(new SpeciesEntity { Name = "Zaunkönig" });
            dbContext.Species.Add(new SpeciesEntity { Name = "unbestimmt" });
            dbContext.Species.Add(new SpeciesEntity { Name = "unbekannt" });

        }

        private static async Task GenerateNestingBoxes(NesteoDbContext dbContext, Stammdaten record, UserEntity user)
        {
            OwnerEntity ownerEntity;
            RegionEntity regionEntity;
            Material material;
            HoleSize holeSize;

            // Search for ownerId
            if (record.eigentumer != " ")
            {
                ownerEntity = (from o in dbContext.Owners where o.Name == record.eigentumer select o).FirstOrDefault();
            }
            else
            {
                ownerEntity = null;
            }

            // Search for regionId
            if (record.ort != " ")
            {
                regionEntity = (from r in dbContext.Regions where r.Name == record.ort select r).FirstOrDefault();
            }
            else
            {
                regionEntity = null;
            }

            // Get Material Value
            if (record.material.StartsWith("Holbeton"))
            {
                material = Material.WoodConcrete;
            }
            else if (record.material == "Holz unbeschicht")
            {
                material = Material.UntreatedWood;
            }
            else if (record.material == "Holz beschicht")
            {
                material = Material.TreatedWood;
            }
            else
            {
                material = Material.Other;
            }

            // Get HoleSize Value
            if (record.loch == "sehr groß")
            {
                holeSize = HoleSize.VeryLarge;
            }
            else if (record.loch == "groß")
            {
                holeSize = HoleSize.Large;
            }
            else if (record.loch == "mittel")
            {
                holeSize = HoleSize.Medium;
            }
            else if (record.loch == "klein")
            {
                holeSize = HoleSize.Small;
            }
            else if (record.loch == "Halbhöhle")
            {
                // TODO This may be wrong translation
                holeSize = HoleSize.Oval;
            }
            else
            {
                holeSize = HoleSize.Other;
            }

            //TODO Id is 6 digits while existing data is 1 letter and 5 digits
//            var existing = dbContext.NestingBoxes.Find($"{regionEntity.NestingBoxIdPrefix}{record.nistkastenNummer}");
            NestingBoxEntity nb = new NestingBoxEntity {
                Id = $"{record.nistkastenNummer}",
//                Id = $"{regionEntity.NestingBoxIdPrefix}{record.nistkastenNummer}",
                Region = regionEntity,
                OldId = null,
                ForeignId = record.nummerFremd,
                CoordinateLongitude = Convert.ToInt32(record.utmHoch)/100000.0,
                CoordinateLatitude = Convert.ToInt32(record.utmRechts)/100000.0,
                HangUpDate = Convert.ToDateTime(record.aufhangDatum),
                HangUpUser = user,
                Owner = ownerEntity,
                Material = material,
                HoleSize = holeSize,
                ImageFileName = null,
                Comment = record.bemerkungen
            };
            dbContext.NestingBoxes.Add(nb);
        }

        private static async Task GenerateInspections(NesteoDbContext dbContext, Kontrolldaten record, Random random, UserEntity user)
        {
            NestingBoxEntity nestingBox;
            SpeciesEntity speciesEntity;
            Condition condition;
            InspectionEntity inspectionEntity;


            // Get nestingBoxId
            nestingBox = (from n in dbContext.NestingBoxes where n.Id == record.nistkastenNummer select n).FirstOrDefault();

            // Get speciesEntity
            speciesEntity = (from s in dbContext.Species where s.Name == record.vogelart select s).FirstOrDefault();

            // Get Condition Value
            if (record.zustandKasten.StartsWith("Repariert") || record.zustandKasten.StartsWith("in Ordnung"))
            {
                condition = Condition.Good;
            }
            else if (record.zustandKasten.StartsWith("leicht defekt"))
            {
                condition = Condition.NeedsRepair;
            }
            else
            {
                condition = Condition.NeedsReplacement;
            }

            inspectionEntity = new InspectionEntity {
                NestingBox = nestingBox,
                InspectionDate = Convert.ToDateTime(record.datum),
                InspectedByUser = user,
                HasBeenCleaned = record.gereinigt.ToLower() == "ja",
                Condition = condition,
                JustRepaired = false,
                Occupied = record.besetzt.ToLower() == "ja",
                ContainsEggs = record.anzahlEier != null,
                EggCount = Convert.ToInt32(record.anzahlEier),
                ChickCount = Convert.ToInt32(record.anzahlJungvogel),
                RingedChickCount = Convert.ToInt32(record.berignt),
                AgeInDays = Convert.ToInt32(record.alterJungvogel),
                FemaleParentBirdDiscovery = ParentBirdDiscovery.None,
                MaleParentBirdDiscovery = ParentBirdDiscovery.None,
                Species = speciesEntity,
                ImageFileName = null,
                Comment = record.bemerkungen
            };
            dbContext.Inspections.Add(inspectionEntity);
        }

        private static T GetRandomOrDefault<T>(this LocalView<T> localView) where T : class => localView.OrderBy(r => Guid.NewGuid()).FirstOrDefault();
    }
}
