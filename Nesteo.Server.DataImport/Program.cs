using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nesteo.Server.Data;
using Nesteo.Server.Data.Entities;
using Nesteo.Server.Data.Entities.Identity;
using Nesteo.Server.Data.Enums;
using CsvHelper;

namespace Nesteo.Server.DataImport
{
    public static class Program
    {

        public static async Task Main(string[] args)
        {


            // Get Home Directory
            string home = Directory.GetCurrentDirectory();
            home = home.Substring(0, home.Length - 23);

            // Data Prep. Change ",," to ", ,"
            ReplaceFileNulls(home, "Stammdaten.csv");
            ReplaceFileNulls(home, "kontrolldaten.csv");

            // Create host
            IHost host = Server.Program.CreateHostBuilder(args).Build();
            await Server.Program.PrepareHostAsync(host).ConfigureAwait(false);

            // Request database context
            NesteoDbContext dbContext = host.Services.GetRequiredService<NesteoDbContext>();

            // Clear database
//            await ClearDatabaseAsync(dbContext);

            UserEntity user = await dbContext.Users.FirstOrDefaultAsync().ConfigureAwait(false);
            if (user == null)
            {
                Console.WriteLine("Please run the server first to make sure that at least one user exists.");
                return;
            }

            //await ReadNestingBoxDataAsync(dbContext, user, home);
            //await SaveDatabaseAsync(dbContext);

            //await ReadInspectionDataAsync(dbContext, user, home);
            //await SaveDatabaseAsync(dbContext);
        }

        private static void ReplaceFileNulls(string home, string file)
        {
// Read csv and replace blanks, need two replace statements to catch all
            string replacenull = File.ReadAllText(home + "/Data/" + file);
            replacenull = replacenull.Replace(",,", ", ,");
            replacenull = replacenull.Replace(",,", ", ,");
            File.WriteAllText(home + "/Data/" + file, replacenull);
        }

        private static async Task SaveDatabaseAsync(NesteoDbContext dbContext)
        {
            Console.WriteLine("Saving");
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
            Console.WriteLine("Done.");
        }

        private static async Task ReadNestingBoxDataAsync(NesteoDbContext dbContext, UserEntity user, string home)
        {
            using (var reader = new StreamReader(home + "Data/Stammdaten-9.csv"))
            using (var csv = new CsvReader(reader))
            {
                var records = csv.GetRecords<Stammdaten>();

                foreach (var record in records)
                {
                    // Generate regions
                    await ImportRegionsAsync(dbContext, record);

                    // Generate owners
                    await ImportOwnersAsync(dbContext, record);

                    await ImportNestingBoxesAsync(dbContext, record, user);
                }
            }
        }

        private static async Task ReadInspectionDataAsync(NesteoDbContext dbContext, UserEntity user, string home)
        {
            using (var reader = new StreamReader(home + "Data/Kontrolldaten.csv"))
            using (var csv = new CsvReader(reader))
            {
                Console.WriteLine("Generating inspections...");
                var records = csv.GetRecords<Kontrolldaten>();
                foreach (var record in records)
                {
                    // Generate species
                    await ImportSpeciesAsync(dbContext, record);

                    await ImportInspectionsAsync(dbContext, record, user);
                }
            }
        }

