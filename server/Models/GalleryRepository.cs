using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CodePaint.WebApi.Models {

    public class GalleryRepository : IGalleryRepository {
        private readonly IGalleryContext _context;

        public GalleryRepository(IGalleryContext context) {
            _context = context;
        }

        public async Task<IEnumerable<ThemeInfo>> GetAllThemesInfo() {
            return await _context
                .ThemesInfo
                .Find(_ => true)
                .ToListAsync();
        }

        public Task<ThemeInfo> GetThemeInfo(string id) {
            var filter = Builders<ThemeInfo>.Filter.Eq(m => m.Id, id);

            return _context
                .ThemesInfo
                .Find(filter)
                .FirstOrDefaultAsync();
        }

        public async Task Create(ThemeInfo themeInfo) {
            await _context.ThemesInfo.InsertOneAsync(themeInfo);
        }

        public async Task<bool> Update(ThemeInfo themeInfo) {
            ReplaceOneResult updateResult =
                await _context
                .ThemesInfo
                .ReplaceOneAsync(
                    filter: g => g.Id == themeInfo.Id,
                    replacement: themeInfo
                );

            return updateResult.IsAcknowledged &&
                updateResult.ModifiedCount > 0;
        }

        public async Task<bool> Delete(string id) {
            FilterDefinition<ThemeInfo> filter = Builders<ThemeInfo>.Filter.Eq(m => m.Id, id);
            DeleteResult deleteResult = await _context
                .ThemesInfo
                .DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged &&
                deleteResult.DeletedCount > 0;
        }
    }
}
