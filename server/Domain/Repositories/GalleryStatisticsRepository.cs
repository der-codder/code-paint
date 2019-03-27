using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodePaint.WebApi.Domain.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CodePaint.WebApi.Domain.Repositories
{
    public interface IGalleryStatisticsRepository
    {
        Task<ExtensionStatistic> GetExtensionStatistic(string id);
        Task<UpdateResult> UpdateThemeStatistics(ExtensionStatistic statistics);
    }

    public class GalleryStatisticsRepository : IGalleryStatisticsRepository
    {
        private readonly IGalleryContext _context;

        public GalleryStatisticsRepository(IGalleryContext context) => _context = context;

        public Task<ExtensionStatistic> GetExtensionStatistic(string id)
        {
            var filter = Builders<ExtensionStatistic>.Filter.Eq(m => m.Id, id);

            return _context.GalleryStatistics
                .Find(filter)
                .FirstOrDefaultAsync();
        }

        public async Task<UpdateResult> UpdateThemeStatistics(ExtensionStatistic statistics)
        {
            var updateOptions = new UpdateOptions { IsUpsert = true };
            var filter = Builders<ExtensionStatistic>.Filter
                .Where(s => s.Id == statistics.Id);
            var updater = Builders<ExtensionStatistic>.Update
                .Set(s => s.InstallCount, statistics.InstallCount)
                .Set(s => s.UpdateCount, statistics.UpdateCount)
                .Set(s => s.AverageRating, statistics.AverageRating)
                .Set(s => s.WeightedRating, statistics.WeightedRating)
                .Set(s => s.RatingCount, statistics.RatingCount)
                .Set(s => s.TrendingDaily, statistics.TrendingDaily)
                .Set(s => s.TrendingWeekly, statistics.TrendingWeekly)
                .Set(s => s.TrendingMonthly, statistics.TrendingMonthly);

            return await _context.GalleryStatistics
                .UpdateOneAsync(filter, updater, updateOptions);
        }
    }
}
