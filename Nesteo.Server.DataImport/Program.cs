using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CoordinateSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nesteo.Server.Data;
using Nesteo.Server.Data.Entities;
using Nesteo.Server.Data.Enums;
using CsvHelper;
using Microsoft.Extensions.Logging;
using Nesteo.Server.DataImport.Exceptions;
using Nesteo.Server.DataImport.RecordModels;

namespace Nesteo.Server.DataImport
{
    public static class Program
    {
        // TODO: Hardcoded for now. U32 is most of germany. Change when required.
        private const string UtmLatZ = "U";
        private const int UtmLongZ = 32;

        public static async Task Main(string[] args)
        {
            // Get file path
            Console.Write("Enter a file path: ");
            string filePath = Console.ReadLine();
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File does not exist. Exiting...");
                return;
            }

            // Get file type
            bool? isNestingBoxesFile;
            do
            {
                Console.Write("What's in this file? n:nesting-boxes / i:inspections > ");
                isNestingBoxesFile = Console.ReadLine() switch {
                    "n" => true,
                    "nesting-boxes" => true,
                    "i" => false,
                    "inspections" => false,
                    _ => (bool?)null
                };
            } while (isNestingBoxesFile == null);

            // Create host
            IHost host = Server.Program.CreateHostBuilder(args).ConfigureLogging(builder => {
                // Disable logging
                builder.ClearProviders();
            }).Build();
            await Server.Program.PrepareHostAsync(host).ConfigureAwait(false);

            // Request database context
            NesteoDbContext dbContext = host.Services.GetRequiredService<NesteoDbContext>();

            int importExceptions;
            if ((bool)isNestingBoxesFile)
                importExceptions = await ImportCsvFileAsync<NestingBoxRecord>(dbContext, filePath).ConfigureAwait(false);
            else
                importExceptions = await ImportCsvFileAsync<InspectionRecord>(dbContext, filePath).ConfigureAwait(false);

            // Do not save if exceptions exist
            if (importExceptions > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Import of {filePath} failed with {importExceptions} exceptions.");
                Console.ResetColor();
                return;
            }

            await SaveDatabaseAsync(dbContext).ConfigureAwait(false);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Import of {filePath} completed successfully.");
            Console.ResetColor();
        }

