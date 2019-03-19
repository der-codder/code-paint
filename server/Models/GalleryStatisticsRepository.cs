using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CodePaint.WebApi.Models
{
    public interface IGalleryStatisticsRepository
    {
        Task<UpdateResult> UpdateThemeStatistics(ThemeStatistic statistics);
    }

    public class GalleryStatisticsRepository : IGalleryStatisticsRepository
    {
        private readonly IGalleryContext _context;

        public GalleryStatisticsRepository(IGalleryContext context) => _context = context;

        public async Task<UpdateResult> UpdateThemeStatistics(ThemeStatistic statistics)
        {
            var updateOptions = new UpdateOptions { IsUpsert = true };
            var filter = Builders<ThemeStatistic>.Filter
                .Where(s => s.ThemeId == statistics.ThemeId);
            var updater = Builders<ThemeStatistic>.Update
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
