using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodePaint.WebApi.Domain.Models;
using MongoDB.Driver;

namespace CodePaint.WebApi.Domain.Repositories
{
    public interface IGalleryMetadataRepository
    {
        Task<IEnumerable<ExtensionMetadata>> GetAllItems();
        Task<ExtensionMetadata> GetExtensionMetadata(string id);
        Task Create(ExtensionMetadata extensionMetadata);
        Task<bool> Update(ExtensionMetadata extensionMetadata);
        Task<bool> ChangeExtensionType(string id, ExtensionType itemType);
        Task<bool> Delete(string id);
    }

    public class GalleryMetadataRepository : IGalleryMetadataRepository
    {
        private readonly IGalleryContext _context;

        public GalleryMetadataRepository(IGalleryContext context) => _context = context;

        public async Task<IEnumerable<ExtensionMetadata>> GetAllItems() =>
            await _context.GalleryMetadata
                .Find(_ => true)
                .ToListAsync();

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
                .Set(i => i.IconSmall, extensionMetadata.IconSmall);

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
