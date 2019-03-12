using System;
using MongoDB.Driver;

namespace CodePaint.WebApi.Models {

    public class GalleryContext : IGalleryContext {
        private readonly IMongoDatabase _db;

        public IMongoCollection<ThemeInfo> ThemesInfo => _db.GetCollection<ThemeInfo>("ThemesInfo");

        public GalleryContext(MongoDBConfig config) {
            var client = new MongoClient(config.ConnectionString);
            _db = client.GetDatabase(config.Database);
        }
    }
}
