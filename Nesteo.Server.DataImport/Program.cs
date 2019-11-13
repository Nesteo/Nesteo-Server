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
using CsvHelper.Expressions;

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
            //await ClearDatabaseAsync(dbContext);

            UserEntity user = await dbContext.Users.FirstOrDefaultAsync().ConfigureAwait(false);
            if (user == null)
            {
                Console.WriteLine("Please run the server first to make sure that at least one user exists.");
                return;
            }

           // int nestingBoxExceptions = await ReadNestingBoxDataAsync(dbContext, user, home, "Stammdaten.csv");

            int inspectionExceptions = await ReadInspectionDataAsync(dbContext, user, home, "kontrolldaten.csv");

            //Console.WriteLine("Number of NestingBox Exceptions: {0}", nestingBoxExceptions);
            Console.WriteLine("Number of Inspection Exceptions: {0}", inspectionExceptions);
        }

        private static void ReplaceFileNulls(string home, string file)
        {
            // Read csv and replace blanks, need two replace statements to catch all
            string replaceNull = File.ReadAllText(home + "/Data/" + file);
            replaceNull = replaceNull.Replace(",,", ", ,");
            replaceNull = replaceNull.Replace(",,", ", ,");
            File.WriteAllText(home + "/Data/" + file, replaceNull);
        }

        private static async Task SaveDatabaseAsync(NesteoDbContext dbContext)
        {
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        private static async Task<int> ReadNestingBoxDataAsync(NesteoDbContext dbContext, UserEntity user, string home, string file)
        {
            using (var reader = new StreamReader(home + "/Data/" + file))
            using (var csv = new CsvReader(reader))
            {
                int nestingBoxExceptions = 0;
                var records = csv.GetRecords<Stammdaten>();


                 foreach (var record in records)
                 {
                     // Import foreign data first
                     await ImportOwnersAsync(dbContext, record);
                     await ImportRegionsAsync(dbContext, record);

                     // Import NestingBox
                     bool valid = await ImportNestingBoxesAsync(dbContext, record, user);

                     if (!valid)
                     {
                         nestingBoxExceptions++;
                     }
                 }
                return nestingBoxExceptions;

            }
        }

        private static async Task<int> ReadInspectionDataAsync(NesteoDbContext dbContext, UserEntity user, string home, string file)
        {
            using (var reader = new StreamReader(home + "/Data/" + file))
            using (var csv = new CsvReader(reader))
            {
                int inspectionExceptions = 0;

                Console.WriteLine("Generating inspections...");
                var records = csv.GetRecords<Kontrolldaten>();

                foreach (var record in records)
                {
                    // Import foreign data first
                    //await ImportSpeciesAsync(dbContext, record);

                    // Import Inspections
                    bool valid = !await ImportInspectionsAsync(dbContext, record, user);

                    if (!valid)
                    {
                        inspectionExceptions++;
                    }
                }
                return inspectionExceptions;
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
            await dbContext.SaveChangesAsync();
        }

        private static async Task ImportNestingBoxData(NesteoDbContext dbContext, Stammdaten record, UserEntity user)
        {
            OwnerEntity ownerEntity;
            RegionEntity regionEntity;
            NestingBoxEntity nestingBoxEntity;
            Material material;
            HoleSize holeSize;

            // If an owner does not exist, add to db, otherwise get record.
            if (!dbContext.Owners.AsEnumerable().Any(row => row.Name == record.Eigentumer))
            {
                ownerEntity = new OwnerEntity { Name = record.Eigentumer };
                dbContext.Owners.Add(ownerEntity);
            }
            else
            {
                ownerEntity = dbContext.Owners.Single(o => o.Name == record.Eigentumer);
            }

            // If a region does not exist, add to db, otherwise get record.
            if (!dbContext.Regions.AsEnumerable().Any(row => row.Name == record.Ort))
            {
                regionEntity = new RegionEntity{ Name = record.Ort, NestingBoxIdPrefix = record.Ort[0].ToString()};
                dbContext.Regions.Add(regionEntity);
            }
            else
            {
                regionEntity = dbContext.Regions.Single(r => r.Name == record.Ort);
            }

            // Find Material and HoleSize enums
            material = GetMaterial(record.Material);
            holeSize = GetHoleSize(record.Loch);

            nestingBoxEntity = new NestingBoxEntity {
                Id = record.NistkastenNummer,
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

            // If nesting box is new, add, otherwise update
            if (!dbContext.NestingBoxes.AsEnumerable().Any(row => row.Id == record.NistkastenNummer))
            {
                dbContext.NestingBoxes.Add(nestingBoxEntity);
            }
            else
            {
                // row = nestingBoxEntity did not seem to work so each property must be assigned
                NestingBoxEntity row = dbContext.NestingBoxes.Single(n => n.Id == nestingBoxEntity.Id);
                row.Region = nestingBoxEntity.Region;
                row.OldId = nestingBoxEntity.OldId;
                row.ForeignId = nestingBoxEntity.ForeignId;
                row.CoordinateLongitude = nestingBoxEntity.CoordinateLongitude;
                row.CoordinateLatitude = nestingBoxEntity.CoordinateLatitude;
                row.HangUpDate = nestingBoxEntity.HangUpDate;
                row.HangUpUser = nestingBoxEntity.HangUpUser;
                row.Owner = nestingBoxEntity.Owner;
                row.Material = nestingBoxEntity.Material;
                row.HoleSize = nestingBoxEntity.HoleSize;
                row.ImageFileName = nestingBoxEntity.ImageFileName;
                row.Comment = nestingBoxEntity.Comment;
            }

            // TODO Exception handling or counting
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

        private static async Task<bool> ImportNestingBoxesAsync(NesteoDbContext dbContext, Stammdaten record, UserEntity user)
        {
            OwnerEntity ownerEntity;
            RegionEntity regionEntity;
            Material material;
            HoleSize holeSize;

            // Search for ownerId
            ownerEntity = (from o in dbContext.Owners where o.Name == record.Eigentumer select o).FirstOrDefault();

            // Search for regionId
            regionEntity = (from r in dbContext.Regions where r.Name == record.Ort select r).FirstOrDefault();

            material = GetMaterial(record.Material);

            holeSize = GetHoleSize(record.Loch);

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
                if (dbContext.NestingBoxes.AsEnumerable().All(row => record.NistkastenNummer != row.Id))
                {
                    dbContext.NestingBoxes.Add(nb);
                }
                else
                {
                    // Update existing record
                    NestingBoxEntity row = dbContext.NestingBoxes.Single(n => n.Id == nb.Id);
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
                return true;
            }
            catch (FormatException)
            {
                Console.WriteLine("Could not convert Nesting Box field to Int");
                return false;
            }
        }

        private static HoleSize GetHoleSize(string data)
        {
            HoleSize holeSize;

            if (data.ToLower() == "sehr groß")
            {
                holeSize = HoleSize.VeryLarge;
            }
            else if (data.ToLower() == "groß")
            {
                holeSize = HoleSize.Large;
            }
            else if (data.ToLower() == "mittel")
            {
                holeSize = HoleSize.Medium;
            }
            else if (data.ToLower() == "klein")
            {
                holeSize = HoleSize.Small;
            }
            else if (data.ToLower() == "halbhöhle")
            {
                // TODO This may be wrong translation
                holeSize = HoleSize.Oval;
            }
            else
            {
                Console.WriteLine(data.ToLower());
                holeSize = HoleSize.Other;
            }

            return holeSize;
        }

        private static Material GetMaterial(string data)
        {
            Material material;

            if (data.ToLower().StartsWith("holzbeton"))
            {
                material = Material.WoodConcrete;
            }
            else if (data.ToLower() == "holz unbeschicht")
            {
                material = Material.UntreatedWood;
            }
            else if (data.ToLower() == "holz beschicht")
            {
                material = Material.TreatedWood;
            }
            else
            {
                Console.WriteLine(data.ToLower());
                material = Material.Other;
            }

            return material;
        }

        private static async Task<bool> ImportInspectionsAsync(NesteoDbContext dbContext, Kontrolldaten record, UserEntity user)
        {
            NestingBoxEntity nestingBox;
            SpeciesEntity speciesEntity;
            Condition condition;
            InspectionEntity inspectionEntity;

            // Get nestingBoxId
            nestingBox = (from n in dbContext.NestingBoxes where n.Id == record.NistkastenNummer select n).FirstOrDefault();
            if (nestingBox == null)
            {
                // TODO log
                return false;
            }

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

                    NestingBox = nestingBox,
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
                await SaveDatabaseAsync(dbContext);
                return true;
            }
            catch (FormatException)
            {
                Console.WriteLine("Could not convert Inspection field to Int");
                return false;
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("An Inspection field is null");
                return false;
            }
        }
    }
}

// var region = new regionentity(name = ".."}
// dbContect.Regions.Add(Region);
// var nesting = new nesting {Region = region}
