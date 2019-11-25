using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Nesteo.Server.Data;
using Nesteo.Server.Data.Entities;
using Nesteo.Server.Data.Entities.Identity;
using Nesteo.Server.IdGeneration;
using Nesteo.Server.Models;

namespace Nesteo.Server.Services.Implementations
{
    public class NestingBoxService : CrudServiceBase<NestingBoxEntity, NestingBox, string>, INestingBoxService
    {
        private readonly INestingBoxIdGenerator _nestingBoxIdGenerator;

        public NestingBoxService(NesteoDbContext dbContext, IMapper mapper, INestingBoxIdGenerator nestingBoxIdGenerator) : base(dbContext, mapper)
        {
            _nestingBoxIdGenerator = nestingBoxIdGenerator ?? throw new ArgumentNullException(nameof(nestingBoxIdGenerator));
        }

        public IAsyncEnumerable<NestingBoxPreview> GetAllPreviewsAsync()
        {
            return Entities.AsNoTracking().OrderBy(entity => entity.Id).ProjectTo<NestingBoxPreview>(Mapper.ConfigurationProvider).AsAsyncEnumerable();
        }

        public Task<NestingBoxPreview> FindPreviewByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return Entities.AsNoTracking().Where(entity => entity.Id == id).ProjectTo<NestingBoxPreview>(Mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
        }

        public IAsyncEnumerable<string> GetAllTakenIdsAsync()
        {
            return Entities.AsNoTracking().OrderBy(entity => entity.Id).Select(entity => entity.Id).AsAsyncEnumerable();
        }

        public IAsyncEnumerable<string> GetAllTakenIdsWithPrefixAsync(string regionPrefix)
        {
            return Entities.AsNoTracking().Where(entity => entity.Region.NestingBoxIdPrefix == regionPrefix).OrderBy(entity => entity.Id).Select(entity => entity.Id)
                           .AsAsyncEnumerable();
        }

        public IAsyncEnumerable<string> ExportAllRowsAsync()
        {
            return Entities.AsNoTracking().OrderBy(entity => entity.Id).
                            ProjectTo<NestingBoxExportRow>(Mapper.ConfigurationProvider).
                            Select(row => string.Join(",", row.Id, row.OldId, row.ForeignId, row.RegionName, row.CoordinateLongitude, row.CoordinateLatitude,
                                                      row.HangUpDate, row.HangUpUserName, row.OwnerName, row.Material, row.HoleSize, row.ImageFilename, row.Comment, row.LastUpdated)).
                            AsAsyncEnumerable().
                            Prepend(string.Join(",", "Id", "Old Id", "Foreign Id", "Region", "Longitude", "Latitude", "Hang Up Date", "Hung By",
                                                                   "Owner", "Material", "Hole Size", "Image Filename", "Comment", "Last Updated"));
        }

        public async Task<NestingBox> AddAsync(NestingBox nestingBox, CancellationToken cancellationToken = default)
        {
            if (nestingBox.Id == null)
            {
                // Generate a new ID
                nestingBox.Id = await _nestingBoxIdGenerator.GetNextIdsAsync(this, nestingBox.Region, 1, cancellationToken).SingleOrDefaultAsync(cancellationToken)
                                                            .ConfigureAwait(false);
            }
            else
            {
                // When an ID has been set, make sure it doesn't exist yet.
                if (await ExistsIdAsync(nestingBox.Id, cancellationToken).ConfigureAwait(false))
                    return null;
            }

            // Add or update related entities
            RegionEntity regionEntity = DbContext.Regions.Update(Mapper.Map<RegionEntity>(nestingBox.Region)).Entity;
            OwnerEntity ownerEntity = DbContext.Owners.Update(Mapper.Map<OwnerEntity>(nestingBox.Owner)).Entity;

            // Retrieve existing user entity. Updating users this way is not supported.
            UserEntity hangUpUserEntity = nestingBox.HangUpUser != null
                ? await DbContext.Users.FindAsync(new object[] { nestingBox.HangUpUser.Id }, cancellationToken).ConfigureAwait(false)
                : null;

            // Create nesting box entry
            NestingBoxEntity nestingBoxEntity = Entities.Add(new NestingBoxEntity {
                Id = nestingBox.Id,
                Region = regionEntity,
                OldId = nestingBox.OldId,
                ForeignId = nestingBox.ForeignId,
                CoordinateLongitude = nestingBox.CoordinateLongitude,
                CoordinateLatitude = nestingBox.CoordinateLatitude,
                HangUpDate = nestingBox.HangUpDate,
                HangUpUser = hangUpUserEntity,
                Owner = ownerEntity,
                Material = nestingBox.Material,
                HoleSize = nestingBox.HoleSize.GetValueOrDefault(),
                Comment = nestingBox.Comment
            }).Entity;

            // Save changes
            await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Mapper.Map<NestingBox>(nestingBoxEntity);
        }

        public async Task<NestingBox> UpdateAsync(NestingBox nestingBox, CancellationToken cancellationToken = default)
        {
            if (nestingBox.Id == null)
                return null;

            // Add or update related entities
            RegionEntity regionEntity = DbContext.Regions.Update(Mapper.Map<RegionEntity>(nestingBox.Region)).Entity;
            OwnerEntity ownerEntity = DbContext.Owners.Update(Mapper.Map<OwnerEntity>(nestingBox.Owner)).Entity;

            // Retrieve existing user entity. Updating users this way is not supported.
            UserEntity hangUpUserEntity = nestingBox.HangUpUser != null
                ? await DbContext.Users.FindAsync(new object[] { nestingBox.HangUpUser.Id }, cancellationToken).ConfigureAwait(false)
                : null;

            // Get existing nesting box entity
            NestingBoxEntity nestingBoxEntity = await Entities.FindAsync(new object[] { nestingBox.Id }, cancellationToken).ConfigureAwait(false);
            if (nestingBoxEntity == null)
                return null;

            // Update values
            nestingBoxEntity.Region = regionEntity;
            nestingBoxEntity.OldId = nestingBox.OldId;
            nestingBoxEntity.ForeignId = nestingBox.ForeignId;
            nestingBoxEntity.CoordinateLongitude = nestingBox.CoordinateLongitude;
            nestingBoxEntity.CoordinateLatitude = nestingBox.CoordinateLatitude;
            nestingBoxEntity.HangUpDate = nestingBox.HangUpDate;
            nestingBoxEntity.HangUpUser = hangUpUserEntity;
            nestingBoxEntity.Owner = ownerEntity;
            nestingBoxEntity.Material = nestingBox.Material;
            nestingBoxEntity.HoleSize = nestingBox.HoleSize.GetValueOrDefault();
            nestingBoxEntity.Comment = nestingBox.Comment;

            // Save changes
            await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Mapper.Map<NestingBox>(nestingBoxEntity);
        }

        public async Task<NestingBox> SetImageFileNameAsync(string id, string imageFileName, CancellationToken cancellationToken = default)
        {
            if (id == null)
                return null;

            // Get existing nesting box entity
            NestingBoxEntity nestingBoxEntity = await Entities.FindAsync(new object[] { id }, cancellationToken).ConfigureAwait(false);
            if (nestingBoxEntity == null)
                return null;

            // Update values
            nestingBoxEntity.ImageFileName = imageFileName;

            // Save changes
            await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Mapper.Map<NestingBox>(nestingBoxEntity);
        }

        public Task<string> GetImageFileNameAsync(string id, CancellationToken cancellationToken = default)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return Entities.AsNoTracking().Where(entity => entity.Id == id).Select(entity => entity.ImageFileName).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
