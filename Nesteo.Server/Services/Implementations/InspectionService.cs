using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarcusW.SharpUtils.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Nesteo.Server.Data;
using Nesteo.Server.Data.Entities;
using Nesteo.Server.Data.Entities.Identity;
using Nesteo.Server.Models;

namespace Nesteo.Server.Services.Implementations
{
    public class InspectionService : CrudServiceBase<InspectionEntity, Inspection, int?>, IInspectionService
    {
        private readonly ILateDependency<INestingBoxService> _nestingBoxServiceDependency;

        public InspectionService(NesteoDbContext dbContext, IMapper mapper, ILateDependency<INestingBoxService> nestingBoxServiceDependency) : base(dbContext, mapper)
        {
            _nestingBoxServiceDependency = nestingBoxServiceDependency ?? throw new ArgumentNullException(nameof(nestingBoxServiceDependency));
        }

        public IAsyncEnumerable<InspectionPreview> GetAllPreviewsAsync()
        {
            return Entities.AsNoTracking().OrderBy(entity => entity.Id).ProjectTo<InspectionPreview>(Mapper.ConfigurationProvider).AsAsyncEnumerable();
        }

        public Task<InspectionPreview> FindPreviewByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return Entities.AsNoTracking().Where(entity => entity.Id == id).ProjectTo<InspectionPreview>(Mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
        }

        public IAsyncEnumerable<Inspection> GetAllForNestingBoxIdAsync(string nestingBoxId)
        {
            return Entities.AsNoTracking().OrderBy(entity => entity.Id).Where(entity => entity.NestingBox.Id == nestingBoxId).ProjectTo<Inspection>(Mapper.ConfigurationProvider)
                           .AsAsyncEnumerable();
        }

        public IAsyncEnumerable<InspectionPreview> GetAllPreviewsForNestingBoxIdAsync(string nestingBoxId)
        {
            return Entities.AsNoTracking().OrderBy(entity => entity.Id).Where(entity => entity.NestingBox.Id == nestingBoxId)
                           .ProjectTo<InspectionPreview>(Mapper.ConfigurationProvider).AsAsyncEnumerable();
        }

        public IAsyncEnumerable<string> ExportAllRowsAsync()
        {
            return Entities.AsNoTracking().OrderBy(entity => entity.Id).
                            ProjectTo<InspectionExportRow>(Mapper.ConfigurationProvider).
                            Select(row => string.Join(",", row.Id, row.NestingBoxId, row.InspectionDate, row.InspectedByUserName,
                                                      row.HasBeenCleaned, row.Condition, row.JustRepaired, row.Occupied, row.ContainsEggs,
                                                      row.EggCount, row.ChickCount, row.RingedChickCount, row.AgeInDays, row.FemaleParentBirdDiscovery,
                                                      row.MaleParentBirdDiscovery, row.SpeciesName, row.ImageFilename, row.Comment, row.LastUpdated)).
                            AsAsyncEnumerable().
                            Prepend(string.Join(",", "Id", "Nesting Box", "Inspection Date", "Inspection By",
                                                        "Has Been Cleaned", "Condition", "Just Repaired",
                                                        "Occupied", "Contains Eggs", "Egg Count", "Chick Count", "Ringed Bird Count", "Age (days)",
                                                        "Female Parent", "Male Parent", "Species", "Image Filename", "Comment", "Last Updated"));
        }

        public async Task<Inspection> AddAsync(Inspection inspection, CancellationToken cancellationToken = default)
        {
            if (inspection.Id != null)
                return null;

            // Add or update related entities
            SpeciesEntity speciesEntity = DbContext.Species.Update(Mapper.Map<SpeciesEntity>(inspection.Species)).Entity;

            // Retrieve existing user entity and nesting box. Updating these this way is not supported.
            NestingBoxEntity nestingBoxEntity = await DbContext.NestingBoxes.FindAsync(new object[] { inspection.NestingBox.Id }, cancellationToken).ConfigureAwait(false);
            if (nestingBoxEntity == null)
                return null;
            UserEntity inspectedByUserEntity = inspection.InspectedByUser != null
                ? await DbContext.Users.FindAsync(new object[] { inspection.InspectedByUser.Id }, cancellationToken).ConfigureAwait(false)
                : null;

            // Create inspection entry
            InspectionEntity inspectionEntity = Entities.Add(new InspectionEntity {
                NestingBox = nestingBoxEntity,
                InspectionDate = inspection.InspectionDate.GetValueOrDefault(),
                InspectedByUser = inspectedByUserEntity,
                HasBeenCleaned = inspection.HasBeenCleaned.GetValueOrDefault(),
                Condition = inspection.Condition.GetValueOrDefault(),
                JustRepaired = inspection.JustRepaired.GetValueOrDefault(),
                Occupied = inspection.Occupied.GetValueOrDefault(),
                ContainsEggs = inspection.ContainsEggs.GetValueOrDefault(),
                EggCount = inspection.EggCount,
                ChickCount = inspection.ChickCount.GetValueOrDefault(),
                RingedChickCount = inspection.RingedChickCount.GetValueOrDefault(),
                AgeInDays = inspection.AgeInDays,
                FemaleParentBirdDiscovery = inspection.FemaleParentBirdDiscovery.GetValueOrDefault(),
                MaleParentBirdDiscovery = inspection.MaleParentBirdDiscovery.GetValueOrDefault(),
                Species = speciesEntity,
                Comment = inspection.Comment
            }).Entity;

            // Save changes
            await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Mapper.Map<Inspection>(inspectionEntity);
        }

