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
            public string NistkastenNummer { get; set; }

            [Index(1)]
            public string NummerFremd { get; set; }

            [Index(2)]
            public string Ort { get; set; }

            [Index(3)]
            public string OrtDetailliert { get; set; }

            [Index(4)]
            public string UTMHoch { get; set; }

            [Index(5)]
            public string UTMRechts { get; set; }

            [Index(6)]
            public string AufhangDatum { get; set; }

            [Index(7)]
            public string Eigentumer{ get; set; }

            [Index(8)]
            public string Material { get; set; }

            [Index(9)]
            public string Loch { get; set; }

            [Index(10)]
            public string Bemerkungen { get; set; }

            [Index(11)]
            public string AktualisiertDatum { get; set; }
        }

        public class Kontrolldaten
        {
            [Index(0)]
            public string NistkastenNummer { get; set; }

            [Index(1)]
            public string Datum { get; set; }

            [Index(2)]
            public string ZustandKasten { get; set; }

            [Index(3)]
            public string Gereinigt { get; set; }

            [Index (4)]
            public string Besetzt { get; set; }

            [Index(5)]
            public string AnzahlEier { get; set; }

            [Index(6)]
            public string AnzahlJungvogel { get; set; }

            [Index(7)]
            public string AlterJungvogel { get; set; }

            [Index(8)]
            public string Vogelart { get; set; }

            [Index(9)]
            public string Berignt { get; set; }

            [Index(10)]
            public string Bemerkungen { get; set; }
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

            UserEntity user = await dbContext.Users.FirstOrDefaultAsync().ConfigureAwait(false);
            if (user == null)
            {
                Console.WriteLine("Please run the server first to make sure that at least one user exists.");
                return;
            }

            ReadStammDaten(dbContext, user);
            await SaveDatabase(dbContext);

            ReadKontrollDaten(dbContext, user);
            await SaveDatabase(dbContext);
        }

        private static async Task SaveDatabase(NesteoDbContext dbContext)
        {
            Console.WriteLine("Saving");
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
            Console.WriteLine("Done.");
        }

        private static async void ReadStammDaten(NesteoDbContext dbContext, UserEntity user)
        {
            using (var reader = new StreamReader("/home/randy/Nesteo-Server/Nesteo.Server.DataImport/Data/Stammdaten-9.csv"))
            using (var csv = new CsvReader(reader))
            {
                var records = csv.GetRecords<Stammdaten>();

                foreach (var record in records)
                {
                    // Generate regions
                    await GenerateRegions(dbContext, record);

                    // Generate owners
                    await GenerateOwners(dbContext, record);

                    GenerateNestingBoxes(dbContext, record, user);
                }
            }
        }

        private static void ReadKontrollDaten(NesteoDbContext dbContext, UserEntity user)
        {
            using (var reader = new StreamReader("/home/randy/Nesteo-Server/Nesteo.Server.DataImport/Data/Kontrolldaten.csv"))
            using (var csv = new CsvReader(reader))
            {
                Console.WriteLine("Generating inspections...");
                var records = csv.GetRecords<Kontrolldaten>();
                foreach (var record in records)
                {
                    // Generate species
                    GenerateSpecies(dbContext, record);

                    GenerateInspections(dbContext, record, user);
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

        private static async Task GenerateRegions(NesteoDbContext dbContext, Stammdaten record)
        {

            if (!dbContext.Regions.AsEnumerable().Any(row => record.Ort == row.Name))
            {
                dbContext.Regions.Add(new RegionEntity { Name = record.Ort, NestingBoxIdPrefix = record.Ort[0].ToString() });
                await SaveDatabase(dbContext);
            }

//            dbContext.Regions.Add(new RegionEntity{Name = "oythe", NestingBoxIdPrefix = "O" });
//            dbContext.Regions.Add(new RegionEntity{Name = "Bergstrup", NestingBoxIdPrefix = "B"});
//            dbContext.Regions.Add(new RegionEntity{Name = "Holzhausen", NestingBoxIdPrefix = "H"});
//            dbContext.Regions.Add(new RegionEntity{Name = "Vechta", NestingBoxIdPrefix = "V"});
//            dbContext.Regions.Add(new RegionEntity{Name = "Stoppelmarkt", NestingBoxIdPrefix = "S"});
//            dbContext.Regions.Add(new RegionEntity{Name = "Langförden", NestingBoxIdPrefix = "L"});
//            dbContext.Regions.Add(new RegionEntity{Name = "Holtrup", NestingBoxIdPrefix = "H"});
//            dbContext.Regions.Add(new RegionEntity{Name = "Langförden-Nord", NestingBoxIdPrefix = "L"});
        }

        private static async Task GenerateOwners(NesteoDbContext dbContext, Stammdaten record)
        {
            if (!dbContext.Owners.AsEnumerable().Any(row => record.Eigentumer == row.Name))
            {
                dbContext.Owners.Add(new OwnerEntity { Name = record.Eigentumer });
                await SaveDatabase(dbContext);
            }

//            Console.WriteLine("Generating owners...");
//            dbContext.Owners.Add(new OwnerEntity{Name = "NABU Vechta"});
//            dbContext.Owners.Add(new OwnerEntity{Name = "unbekannt"});
//            dbContext.Owners.Add(new OwnerEntity{Name = "Kreisjägerschaft Vechta"});
//            dbContext.Owners.Add(new OwnerEntity{Name = "Ludger Nerkamp"});
//            dbContext.Owners.Add(new OwnerEntity{Name = "Daniel Cobold"});
//            dbContext.Owners.Add(new OwnerEntity{Name = "Wilhelm Rieken"});
//            dbContext.Owners.Add(new OwnerEntity{Name = "Ursula Wilmering"});
//            dbContext.Owners.Add(new OwnerEntity{Name = "Kolleg St. Thomas"});
//            dbContext.Owners.Add(new OwnerEntity{Name = "Beringergemeinschaft Landkreis Vechta"});
//            dbContext.Owners.Add(new OwnerEntity{Name = "Beringergemeinschaft LK Vechta"});
//            dbContext.Owners.Add(new OwnerEntity{Name = "NABU Lohne"});
//            dbContext.Owners.Add(new OwnerEntity{Name = "von Frydag"});
//            dbContext.Owners.Add(new OwnerEntity{Name = "Ludger Ellert"});
    }

        private static async Task GenerateSpecies(NesteoDbContext dbContext, Kontrolldaten record)
        {
            if (!dbContext.Species.AsEnumerable().Any(row => record.Vogelart == row.Name))
            {
                dbContext.Species.Add(new SpeciesEntity { Name = record.Vogelart });
                await SaveDatabase(dbContext);
            }

//            Console.WriteLine("Generating species...");
//            dbContext.Species.Add(new SpeciesEntity { Name = "Amsel" });
//            dbContext.Species.Add(new SpeciesEntity { Name = "Bachstelze" });
//            dbContext.Species.Add(new SpeciesEntity { Name = "Baumläufer" });
//            dbContext.Species.Add(new SpeciesEntity { Name = "Blaumeise" });
//            dbContext.Species.Add(new SpeciesEntity { Name = "Buntspecht" });
//            dbContext.Species.Add(new SpeciesEntity { Name = "Dohle" });
//            dbContext.Species.Add(new SpeciesEntity { Name = "Feldsperling" });
//            dbContext.Species.Add(new SpeciesEntity { Name = "Gartenbaumläufer" });
//            dbContext.Species.Add(new SpeciesEntity { Name = "Gartenrotschwanz" });
//            dbContext.Species.Add(new SpeciesEntity { Name = "Grauschnäpper" });
//            dbContext.Species.Add(new SpeciesEntity { Name = "Haussperling" });
//            dbContext.Species.Add(new SpeciesEntity { Name = "Hohltaube" });
//            dbContext.Species.Add(new SpeciesEntity { Name = "Kleiber" });
//            dbContext.Species.Add(new SpeciesEntity { Name = "Kohlmeise" });
//            dbContext.Species.Add(new SpeciesEntity { Name = "Kohlmeise x Blaumeise" });
//            dbContext.Species.Add(new SpeciesEntity { Name = "Meise" });
//            dbContext.Species.Add(new SpeciesEntity { Name = "Meise unbestimmt" });
//            dbContext.Species.Add(new SpeciesEntity { Name = "Rotkehlchen" });
//            dbContext.Species.Add(new SpeciesEntity { Name = "Star" });
//            dbContext.Species.Add(new SpeciesEntity { Name = "Sumpfmeise" });
//            dbContext.Species.Add(new SpeciesEntity { Name = "Tannenmeise" });
//            dbContext.Species.Add(new SpeciesEntity { Name = "Trauerschnäpper" });
//            dbContext.Species.Add(new SpeciesEntity { Name = "Zaunkönig" });
//            dbContext.Species.Add(new SpeciesEntity { Name = "unbestimmt" });
//            dbContext.Species.Add(new SpeciesEntity { Name = "unbekannt" });

        }

        private static async Task GenerateNestingBoxes(NesteoDbContext dbContext, Stammdaten record, UserEntity user)
        {
            OwnerEntity ownerEntity;
            RegionEntity regionEntity;
            Material material;
            HoleSize holeSize;

            // Search for ownerId
            if (record.Eigentumer != " ")
            {
                ownerEntity = (from o in dbContext.Owners where o.Name == record.Eigentumer select o).FirstOrDefault();
            }
            else
            {
                ownerEntity = null;
            }

            // Search for regionId
            if (record.Ort != " ")
            {
                regionEntity = (from r in dbContext.Regions where r.Name == record.Ort select r).FirstOrDefault();
            }
            else
            {
                regionEntity = null;
            }

            // Get Material Value
            if (record.Material.StartsWith("Holbeton"))
            {
                material = Material.WoodConcrete;
            }
            else if (record.Material == "Holz unbeschicht")
            {
                material = Material.UntreatedWood;
            }
            else if (record.Material == "Holz beschicht")
            {
                material = Material.TreatedWood;
            }
            else
            {
                material = Material.Other;
            }

            // Get HoleSize Value
            if (record.Loch == "sehr groß")
            {
                holeSize = HoleSize.VeryLarge;
            }
            else if (record.Loch == "groß")
            {
                holeSize = HoleSize.Large;
            }
            else if (record.Loch == "mittel")
            {
                holeSize = HoleSize.Medium;
            }
            else if (record.Loch == "klein")
            {
                holeSize = HoleSize.Small;
            }
            else if (record.Loch == "Halbhöhle")
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
                Id = $"{record.NistkastenNummer}",
//                Id = $"{regionEntity.NestingBoxIdPrefix}{record.nistkastenNummer}",
                Region = regionEntity,
                OldId = null,
                ForeignId = record.NummerFremd == " "? null: record.NummerFremd,
                CoordinateLongitude = Convert.ToInt32(record.UTMHoch)/100000.0,
                CoordinateLatitude = Convert.ToInt32(record.UTMRechts)/100000.0,
                HangUpDate = Convert.ToDateTime(record.AufhangDatum),
                HangUpUser = user,
                Owner = ownerEntity,
                Material = material,
                HoleSize = holeSize,
                ImageFileName = null,
                Comment = record.Bemerkungen
            };
            dbContext.NestingBoxes.Add(nb);
        }

        private static async Task GenerateInspections(NesteoDbContext dbContext, Kontrolldaten record, UserEntity user)
        {
            NestingBoxEntity nestingBox;
            SpeciesEntity speciesEntity;
            Condition condition;
            InspectionEntity inspectionEntity;


            // Get nestingBoxId
            nestingBox = (from n in dbContext.NestingBoxes where n.Id == record.NistkastenNummer select n).FirstOrDefault();

            // Get speciesEntity
            speciesEntity = (from s in dbContext.Species where s.Name == record.Vogelart select s).FirstOrDefault();

            // Get Condition Value
            if (record.ZustandKasten.StartsWith("Repariert") || record.ZustandKasten.StartsWith("in Ordnung"))
            {
                condition = Condition.Good;
            }
            else if (record.ZustandKasten.StartsWith("leicht defekt"))
            {
                condition = Condition.NeedsRepair;
            }
            else
            {
                condition = Condition.NeedsReplacement;
            }

            inspectionEntity = new InspectionEntity {
                NestingBox = nestingBox.Equals(null) ? new NestingBoxEntity{Id = "0"} : nestingBox,
                InspectionDate = Convert.ToDateTime(record.Datum),
                InspectedByUser = user,
                HasBeenCleaned = record.Gereinigt.ToLower() == "ja",
                Condition = condition,
                JustRepaired = false,
                Occupied = record.Besetzt.ToLower() == "ja",
                ContainsEggs = record.AnzahlEier != " " && record.AnzahlEier != "0",
                EggCount = record.AnzahlEier == " "? 0 : Convert.ToInt32(record.AnzahlEier),
                ChickCount = record.AnzahlJungvogel == " "? 0 : Convert.ToInt32(record.AnzahlJungvogel),
                RingedChickCount = record.Berignt == " "? 0 : Convert.ToInt32(record.Berignt),
                AgeInDays = record.AlterJungvogel == " "? 0 : Convert.ToInt32(record.AlterJungvogel),
                FemaleParentBirdDiscovery = ParentBirdDiscovery.None,
                MaleParentBirdDiscovery = ParentBirdDiscovery.None,
                Species = speciesEntity,
                ImageFileName = null,
                Comment = record.Bemerkungen == " "? null: record.Bemerkungen

            };
            dbContext.Inspections.Add(inspectionEntity);
        }

        private static T GetRandomOrDefault<T>(this LocalView<T> localView) where T : class => localView.OrderBy(r => Guid.NewGuid()).FirstOrDefault();
    }
}
