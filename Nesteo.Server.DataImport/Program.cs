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
using Nesteo.Server.Models;

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
            // await ClearDatabaseAsync(dbContext);

            UserEntity user = await dbContext.Users.FirstOrDefaultAsync().ConfigureAwait(false);
            if (user == null)
            {
                Console.WriteLine("Please run the server first to make sure that at least one user exists.");
                return;
            }

//            int nestingBoxExceptions = await ReadNestingBoxDataAsync(dbContext, user, home, "Stammdaten.csv");

            int inspectionExceptions = await ReadInspectionDataAsync(dbContext, user, home, "kontrolldaten.csv");

            //Console.WriteLine("Number of NestingBox Exceptions: {0}", nestingBoxExceptions);
            //Console.WriteLine("Number of Inspection Exceptions: {0}", inspectionExceptions);
            await SaveDatabaseAsync(dbContext);
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
                     await ImportNestingBoxDataAsync(dbContext, record, user);
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
                    await ImportInspectionDataAsync(dbContext, record, user);
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

        private static async Task ImportNestingBoxDataAsync (NesteoDbContext dbContext, Stammdaten record, UserEntity user)
        {

            // Get db record of owner and region
            OwnerEntity owner = GetOwnerEntity(dbContext, record);
            RegionEntity region = GetRegionEntity(dbContext, record);

            // Find Material and HoleSize enums
            Material material = GetMaterial(record.Material);
            HoleSize holeSize = GetHoleSize(record.Loch);

            NestingBoxEntity nestingBox = new NestingBoxEntity {
                Id = record.NistkastenNummer,
                Region = region,
                OldId = null,
                ForeignId = record.NummerFremd == " " ? null : record.NummerFremd,
                CoordinateLongitude = Convert.ToInt32(record.UTMHoch) / 100000.0,
                CoordinateLatitude = Convert.ToInt32(record.UTMRechts) / 100000.0,
                HangUpDate = Convert.ToDateTime(record.AufhangDatum),
                HangUpUser = user,
                Owner = owner,
                Material = material,
                HoleSize = holeSize,
                ImageFileName = null,
                Comment = record.Bemerkungen == " " ? null : record.Bemerkungen
            };

            // If nesting box is new, add, otherwise update
            if (!dbContext.NestingBoxes.AsEnumerable().Any(row => row.Id == record.NistkastenNummer))
            {
                dbContext.NestingBoxes.Add(nestingBox);
            }
            else
            {
                // row = nestingBoxEntity did not seem to work so each property must be assigned
                NestingBoxEntity row = dbContext.NestingBoxes.Single(n => n.Id == nestingBox.Id);
                row.Region = nestingBox.Region;
                row.OldId = nestingBox.OldId;
                row.ForeignId = nestingBox.ForeignId;
                row.CoordinateLongitude = nestingBox.CoordinateLongitude;
                row.CoordinateLatitude = nestingBox.CoordinateLatitude;
                row.HangUpDate = nestingBox.HangUpDate;
                row.HangUpUser = nestingBox.HangUpUser;
                row.Owner = nestingBox.Owner;
                row.Material = nestingBox.Material;
                row.HoleSize = nestingBox.HoleSize;
                row.ImageFileName = nestingBox.ImageFileName;
                row.Comment = nestingBox.Comment;
            }

            // TODO Exception handling or counting
        }

        private static RegionEntity GetRegionEntity(NesteoDbContext dbContext, Stammdaten record)
        {
            RegionEntity region;

            // If a region does not exist, add to db, otherwise get record.
            if (!dbContext.Regions.AsEnumerable().Any(row => row.Name == record.Ort))
            {
                region = new RegionEntity {Name = record.Ort, NestingBoxIdPrefix = record.Ort[0].ToString()};
                dbContext.Regions.Add(region);
            }
            else
            {
                region = dbContext.Regions.Single(r => r.Name == record.Ort);
            }

            return region;
        }

        private static OwnerEntity GetOwnerEntity(NesteoDbContext dbContext, Stammdaten record)
        {
            OwnerEntity owner;

            // If an owner does not exist, add to db, otherwise get record.
            if (!dbContext.Owners.AsEnumerable().Any(row => row.Name == record.Eigentumer))
            {
                owner = new OwnerEntity {Name = record.Eigentumer};
                dbContext.Owners.Add(owner);
            }
            else
            {
                owner = dbContext.Owners.Single(o => o.Name == record.Eigentumer);
            }

            return owner;
        }

        private static async Task<bool> ImportInspectionDataAsync (NesteoDbContext dbContext, Kontrolldaten record, UserEntity user)
        {

            // Get db record of nesting box and species
            NestingBoxEntity nestingBox = GetNestingBoxEntity(dbContext, record);
            if (nestingBox == null)
            {
                return false;
            }
            SpeciesEntity species = GetSpeciesEntity(dbContext, record);

            // Find condition enum
            Condition condition = GetCondition(record.ZustandKasten);

            try
            {
                // Create and add inspection
                InspectionEntity inspection = new InspectionEntity {
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
                    Species = species,
                    ImageFileName = null,
                    Comment = record.Bemerkungen == " " ? null : record.Bemerkungen
                };
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

        private static SpeciesEntity GetSpeciesEntity(NesteoDbContext dbContext, Kontrolldaten record)
        {
            SpeciesEntity species;

            // If an species does not exist, add to db, otherwise get record.
            if (!dbContext.Species.AsEnumerable().Any(row => row.Name == record.Vogelart))
            {
                species = new SpeciesEntity {Name = record.Vogelart};
                dbContext.Species.Add(species);
            }
            else
            {
                species = dbContext.Species.Single(o => o.Name == record.Vogelart);
            }

            return species;
        }

        private static NestingBoxEntity GetNestingBoxEntity(NesteoDbContext dbContext, Kontrolldaten record)
        {

            // If an nesting box does not exist, return blank, otherwise get record.
            if (!dbContext.NestingBoxes.AsEnumerable().Any(row => row.Id == record.NistkastenNummer))
            {
                return null;
            }
            else
            {
                return dbContext.NestingBoxes.Single(o => o.Id == record.NistkastenNummer);
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

        private static Condition GetCondition(string data)
        {
            Condition condition;

            if (data.ToLower().StartsWith("repariert") || data.ToLower().StartsWith("in ordnung"))
            {
                condition = Condition.Good;
            }
            else if (data.ToLower().StartsWith("leicht defekt"))
            {
                condition = Condition.NeedsRepair;
            }
            else
            {
                Console.WriteLine(data.ToLower());
                condition = Condition.NeedsReplacement;
            }

            return condition;
        }
    }
}