        public async Task<Inspection> UpdateAsync(Inspection inspection, CancellationToken cancellationToken = default)
        {
            if (inspection.Id == null)
                return null;

            // Add or update related entities
            SpeciesEntity speciesEntity = DbContext.Species.Update(Mapper.Map<SpeciesEntity>(inspection.Species)).Entity;

            // Retrieve existing user entity and nesting box. Updating these this way is not supported.
            NestingBoxEntity nestingBoxEntity = await DbContext.NestingBoxes.FindAsync(new object[] { inspection.NestingBox.Id }, cancellationToken).ConfigureAwait(false);
            if (nestingBoxEntity == null)
                return null;
            UserEntity inspectedByUserEntity = inspection.InspectedByUser != null
                ? await DbContext.Users.FindAsync(new object[] { inspection.InspectedByUser.Id }, cancellationToken).ConfigureAwait(false)
                : null;

            // Get existing inspection entity
            InspectionEntity inspectionEntity = await Entities.FindAsync(new object[] { inspection.Id }, cancellationToken).ConfigureAwait(false);
            if (inspectionEntity == null)
                return null;

            // Update values
            inspectionEntity.NestingBox = nestingBoxEntity;
            inspectionEntity.InspectionDate = inspection.InspectionDate.GetValueOrDefault();
            inspectionEntity.InspectedByUser = inspectedByUserEntity;
            inspectionEntity.HasBeenCleaned = inspection.HasBeenCleaned.GetValueOrDefault();
            inspectionEntity.Condition = inspection.Condition.GetValueOrDefault();
            inspectionEntity.JustRepaired = inspection.JustRepaired.GetValueOrDefault();
            inspectionEntity.Occupied = inspection.Occupied.GetValueOrDefault();
            inspectionEntity.ContainsEggs = inspection.ContainsEggs.GetValueOrDefault();
            inspectionEntity.EggCount = inspection.EggCount;
            inspectionEntity.ChickCount = inspection.ChickCount.GetValueOrDefault();
            inspectionEntity.RingedChickCount = inspection.RingedChickCount.GetValueOrDefault();
            inspectionEntity.AgeInDays = inspection.AgeInDays;
            inspectionEntity.FemaleParentBirdDiscovery = inspection.FemaleParentBirdDiscovery.GetValueOrDefault();
            inspectionEntity.MaleParentBirdDiscovery = inspection.MaleParentBirdDiscovery.GetValueOrDefault();
            inspectionEntity.Species = speciesEntity;
            inspectionEntity.Comment = inspection.Comment;

            // Save changes
            await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Mapper.Map<Inspection>(inspectionEntity);
        }

        public async Task<Inspection> SetImageFileNameAsync(int id, string imageFileName, CancellationToken cancellationToken = default)
        {
            if (id == null)
                return null;

            // Get existing nesting box entity
            InspectionEntity inspectionEntity = await Entities.FindAsync(new object[] { id }, cancellationToken).ConfigureAwait(false);
            if (inspectionEntity == null)
                return null;

            // Update values
            inspectionEntity.ImageFileName = imageFileName;

            // Save changes
            await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Mapper.Map<Inspection>(inspectionEntity);
        }

        public Task<string> GetImageFileNameAsync(int id, CancellationToken cancellationToken = default)
        {
            return Entities.AsNoTracking().Where(entity => entity.Id == id).Select(entity => entity.ImageFileName).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
