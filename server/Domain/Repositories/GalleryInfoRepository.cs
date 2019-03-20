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
        Task Create(GalleryItem themeInfo);
        Task<bool> Update(GalleryItem themeInfo);
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

        public async Task Create(GalleryItem themeInfo) => await _context.GalleryItems.InsertOneAsync(themeInfo);

        public async Task<bool> Update(GalleryItem themeInfo)
        {
            var updateResult =
                await _context.GalleryItems
                    .ReplaceOneAsync(
                        filter: g => g.Id == themeInfo.Id,
                        replacement: themeInfo
                    );

            return updateResult.IsAcknowledged
                && updateResult.ModifiedCount > 0;
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
