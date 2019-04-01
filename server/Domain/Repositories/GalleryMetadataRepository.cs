using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodePaint.WebApi.Domain.Models;
using MongoDB.Driver;

namespace CodePaint.WebApi.Domain.Repositories
{
    public interface IGalleryMetadataRepository
    {
        Task<QueryResult<ExtensionMetadata>> GetItems(ExtensionsQuery query);
        Task<ExtensionMetadata> GetExtensionMetadata(string id);
        Task Create(ExtensionMetadata extensionMetadata);
        Task<bool> Update(ExtensionMetadata extensionMetadata);
        Task<bool> UpdateStatistics(string extensionId, Statistics statistics);
        Task<bool> ChangeExtensionType(string id, ExtensionType itemType);
        Task<bool> Delete(string id);
    }

    public class GalleryMetadataRepository : IGalleryMetadataRepository
    {
        private readonly IGalleryContext _context;

        public GalleryMetadataRepository(IGalleryContext context) => _context = context;

        public async Task<QueryResult<ExtensionMetadata>> GetItems(ExtensionsQuery query)
        {
            var filter = Builders<ExtensionMetadata>.Filter.Eq(extension => extension.Type, ExtensionType.Default);

            var totalCount = await _context.GalleryMetadata.Find(filter).CountDocumentsAsync();
            var items = await _context.GalleryMetadata
                .Find(filter)
                .Sort(query.GetSorting())
                .Skip(query.PageNumber * query.PageSize)
                .Limit(query.PageSize)
                .ToListAsync();

            return new QueryResult<ExtensionMetadata>
            {
                TotalCount = (int) totalCount,
                Items = items
            };
        }

        public Task<ExtensionMetadata> GetExtensionMetadata(string id)
        {
            var filter = Builders<ExtensionMetadata>.Filter.Eq(m => m.Id, id);

            return _context.GalleryMetadata
                .Find(filter)
                .FirstOrDefaultAsync();
        }

        public async Task Create(ExtensionMetadata extensionMetadata) =>
            await _context.GalleryMetadata.InsertOneAsync(extensionMetadata);

        public async Task<bool> Update(ExtensionMetadata extensionMetadata)
        {
            var filter = Builders<ExtensionMetadata>.Filter
                .Where(i => i.Id == extensionMetadata.Id);
            var updater = Builders<ExtensionMetadata>.Update
                .Set(i => i.Name, extensionMetadata.Name)
                .Set(i => i.DisplayName, extensionMetadata.DisplayName)
                .Set(i => i.Description, extensionMetadata.Description)
                .Set(i => i.PublisherName, extensionMetadata.PublisherName)
                .Set(i => i.PublisherDisplayName, extensionMetadata.PublisherDisplayName)
                .Set(i => i.Version, extensionMetadata.Version)
                .Set(i => i.LastUpdated, extensionMetadata.LastUpdated)
                .Set(i => i.IconDefault, extensionMetadata.IconDefault)
                .Set(i => i.IconSmall, extensionMetadata.IconSmall)
                .Set(i => i.AssetUri, extensionMetadata.AssetUri)
                .Set(i => i.Statistics.InstallCount, extensionMetadata.Statistics.InstallCount)
                .Set(i => i.Statistics.Downloads, extensionMetadata.Statistics.Downloads)
                .Set(i => i.Statistics.AverageRating, extensionMetadata.Statistics.AverageRating)
                .Set(i => i.Statistics.WeightedRating, extensionMetadata.Statistics.WeightedRating)
                .Set(i => i.Statistics.RatingCount, extensionMetadata.Statistics.RatingCount)
                .Set(i => i.Statistics.TrendingDaily, extensionMetadata.Statistics.TrendingDaily)
                .Set(i => i.Statistics.TrendingWeekly, extensionMetadata.Statistics.TrendingWeekly)
                .Set(i => i.Statistics.TrendingMonthly, extensionMetadata.Statistics.TrendingMonthly);

            var result = await _context.GalleryMetadata
                .UpdateOneAsync(filter, updater);

            return result.IsAcknowledged && result.ModifiedCount == 1;
        }

        public async Task<bool> UpdateStatistics(string extensionId, Statistics statistics)
        {
            var filter = Builders<ExtensionMetadata>.Filter
                .Where(i => i.Id == extensionId);
            var updater = Builders<ExtensionMetadata>.Update
                .Set(i => i.Statistics.InstallCount, statistics.InstallCount)
                .Set(i => i.Statistics.Downloads, statistics.Downloads)
                .Set(i => i.Statistics.AverageRating, statistics.AverageRating)
                .Set(i => i.Statistics.WeightedRating, statistics.WeightedRating)
                .Set(i => i.Statistics.RatingCount, statistics.RatingCount)
                .Set(i => i.Statistics.TrendingDaily, statistics.TrendingDaily)
                .Set(i => i.Statistics.TrendingWeekly, statistics.TrendingWeekly)
                .Set(i => i.Statistics.TrendingMonthly, statistics.TrendingMonthly);

            var result = await _context.GalleryMetadata
                .UpdateOneAsync(filter, updater);

            return result.IsAcknowledged && result.ModifiedCount == 1;
        }

        public async Task<bool> ChangeExtensionType(string id, ExtensionType itemType)
        {
            var filter = Builders<ExtensionMetadata>.Filter
                .Where(i => i.Id == id);
            var updater = Builders<ExtensionMetadata>.Update
                .Set(i => i.Type, itemType);

            var result = await _context.GalleryMetadata
                .UpdateOneAsync(filter, updater);

            return result.IsAcknowledged && result.ModifiedCount == 1;
        }

        public async Task<bool> Delete(string id)
        {
            FilterDefinition<ExtensionMetadata> filter = Builders<ExtensionMetadata>.Filter
                .Eq(m => m.Id, id);
            DeleteResult deleteResult = await _context.GalleryMetadata
                .DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged
                && deleteResult.DeletedCount > 0;
        }
    }
}
