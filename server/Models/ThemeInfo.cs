using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json.Linq;

using static CodePaint.WebApi.Utils.Extensions;

namespace CodePaint.WebApi.Models
{
    public class ThemeInfo
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string PublisherName { get; set; }
        public string PublisherDisplayName { get; set; }
        public string Version { get; set; }
        public DateTime LastUpdated { get; set; }
        public string IconDefault { get; set; }
        public string IconSmall { get; set; }

        public static ThemeInfo FromJson(JObject jObject) => Create()
            .TakeBaseData(jObject)
            .TakeVersionData(jObject);

        private static ThemeInfoParser Create() => new ThemeInfoParser(new ThemeInfo());

        private class ThemeInfoParser
        {
            private readonly ThemeInfo _themeInfo;

            public ThemeInfoParser(ThemeInfo themeInfo) => _themeInfo = themeInfo;

            public ThemeInfoParser TakeBaseData(JObject jObject)
            {
                _themeInfo.Name = jObject.SelectToken("extensionName", true).ToString();
                _themeInfo.DisplayName = jObject.SelectToken("displayName", true).ToString();
                _themeInfo.Description = jObject.SelectToken("shortDescription", true).ToString();
                _themeInfo.PublisherName = jObject.SelectToken("publisher.publisherName", true).ToString();
                _themeInfo.PublisherDisplayName = jObject.SelectToken("publisher.displayName", true).ToString();
                _themeInfo.Id = $"{_themeInfo.PublisherName}.{_themeInfo.Name}";

                return this;
            }

            public ThemeInfoParser TakeVersionData(JObject jObject)
            {
                var jVersion = (JObject) jObject.SelectToken("versions[0]", true);
                _themeInfo.Version = jVersion.SelectToken("version", true).ToString();

                var lastUpdatedStr = (string) jVersion.SelectToken("lastUpdated", true);
                _themeInfo.LastUpdated = DateTime.Parse(
                    lastUpdatedStr,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal);

                var assets = ((JArray) jVersion.SelectToken("files"))
                    .ToDictionary<string, string>("assetType", "source");

                _themeInfo.IconDefault = assets.GetValueOrDefault("Microsoft.VisualStudio.Services.Icons.Default");
                _themeInfo.IconSmall = assets.GetValueOrDefault("Microsoft.VisualStudio.Services.Icons.Small");

                return this;
            }

            public static implicit operator ThemeInfo(ThemeInfoParser themeInfo) => themeInfo._themeInfo;
        }
    }
}