        private static async Task ClearDatabaseAsync(NesteoDbContext dbContext)
        {
            // Safety check
            Console.Write("Do you want to fill the database with sample data? Please type 'yes': ");
            if (Console.ReadLine()?.ToLower() != "yes")
                return;
            Console.Write("Do you really want to do this? This will DELETE all existing data! Please type 'yes': ");
            if (Console.ReadLine()?.ToLower() != "yes")
                return;

            Console.WriteLine("Clearing database...");

            dbContext.Regions.RemoveRange(dbContext.Regions);
            dbContext.Owners.RemoveRange(dbContext.Owners);
            dbContext.Species.RemoveRange(dbContext.Species);
            dbContext.NestingBoxes.RemoveRange(dbContext.NestingBoxes);
            dbContext.ReservedIdSpaces.RemoveRange(dbContext.ReservedIdSpaces);
            dbContext.Inspections.RemoveRange(dbContext.Inspections);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        private static async Task ImportRegionsAsync(NesteoDbContext dbContext, Stammdaten record)
        {
            if (!dbContext.Regions.AsEnumerable().Any(row => record.Ort == row.Name))
            {
                dbContext.Regions.Add(new RegionEntity { Name = record.Ort, NestingBoxIdPrefix = record.Ort[0].ToString() });
                await SaveDatabaseAsync(dbContext);
            }
        }

        private static async Task ImportOwnersAsync(NesteoDbContext dbContext, Stammdaten record)
        {
            if (!dbContext.Owners.AsEnumerable().Any(row => record.Eigentumer == row.Name))
            {
                dbContext.Owners.Add(new OwnerEntity { Name = record.Eigentumer });
                await SaveDatabaseAsync(dbContext);
            }
        }

        private static async Task ImportSpeciesAsync(NesteoDbContext dbContext, Kontrolldaten record)
        {
            if (!dbContext.Species.AsEnumerable().Any(row => record.Vogelart == row.Name))
            {
                dbContext.Species.Add(new SpeciesEntity { Name = record.Vogelart });
                await SaveDatabaseAsync(dbContext);
            }
        }

        private static async Task ImportNestingBoxesAsync(NesteoDbContext dbContext, Stammdaten record, UserEntity user)
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
            if (record.Material.ToLower().StartsWith("holzbeton"))
            {
                material = Material.WoodConcrete;
            }
            else if (record.Material.ToLower() == "holz unbeschicht")
            {
                material = Material.UntreatedWood;
            }
            else if (record.Material.ToLower() == "holz beschicht")
            {
                material = Material.TreatedWood;
            }
            else
            {
                Console.WriteLine(record.Material.ToLower());
                material = Material.Other;
            }

            // Get HoleSize Value
            if (record.Loch.ToLower() == "sehr groß")
            {
                holeSize = HoleSize.VeryLarge;
            }
            else if (record.Loch.ToLower() == "groß")
            {
                holeSize = HoleSize.Large;
            }
            else if (record.Loch.ToLower() == "mittel")
            {
                holeSize = HoleSize.Medium;
            }
            else if (record.Loch.ToLower() == "klein")
            {
                holeSize = HoleSize.Small;
            }
            else if (record.Loch.ToLower() == "halbhöhle")
            {
                // TODO This may be wrong translation
                holeSize = HoleSize.Oval;
            }
            else
            {
                Console.WriteLine(record.Loch.ToLower());
                holeSize = HoleSize.Other;
            }

            try
            {
                //TODO Id is 6 digits while existing data is 1 letter and 5 digits
//            var existing = dbContext.NestingBoxes.Find($"{regionEntity.NestingBoxIdPrefix}{record.nistkastenNummer}");
                NestingBoxEntity nb = new NestingBoxEntity {
                    Id = $"{record.NistkastenNummer}",
//                Id = $"{regionEntity.NestingBoxIdPrefix}{record.nistkastenNummer}",
                    Region = regionEntity,
                    OldId = null,
                    ForeignId = record.NummerFremd == " " ? null : record.NummerFremd,
                    CoordinateLongitude = Convert.ToInt32(record.UTMHoch) / 100000.0,
                    CoordinateLatitude = Convert.ToInt32(record.UTMRechts) / 100000.0,
                    HangUpDate = Convert.ToDateTime(record.AufhangDatum),
                    HangUpUser = user,
                    Owner = ownerEntity,
                    Material = material,
                    HoleSize = holeSize,
                    ImageFileName = null,
                    Comment = record.Bemerkungen == " " ? null : record.Bemerkungen
                };
                if (!dbContext.NestingBoxes.AsEnumerable().Any(row => record.NistkastenNummer == row.Id))
                {
                    dbContext.NestingBoxes.Add(nb);
                }
                else
                {
                    var row = dbContext.NestingBoxes.Single(n => n.Id == nb.Id);
                    row.Region = nb.Region;
                    row.OldId = nb.OldId;
                    row.ForeignId = nb.ForeignId;
                    row.CoordinateLongitude = nb.CoordinateLongitude;
                    row.CoordinateLatitude = nb.CoordinateLatitude;
                    row.HangUpDate = nb.HangUpDate;
                    row.HangUpUser = nb.HangUpUser;
                    row.Owner = nb.Owner;
                    row.Material = nb.Material;
                    row.HoleSize = nb.HoleSize;
                    row.ImageFileName = nb.ImageFileName;
                    row.Comment = nb.Comment;
                }

                await SaveDatabaseAsync(dbContext);
            }
            catch (FormatException)
            {
                Console.WriteLine("Could not convert Nesting Box field to Int");
            }
        }

        private static async Task ImportInspectionsAsync(NesteoDbContext dbContext, Kontrolldaten record, UserEntity user)
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
            if (record.ZustandKasten.ToLower().StartsWith("repariert") || record.ZustandKasten.ToLower().StartsWith("in ordnung"))
            {
                condition = Condition.Good;
            }
            else if (record.ZustandKasten.ToLower().StartsWith("leicht defekt"))
            {
                condition = Condition.NeedsRepair;
            }
            else
            {
                Console.WriteLine(record.ZustandKasten.ToLower());
                condition = Condition.NeedsReplacement;
            }

            try
            {
                inspectionEntity = new InspectionEntity {

                    NestingBox = nestingBox ?? new NestingBoxEntity{Id = "0"},
                    InspectionDate = Convert.ToDateTime(record.Datum),
                    InspectedByUser = user,
                    HasBeenCleaned = record.Gereinigt.ToLower() == "ja",
                    Condition = condition,
                    JustRepaired = false,
                    Occupied = record.Besetzt.ToLower() == "ja",
                    ContainsEggs = record.AnzahlEier != " " && record.AnzahlEier != "0",
                    EggCount = record.AnzahlEier == " " ? 0 : Convert.ToInt32(record.AnzahlEier),
                    ChickCount = record.AnzahlJungvogel == " " ? 0 : Convert.ToInt32(record.AnzahlJungvogel),
                    RingedChickCount = record.Berignt == " " ? 0 : Convert.ToInt32(record.Berignt),
                    AgeInDays = record.AlterJungvogel == " " ? 0 : Convert.ToInt32(record.AlterJungvogel),
                    FemaleParentBirdDiscovery = ParentBirdDiscovery.None,
                    MaleParentBirdDiscovery = ParentBirdDiscovery.None,
                    Species = speciesEntity,
                    ImageFileName = null,
                    Comment = record.Bemerkungen
                };
                dbContext.Inspections.Add(inspectionEntity);
            }
            catch (FormatException)
            {
                Console.WriteLine("Could not convert Inspection field to Int");
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("An Inspection field is null");
            }

            await SaveDatabaseAsync(dbContext);
        }
    }
}



// var region = new regionentity(name = ".."}
// dbContect.Regions.Add(Region);
// var nesting = new nesting {Region = region}
