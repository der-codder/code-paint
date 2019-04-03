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
                    extensionMetadataBuilder.Ascending(m => m.PublisherDisplayName)),
                new CreateIndexModel<ExtensionMetadata>(
                    extensionMetadataBuilder.Ascending(m => m.DisplayName)),
                new CreateIndexModel<ExtensionMetadata>(
                    extensionMetadataBuilder.Descending(m => m.Statistics.WeightedRating)),
                new CreateIndexModel<ExtensionMetadata>(
                    extensionMetadataBuilder.Descending(m => m.Statistics.TrendingWeekly)),
                new CreateIndexModel<ExtensionMetadata>(
                    extensionMetadataBuilder
                        .Text(m => m.PublisherDisplayName)
                        .Text(m => m.DisplayName)
                        .Text(m => m.Description)
                )
            };
            GalleryMetadata.Indexes.CreateMany(indexModel);
        }
    }
}