        private static async Task SaveDatabaseAsync(NesteoDbContext dbContext)
        {
            Console.WriteLine("Saving changes to database...");
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        private static async Task<int> ImportCsvFileAsync<TRecord>(NesteoDbContext dbContext, string filePath)
        {
            using var fileReader = new StreamReader(filePath);
            using var csvReader = new CsvReader(fileReader);

            // File is expected to have a header
            csvReader.Configuration.HasHeaderRecord = true;

            // Set field delimiter
            csvReader.Configuration.Delimiter = ",";

            // Don't throw errors when fields are empty/missing.
            csvReader.Configuration.MissingFieldFound = null;

            // Match header names case insensitively
            csvReader.Configuration.PrepareHeaderForMatch = (header, index) => header.ToLower();

            // Validate header
            Console.WriteLine("Validating file header...");
            csvReader.Read();
            csvReader.ReadHeader();
            csvReader.ValidateHeader<TRecord>();

            Console.WriteLine($"Importing data from {filePath} ...");

            int exceptions = 0;
            int fileLineNumber = 1;
            foreach (TRecord record in csvReader.GetRecords<TRecord>())
            {
                fileLineNumber++;
                Console.WriteLine($"Importing line {fileLineNumber}: {record.GetType().Name} {{{Utils.SerializeObjectProperties(record)}}}");

                try
                {
                    switch (record)
                    {
                        case NestingBoxRecord nestingBoxRecord:
                            await ImportNestingBoxRecordAsync(dbContext, nestingBoxRecord).ConfigureAwait(false);
                            break;
                        case InspectionRecord inspectionRecord:
                            await ImportInspectionRecordAsync(dbContext, inspectionRecord).ConfigureAwait(false);
                            break;
                        default:
                            throw new InvalidOperationException($"Unexpected record type: {record.GetType()}");
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  !! Import failed: {ex.Message}");
                    Console.ResetColor();
                    exceptions++;
                }
            }

            return exceptions;
        }

        private static async Task ImportNestingBoxRecordAsync(NesteoDbContext dbContext, NestingBoxRecord nestingBoxRecord)
        {
            // Check id length
            if (nestingBoxRecord.Id.Length != 6)
                throw new InvalidCsvRecordException("Id length is invalid. Expected 6 characters.");

            // Ensure the nesting box doesn't exist yet
            if (await dbContext.NestingBoxes.FindAsync(nestingBoxRecord.Id).ConfigureAwait(false) != null)
                throw new InvalidCsvRecordException("Nesting box exists and should not be overwritten.");

            // Create collection for comments
            var comments = new List<string>();
            if (!string.IsNullOrWhiteSpace(nestingBoxRecord.Comments))
                comments.AddRange(nestingBoxRecord.Comments.Split(',').Select(c => c.Trim()).Where(c => !string.IsNullOrEmpty(c)));

            // Ensure the owner entity exists
            OwnerEntity ownerEntity = await GetOrCreateEntityAsync(dbContext,
                                                                   owner => owner.Name == nestingBoxRecord.OwnerName,
                                                                   () => new OwnerEntity { Name = nestingBoxRecord.OwnerName }).ConfigureAwait(false);

            // Ensure the region entity exists
            string regionName = $"{nestingBoxRecord.RegionCityName} - {nestingBoxRecord.RegionDetailedName}";
            string ExtractIdPrefix() => nestingBoxRecord.Id[0].ToString();
            RegionEntity regionEntity = await GetOrCreateEntityAsync(dbContext,
                                                                     region => region.Name == regionName,
                                                                     () => new RegionEntity { Name = regionName, NestingBoxIdPrefix = ExtractIdPrefix() }).ConfigureAwait(false);

            // Map material type
            Material material = Material.Other;
            if (!string.IsNullOrWhiteSpace(nestingBoxRecord.Material))
            {
                material = GetMaterial(nestingBoxRecord.Material);
                if (material == Material.Other)
                    comments.Add($"Material: {nestingBoxRecord.Material}");
            }

            // Map hole size
            HoleSize holeSize = HoleSize.Other;
            if (!string.IsNullOrWhiteSpace(nestingBoxRecord.HoleSize))
            {
                holeSize = GetHoleSize(nestingBoxRecord.HoleSize);
                if (holeSize == HoleSize.Other)
                    comments.Add($"Lochgröße: {nestingBoxRecord.HoleSize}");
            }

            // Convert UTM to decimal coordinates
            Coordinate coordinate = null;
            if (!string.IsNullOrWhiteSpace(nestingBoxRecord.UtmEast) && !string.IsNullOrWhiteSpace(nestingBoxRecord.UtmNorth))
            {
                var utm = new UniversalTransverseMercator(UtmLatZ, UtmLongZ, double.Parse(nestingBoxRecord.UtmEast), double.Parse(nestingBoxRecord.UtmNorth));
                coordinate = UniversalTransverseMercator.ConvertUTMtoLatLong(utm);
            }

            // Analyze date info
            DateTime? hangUpDate = null;
            if (!string.IsNullOrWhiteSpace(nestingBoxRecord.HangUpDate))
            {
                hangUpDate = ParseDate(nestingBoxRecord.HangUpDate);
                if (hangUpDate.Value.Date == new DateTime(1999, 1, 1))
                    hangUpDate = null;
            }

            // Create nesting box
            dbContext.NestingBoxes.Add(new NestingBoxEntity {
                Id = nestingBoxRecord.Id,
                Region = regionEntity,
                OldId = null,
                ForeignId = string.IsNullOrWhiteSpace(nestingBoxRecord.ForeignId) ? null : nestingBoxRecord.ForeignId,
                CoordinateLongitude = coordinate?.Longitude.DecimalDegree,
                CoordinateLatitude = coordinate?.Latitude.DecimalDegree,
                HangUpDate = hangUpDate,
                HangUpUser = null,
                Owner = ownerEntity,
                Material = material,
                HoleSize = holeSize,
                ImageFileName = null,
                Comment = comments.Any() ? string.Join(", ", comments) : null,
                LastUpdated = string.IsNullOrWhiteSpace(nestingBoxRecord.DataUpdateDate) ? (hangUpDate ?? DateTime.UtcNow) : ParseDate(nestingBoxRecord.DataUpdateDate)
            });
        }

        private static async Task ImportInspectionRecordAsync(NesteoDbContext dbContext, InspectionRecord inspectionRecord)
        {
            // Create collection for comments
            var comments = new List<string>();
            if (!string.IsNullOrWhiteSpace(inspectionRecord.Comments))
                comments.AddRange(inspectionRecord.Comments.Split(',').Select(c => c.Trim()).Where(c => !string.IsNullOrEmpty(c)));

            // Get nesting box entity
            NestingBoxEntity nestingBoxEntity = await dbContext.NestingBoxes.FindAsync(inspectionRecord.NestingBoxId).ConfigureAwait(false);
            if (nestingBoxEntity == null)
                throw new InvalidCsvRecordException($"Nesting box {inspectionRecord.NestingBoxId} doesn't exist.");

            // Ensure the species entity exists
            SpeciesEntity speciesEntity = null;
            if (!string.IsNullOrWhiteSpace(inspectionRecord.SpeciesName) && !new[] { "unbestimmt", "unbekannt" }.Contains(inspectionRecord.SpeciesName.ToLower()))
            {
                speciesEntity = await GetOrCreateEntityAsync(dbContext,
                                                             species => species.Name == inspectionRecord.SpeciesName,
                                                             () => new SpeciesEntity { Name = inspectionRecord.SpeciesName }).ConfigureAwait(false);
            }

            // Analyze date info
            DateTime inspectionDate = string.IsNullOrWhiteSpace(inspectionRecord.Date)
                ? throw new InvalidCsvRecordException("Inspection date not set.")
                : ParseDate(inspectionRecord.Date);

            // Map nesting box condition
            (Condition condition, bool justRepaired) = GetCondition(inspectionRecord.Condition);

            // Analyze ringing activity
            (ParentBirdDiscovery femaleParentBirdDiscovery, ParentBirdDiscovery maleParentBirdDiscovery, int ringedChickCount) =
                AnalyzeRingingActivity(inspectionRecord.RingedCount);

            // Create inspection
            dbContext.Inspections.Add(new InspectionEntity {
                NestingBox = nestingBoxEntity,
                InspectionDate = inspectionDate,
                InspectedByUser = null,
                HasBeenCleaned = GetYesNo(inspectionRecord.HasBeenCleaned),
                Condition = condition,
                JustRepaired = justRepaired,
                Occupied = GetYesNoWithUnknown(inspectionRecord.Occupied),
                ContainsEggs = !string.IsNullOrWhiteSpace(inspectionRecord.EggCount),
                EggCount = ParseNumberWithUnknown(inspectionRecord.EggCount),
                ChickCount = ParseNumberWithUnknown(inspectionRecord.ChickCount),
                RingedChickCount = ringedChickCount,
                AgeInDays = ParseNumberWithUnknown(inspectionRecord.ChickAges),
                FemaleParentBirdDiscovery = femaleParentBirdDiscovery,
                MaleParentBirdDiscovery = maleParentBirdDiscovery,
                Species = speciesEntity,
                ImageFileName = null,
                Comment = comments.Any() ? string.Join(", ", comments) : null,
                LastUpdated = inspectionDate
            });
        }

        private static async Task<TEntity> GetOrCreateEntityAsync<TEntity>(NesteoDbContext dbContext, Expression<Func<TEntity, bool>> predicate, Func<TEntity> factoryFunc)
            where TEntity : class
        {
            // Get db set
            DbSet<TEntity> dbSet = dbContext.Set<TEntity>();

            // If the entity exists in the local snapshot, return it directly
            TEntity entity = dbSet.Local.FirstOrDefault(predicate.Compile());
            if (entity != null)
                return entity;

            // Search the entity in the database
            entity = await dbSet.FirstOrDefaultAsync(predicate).ConfigureAwait(false);
            if (entity != null)
                return entity;

            // Entity doesn't exist yet. Create a new one
            entity = factoryFunc.Invoke();
            return dbSet.Add(entity).Entity;
        }

        private static DateTime ParseDate(string value) => DateTime.Parse(value, new CultureInfo("en-US"));

        private static Material GetMaterial(string value)
        {
            return value.ToLower() switch {
                "holzbeton" => Material.WoodConcrete,
                "holzbetonludgeruswerk" => Material.WoodConcrete,
                "holz unbeschichtet" => Material.UntreatedWood,
                "holz beschichtet" => Material.TreatedWood,
                _ => throw new InvalidCsvRecordException($"Unrecognized material type: {value}")
            };
        }

        private static HoleSize GetHoleSize(string value)
        {
            return value.ToLower() switch {
                "sehr groß" => HoleSize.VeryLarge,
                "groß" => HoleSize.Large,
                "mittel" => HoleSize.Medium,
                "klein" => HoleSize.Small,
                "halbhöhle" => HoleSize.OpenFronted,
                "oval" => HoleSize.Oval,
                "zwei oval" => HoleSize.Oval,
                "baumläufer" => HoleSize.Other,
                "sonstiges" => HoleSize.Other,
                _ => throw new InvalidCsvRecordException($"Unrecognized hole size: {value}")
            };
        }

        private static (Condition condition, bool justRepaired) GetCondition(string value)
        {
            return value.ToLower() switch {
                "in ordnung" => (Condition.Good, false),
                "repariert" => (Condition.Good, true),
                "leicht defekt" => (Condition.NeedsRepair, false),
                "aufhängung defekt" => (Condition.NeedsRepair, false),
                "vorderfront gebrochen" => (Condition.NeedsRepair, false),
                "lässt sich nicht öffnen" => (Condition.NeedsRepair, false),
                "kann nicht geöffnet werden" => (Condition.NeedsRepair, false),
                "defekt" => (Condition.NeedsReplacement, false),
                "stark defekt" => (Condition.NeedsReplacement, false),
                "dach defekt" => (Condition.NeedsReplacement, false),
                "deckel defekt" => (Condition.NeedsReplacement, false),
                "front defekt" => (Condition.NeedsReplacement, false),
                "klappe war ab" => (Condition.NeedsReplacement, false),
                "front fehlt" => (Condition.NeedsReplacement, false),
                "frontplatte fehlt" => (Condition.NeedsReplacement, false),
                "boden fehlt" => (Condition.NeedsReplacement, false),
                "boden fehlt, abgenommen" => (Condition.NeedsReplacement, false),
                "kaputt" => (Condition.NeedsReplacement, false),
                _ => throw new InvalidCsvRecordException($"Unrecognized condition: {value}")
            };
        }

        private static bool? GetYesNoWithUnknown(string value)
        {
            return value.ToLower() switch {
                "unbekannt" => (bool?)null,
                _ => GetYesNo(value)
            };
        }

        private static bool GetYesNo(string value)
        {
            return value.ToLower() switch {
                "ja" => true,
                "nein" => false,
                _ => throw new InvalidCsvRecordException($"Unrecognized yes/no answer: {value}")
            };
        }

        private static int? ParseNumberWithUnknown(string value)
        {
            // I think the empty fields are just laziness
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            return value.ToLower() switch {
                "ja" => (int?)null,
                "unbekannt" => (int?)null,
                _ => int.Parse(value)
            };
        }

        private static (ParentBirdDiscovery femaleParentBirdDiscovery, ParentBirdDiscovery maleParentBirdDiscovery, int ringedChickCount) AnalyzeRingingActivity(string value)
        {
            value = value.ToLower();
            value = new[] { "+", ",", "und", "&", "juv" }.Aggregate(value, (current, str) => current.Replace(str, string.Empty));

            ParentBirdDiscovery femaleParentBirdDiscovery = ParentBirdDiscovery.None;
            ParentBirdDiscovery maleParentBirdDiscovery = ParentBirdDiscovery.None;

            // Check for parent bird discoveries and remove these information from the string
            if (value.Contains("weibchen"))
            {
                femaleParentBirdDiscovery = ParentBirdDiscovery.NewlyRinged;
                value = value.Replace("weibchen", string.Empty);
            }

            if (value.Contains("männchen"))
            {
                maleParentBirdDiscovery = ParentBirdDiscovery.NewlyRinged;
                value = value.Replace("männchen", string.Empty);
            }

            if (value.Contains("w"))
            {
                femaleParentBirdDiscovery = ParentBirdDiscovery.NewlyRinged;
                value = value.Replace("w", string.Empty);
            }

            if (value.Contains("m"))
            {
                maleParentBirdDiscovery = ParentBirdDiscovery.NewlyRinged;
                value = value.Replace("m", string.Empty);
            }

            // It's expected that only the ringed chick count (if any) is left now
            int ringedChickCount = string.IsNullOrWhiteSpace(value) ? 0 : int.Parse(value);

            return (femaleParentBirdDiscovery, maleParentBirdDiscovery, ringedChickCount);
        }
    }
}
