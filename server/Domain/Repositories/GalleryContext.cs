using CodePaint.WebApi.Domain.Models;
using MongoDB.Driver;

namespace CodePaint.WebApi.Domain.Repositories
{
    public interface IGalleryContext
    {
        IMongoCollection<ExtensionMetadata> GalleryMetadata { get; }
        IMongoCollection<VSCodeTheme> VSCodeThemeStore { get; }
    }

    public class GalleryContext : IGalleryContext
    {
        private readonly IMongoDatabase _db;

        public IMongoCollection<ExtensionMetadata> GalleryMetadata =>
            _db.GetCollection<ExtensionMetadata>("GalleryMetadata");

        public IMongoCollection<VSCodeTheme> VSCodeThemeStore =>
            _db.GetCollection<VSCodeTheme>("VSCodeThemeStore");

        public GalleryContext(MongoDBConfig config)
        {
            var client = new MongoClient(config.ConnectionString);
            _db = client.GetDatabase(config.Database);

            SetupIndexes();
        }

        /// <summary>
        /// Sets up the database indexes for collections
        /// </summary>
        private void SetupIndexes()
        {
            var extensionMetadataBuilder = Builders<ExtensionMetadata>.IndexKeys;

            CreateIndexModel<ExtensionMetadata>[] indexModel = new[]
            {
                new CreateIndexModel<ExtensionMetadata>(
                    extensionMetadataBuilder.Descending(m => m.Statistics.Downloads)),
                new CreateIndexModel<ExtensionMetadata>(
                    extensionMetadataBuilder.Descending(m => m.LastUpdated)),
                new CreateIndexModel<ExtensionMetadata>(
                    extensionMetadataBuilder.Ascending(m => m.PublisherName)),
                new CreateIndexModel<ExtensionMetadata>(
                    extensionMetadataBuilder.Ascending(m => m.Name)),
                new CreateIndexModel<ExtensionMetadata>(
                    extensionMetadataBuilder.Descending(m => m.Statistics.AverageRating)),
                new CreateIndexModel<ExtensionMetadata>(
                    extensionMetadataBuilder.Descending(m => m.Statistics.TrendingWeekly))
            };
            GalleryMetadata.Indexes.CreateMany(indexModel);
        }
    }
}
