using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodePaint.WebApi.Domain.Models;
using MongoDB.Driver;

namespace CodePaint.WebApi.Domain.Repositories
{
    public interface IGalleryItemsRepository
    {
        Task<IEnumerable<GalleryItem>> GetAllItems();
        Task<GalleryItem> GetGalleryItem(string id);
        Task Create(GalleryItem galleryItem);
        Task<bool> Update(GalleryItem galleryItem);
        Task<bool> ChangeGalleryItemType(string id, GalleryItemType itemType);
        Task<bool> Delete(string id);
    }

    public class GalleryItemsRepository : IGalleryItemsRepository
    {
        private readonly IGalleryContext _context;

        public GalleryItemsRepository(IGalleryContext context) => _context = context;

        public async Task<IEnumerable<GalleryItem>> GetAllItems() =>
            await _context.GalleryItems
                .Find(_ => true)
                .ToListAsync();

        public Task<GalleryItem> GetGalleryItem(string id)
        {
            var filter = Builders<GalleryItem>.Filter.Eq(m => m.Id, id);

            return _context.GalleryItems
                .Find(filter)
                .FirstOrDefaultAsync();
        }

        public async Task Create(GalleryItem galleryItem) => await _context.GalleryItems.InsertOneAsync(galleryItem);

        public async Task<bool> Update(GalleryItem galleryItem)
        {
            var filter = Builders<GalleryItem>.Filter
                .Where(i => i.Id == galleryItem.Id);
            var updater = Builders<GalleryItem>.Update
                .Set(i => i.Name, galleryItem.Name)
                .Set(i => i.DisplayName, galleryItem.DisplayName)
                .Set(i => i.Description, galleryItem.Description)
                .Set(i => i.PublisherName, galleryItem.PublisherName)
                .Set(i => i.PublisherDisplayName, galleryItem.PublisherDisplayName)
                .Set(i => i.Version, galleryItem.Version)
                .Set(i => i.LastUpdated, galleryItem.LastUpdated)
                .Set(i => i.IconDefault, galleryItem.IconDefault)
                .Set(i => i.IconSmall, galleryItem.IconSmall);

            var result = await _context.GalleryItems
                .UpdateOneAsync(filter, updater);

            return result.IsAcknowledged && result.ModifiedCount == 1;
        }

        public async Task<bool> ChangeGalleryItemType(string id, GalleryItemType itemType)
        {
            var filter = Builders<GalleryItem>.Filter
                .Where(i => i.Id == id);
            var updater = Builders<GalleryItem>.Update
                .Set(i => i.Type, itemType);

            var result = await _context.GalleryItems
                .UpdateOneAsync(filter, updater);

            return result.IsAcknowledged && result.ModifiedCount == 1;
        }

        public async Task<bool> Delete(string id)
        {
            FilterDefinition<GalleryItem> filter = Builders<GalleryItem>.Filter
                .Eq(m => m.Id, id);
            DeleteResult deleteResult = await _context.GalleryItems
                .DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged
                && deleteResult.DeletedCount > 0;
        }
    }
}
