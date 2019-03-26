using System;
using CodePaint.WebApi.Domain.Models;
using MongoDB.Driver;

namespace CodePaint.WebApi.Domain.Repositories
{
    public interface IGalleryContext
    {
        IMongoCollection<ExtensionMetadata> GalleryMetadata { get; }
        IMongoCollection<GalleryItemStatistic> GalleryStatistics { get; }
        IMongoCollection<VSCodeTheme> VSCodeThemeStore { get; }
    }

    public class GalleryContext : IGalleryContext
    {
        private readonly IMongoDatabase _db;

        public IMongoCollection<ExtensionMetadata> GalleryMetadata =>
            _db.GetCollection<ExtensionMetadata>("GalleryMetadata");

        public IMongoCollection<GalleryItemStatistic> GalleryStatistics =>
            _db.GetCollection<GalleryItemStatistic>("GalleryStatistics");

        public IMongoCollection<VSCodeTheme> VSCodeThemeStore =>
            _db.GetCollection<VSCodeTheme>("VSCodeThemeStore");

        public GalleryContext(MongoDBConfig config)
        {
            var client = new MongoClient(config.ConnectionString);
            _db = client.GetDatabase(config.Database);
        }
    }
}
