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

            // Get Filename
            Console.Write("Enter a filename: ");
            string filename = Console.ReadLine();

            // Data Prep. Change ",," to ", ,"
            try
            {
                ReplaceFileNulls(home, filename);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File does not exist. Exiting...");
                return;
            }

            // Get filetype
            Console.Write("Is this a Nesting Box file? (Y?) ");
            string filetype = Console.ReadLine();

            // Create host
            IHost host = Server.Program.CreateHostBuilder(args).Build();
            await Server.Program.PrepareHostAsync(host).ConfigureAwait(false);

            // Request database context
            NesteoDbContext dbContext = host.Services.GetRequiredService<NesteoDbContext>();
//            ClearDatabaseAsync(dbContext);

            UserEntity user = await dbContext.Users.FirstOrDefaultAsync().ConfigureAwait(false);
            if (user == null)
            {
                Console.WriteLine("Please run the server first to make sure that at least one user exists.");
                return;
            }

            int exceptions = 0;
            if (filetype == "Y")
            {
                exceptions = await ReadNestingBoxDataAsync(dbContext, user, home, filename);
                Console.WriteLine("Number of NestingBox Exceptions: {0}", exceptions);

                // Do not save if exceptions exist
                if (exceptions == 0)
                {
                    await SaveDatabaseAsync(dbContext);
                }
                else
                {
                    Console.WriteLine("Errors exist. Cannot import {0}", filename);
                }
            }
            else
            {
                exceptions = await ReadInspectionDataAsync(dbContext, user, home, filename);
                Console.WriteLine("Number of Inspection Exceptions: {0}", exceptions);

                // Do not save if exceptions exist
                if (exceptions == 0)
                {
                    await SaveDatabaseAsync(dbContext);
                }
                else
                {
                    Console.WriteLine("Errors exist. Cannot import {0}", filename);
                }
            }
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
            Console.WriteLine("Saving");
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
            Console.WriteLine("Done");
        }

        private static async Task<int> ReadNestingBoxDataAsync(NesteoDbContext dbContext, UserEntity user, string home, string file)
        {
            using (var reader = new StreamReader(home + "/Data/" + file))
            using (var csv = new CsvReader(reader))
            {
                int nestingBoxExceptions = 0;

                Console.WriteLine("Generating Nesting Boxes...");
                var records = csv.GetRecords<Stammdaten>();

                int index = 2;
                foreach (var record in records)
                {
                    if (!await ImportNestingBoxDataAsync(dbContext, record, user, index))
                    {
                        nestingBoxExceptions++;
                    }
                    index++;
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

                int index = 2;
                foreach (var record in records)
                {
                    if (!await ImportInspectionDataAsync(dbContext, record, user, index))
                    {
                        inspectionExceptions++;
                    }
                    index++;
                }
                return inspectionExceptions;
            }
        }

        private static async Task ClearDatabaseAsync(NesteoDbContext dbContext)
        {
            // Safety check
            Console.Write("Do you want to clear the database? Please type 'yes': ");
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

        private static async Task<bool> ImportNestingBoxDataAsync (NesteoDbContext dbContext, Stammdaten record, UserEntity user, int index)
        {

            // Get db record of owner and region
            OwnerEntity owner = GetOwnerEntity(dbContext, record);
            RegionEntity region = GetRegionEntity(dbContext, record);

            // Find Material and HoleSize enums
            Material material = GetMaterial(record.Material);
            HoleSize holeSize = GetHoleSize(record.Loch);

            try
            {
                NestingBoxEntity nestingBox = new NestingBoxEntity {
                    Id = record.NistkastenNummer,
                    Region = region,
                    OldId = null,
                    ForeignId = record.NummerFremd == " " ? null : record.NummerFremd,
                    CoordinateLongitude = record.UTMHoch == " "? 0.0 : Convert.ToInt32(record.UTMHoch) / 100000.0,
                    CoordinateLatitude = record.UTMRechts == " "? 0.0 : Convert.ToInt32(record.UTMRechts) / 100000.0,
                    HangUpDate = record.AufhangDatum == " "? Convert.ToDateTime("01/01/1901") : Convert.ToDateTime(record.AufhangDatum),
                    HangUpUser = user,
                    Owner = owner,
                    Material = material,
                    HoleSize = holeSize,
                    ImageFileName = null,
                    Comment = record.Bemerkungen == " " ? null : record.Bemerkungen
                };

                // If nesting box is new, add, otherwise update
                if (!dbContext.NestingBoxes.Local.AsEnumerable().Any(row => row.Id == nestingBox.Id) &&
                    !dbContext.NestingBoxes.AsEnumerable().Any(row => row.Id == nestingBox.Id))
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

                return true;
            }
            catch (FormatException)
            {
                Console.WriteLine("Could not convert a Nesting Box field to Int \tRow:{0} \tId:{1} \tLocation:{2} \tOwner:{3}",
                                  index, record.NistkastenNummer, record.Ort, record.Eigentumer);
                return false;
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("A Nesting box field is null \tRow:{0} \tId:{1} \tLocation:{2} \tOwner:{3}",
                                  index, record.NistkastenNummer, record.Ort, record.Eigentumer);
                return false;
            }
        }

        private static RegionEntity GetRegionEntity(NesteoDbContext dbContext, Stammdaten record)
        {
            RegionEntity region;

            // If a region does not exist in local and actual, add to db, otherwise get record.
            if (!dbContext.Regions.Local.AsEnumerable().Any(row => row.Name == record.Ort) &&
                !dbContext.Regions.AsEnumerable().Any(row => row.Name == record.Ort))
            {
                region = new RegionEntity {Name = record.Ort, NestingBoxIdPrefix = record.Ort[0].ToString()};
                dbContext.Regions.Local.Add(region);
            }
            else
            {
                region = dbContext.Regions.Local.Single(r => r.Name == record.Ort);
            }

            return region;
        }

        private static OwnerEntity GetOwnerEntity(NesteoDbContext dbContext, Stammdaten record)
        {
            OwnerEntity owner;

            // If an owner does not exist in local and actual, add to db, otherwise get record.
            if (!dbContext.Owners.Local.AsEnumerable().Any(row => row.Name == record.Eigentumer) &&
                !dbContext.Owners.AsEnumerable().Any(row => row.Name == record.Eigentumer))
            {
                owner = new OwnerEntity {Name = record.Eigentumer};
                dbContext.Owners.Local.Add(owner);
            }
            else
            {
                owner = dbContext.Owners.Local.Single(o => o.Name == record.Eigentumer);
            }

            return owner;
        }

        private static async Task<bool> ImportInspectionDataAsync (NesteoDbContext dbContext, Kontrolldaten record, UserEntity user, int index)
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
                dbContext.Inspections.Local.Add(inspection);
                return true;
            }
            catch (FormatException)
            {
                Console.WriteLine("Could not convert an Inspection field to Int \tRow:{0} \tId:{1} \tDate:{2} \tCondition:{3} \tSpecie:{4}",
                                  index, record.NistkastenNummer, record.Datum, record.ZustandKasten, record.Vogelart);
                return false;
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("An Inspection field is null \tRow:{0} \tId:{1} \tDate:{2} \tCondition:{3} \tSpecie:{4}",
                                  index, record.NistkastenNummer, record.Datum, record.ZustandKasten, record.Vogelart);
                return false;
            }
        }

        private static SpeciesEntity GetSpeciesEntity(NesteoDbContext dbContext, Kontrolldaten record)
        {
            SpeciesEntity species;

            // If an species does not exist in local and actual, add to db, otherwise get record.
            if (!dbContext.Species.Local.AsEnumerable().Any(row => row.Name == record.Vogelart) &&
                !dbContext.Species.AsEnumerable().Any(row => row.Name == record.Vogelart))
            {
                species = new SpeciesEntity {Name = record.Vogelart};
                dbContext.Species.Local.Add(species);
            }
            else
            {
                species = dbContext.Species.Local.Single(o => o.Name == record.Vogelart);
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
            return dbContext.NestingBoxes.Single(o => o.Id == record.NistkastenNummer);
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
//                Console.WriteLine(data.ToLower());
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
//                Console.WriteLine(data.ToLower());
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
//                Console.WriteLine(data.ToLower());
                condition = Condition.NeedsReplacement;
            }

            return condition;
        }
    }
}
