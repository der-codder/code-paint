using System;
using CodePaint.WebApi.Domain.Models;
using MongoDB.Driver;

namespace CodePaint.WebApi.Domain.Repositories
{
    public interface IGalleryContext
    {
        IMongoCollection<ThemeInfo> GalleryInfo { get; }
        IMongoCollection<ThemeStatistic> GalleryStatistics { get; }
        IMongoCollection<ColorTheme> GalleryStore { get; }
    }

    public class GalleryContext : IGalleryContext
    {
        private readonly IMongoDatabase _db;

        public IMongoCollection<ThemeInfo> GalleryInfo =>
            _db.GetCollection<ThemeInfo>("GalleryInfo");

        public IMongoCollection<ThemeStatistic> GalleryStatistics =>
            _db.GetCollection<ThemeStatistic>("GalleryStatistics");

        public IMongoCollection<ColorTheme> GalleryStore =>
            _db.GetCollection<ColorTheme>("GalleryStore");

        public GalleryContext(MongoDBConfig config)
        {
            var client = new MongoClient(config.ConnectionString);
            _db = client.GetDatabase(config.Database);
        }
    }
}
