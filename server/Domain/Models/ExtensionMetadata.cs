using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json.Linq;

using static CodePaint.WebApi.Utils.Extensions;

namespace CodePaint.WebApi.Domain.Models
{
    public class ExtensionMetadata
    {
        // TODO: add ExtensionId to identify it?
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

        public string AssetUri { get; set; }

        public ExtensionType Type { get; set; }

        public Statistics Statistics { get; set; }

        public ExtensionMetadata() => Type = ExtensionType.Default;

        public static ExtensionMetadata FromJson(JObject jObject) =>
            Create()
                .TakeBaseData(jObject)
                .TakeVersionData(jObject)
                .TakeStatistics(jObject);

        private static ExtensionMetadataParser Create() => new ExtensionMetadataParser(new ExtensionMetadata());

        private class ExtensionMetadataParser
        {
            private readonly ExtensionMetadata _extensionMetadata;

            public ExtensionMetadataParser(ExtensionMetadata extensionMetadata) => _extensionMetadata = extensionMetadata;

            public ExtensionMetadataParser TakeBaseData(JObject jObject)
            {
                _extensionMetadata.Name = jObject.SelectToken("extensionName", true).ToString();
                _extensionMetadata.PublisherName = jObject.SelectToken("publisher.publisherName", true).ToString();
                _extensionMetadata.Id = $"{_extensionMetadata.PublisherName}.{_extensionMetadata.Name}";

                _extensionMetadata.DisplayName = jObject.SelectToken("displayName", true).ToString();
                _extensionMetadata.PublisherDisplayName = jObject.SelectToken("publisher.displayName", true).ToString();
                _extensionMetadata.Description = (string) jObject.SelectToken("shortDescription");

                return this;
            }

            public ExtensionMetadataParser TakeStatistics(JObject jObject)
            {
                var statisticDict = ((JArray) jObject.SelectToken("statistics", true))
                    .ToDictionary<string, string>("statisticName", "value");

                _extensionMetadata.Statistics = new Statistics
                {
                    InstallCount = Convert.ToInt32(
                        statisticDict.GetValueOrDefault("install"),
                        CultureInfo.InvariantCulture
                    ),
                    UpdateCount = Convert.ToInt32(
                        statisticDict.GetValueOrDefault("updateCount"),
                        CultureInfo.InvariantCulture
                    ),
                    AverageRating = Convert.ToDouble(
                        statisticDict.GetValueOrDefault("averagerating"),
                        CultureInfo.InvariantCulture
                    ),
                    WeightedRating = Convert.ToDouble(
                        statisticDict.GetValueOrDefault("weightedRating"),
                        CultureInfo.InvariantCulture
                    ),
                    RatingCount = Convert.ToInt32(
                        statisticDict.GetValueOrDefault("ratingcount"),
                        CultureInfo.InvariantCulture
                    ),
                    TrendingDaily = Convert.ToDouble(
                        statisticDict.GetValueOrDefault("trendingdaily"),
                        CultureInfo.InvariantCulture
                    ),
                    TrendingWeekly = Convert.ToDouble(
                        statisticDict.GetValueOrDefault("trendingweekly"),
                        CultureInfo.InvariantCulture
                    ),
                    TrendingMonthly = Convert.ToDouble(
                        statisticDict.GetValueOrDefault("trendingmonthly"),
                        CultureInfo.InvariantCulture
                    )
                };

                return this;
            }

            public ExtensionMetadataParser TakeVersionData(JObject jObject)
            {
                var jVersion = (JObject) jObject.SelectToken("versions[0]", true);
                _extensionMetadata.Version = jVersion.SelectToken("version", true).ToString();
                _extensionMetadata.AssetUri = jVersion.SelectToken("fallbackAssetUri", true).ToString()
                    + "/Microsoft.VisualStudio.Services.VSIXPackage";

                var lastUpdatedStr = (string) jVersion.SelectToken("lastUpdated", true);
                _extensionMetadata.LastUpdated = DateTime.Parse(
                    lastUpdatedStr,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal);

                var assets = ((JArray) jVersion.SelectToken("files"))
                    .ToDictionary<string, string>("assetType", "source");

                _extensionMetadata.IconDefault = assets
                    .GetValueOrDefault("Microsoft.VisualStudio.Services.Icons.Default");
                _extensionMetadata.IconSmall = assets
                    .GetValueOrDefault("Microsoft.VisualStudio.Services.Icons.Small");

                return this;
            }

            public static implicit operator ExtensionMetadata(ExtensionMetadataParser extensionMetadata) =>
                extensionMetadata._extensionMetadata;
        }
    }
}
