using System;
using System.Threading.Tasks;
using CodePaint.WebApi.Domain.Models;
using MongoDB.Driver;

namespace CodePaint.WebApi.Domain.Repositories
{
    public interface IVSCodeThemeStoreRepository
    {
        Task<VSCodeTheme> GetTheme(string id);
        Task Create(VSCodeTheme theme);
        Task<bool> Update(VSCodeTheme theme);
    }

    public class VSCodeThemeStoreRepository : IVSCodeThemeStoreRepository
    {
        private readonly IMongoCollection<VSCodeTheme> _vsCodeThemeStore;

        public VSCodeThemeStoreRepository(IGalleryContext context) =>
            _vsCodeThemeStore = context.VSCodeThemeStore;

        public Task<VSCodeTheme> GetTheme(string id)
        {
            var filter = Builders<VSCodeTheme>.Filter.Eq(m => m.Id, id);

            return _vsCodeThemeStore
                .Find(filter)
                .FirstOrDefaultAsync();
        }

        public async Task Create(VSCodeTheme theme) =>
            await _vsCodeThemeStore.InsertOneAsync(theme);

        public async Task<bool> Update(VSCodeTheme theme)
        {
            var updateResult = await _vsCodeThemeStore
                    .ReplaceOneAsync(
                        filter: g => g.Id == theme.Id,
                        replacement: theme
                    );

            return updateResult.IsAcknowledged
                && updateResult.ModifiedCount > 0;
        }
    }
}
