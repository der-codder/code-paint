using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;

using static CodePaint.WebApi.Utils.Extensions;

namespace CodePaint.WebApi.Models
{
    public class ThemeInfo
    {
        [BsonId]
        public ObjectId InternalId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string PublisherName { get; set; }
        public string PublisherDisplayName { get; set; }
        public string Version { get; set; }
        public DateTime LastUpdated { get; set; }
        public string AssetUri { get; set; }
        public string FallbackAssetUri { get; set; }
        public string IconDefault { get; set; }
        public string IconSmall { get; set; }
        public int InstallCount { get; set; }
        public int UpdateCount { get; set; }
        public double AverageRating { get; set; }
        public double WeightedRating { get; set; }
        public int RatingCount { get; set; }
        public double TrendingDaily { get; set; }
        public double TrendingWeekly { get; set; }
        public double TrendingMonthly { get; set; }

        public static ThemeInfo FromJson(JObject jObject)
        {
            var themeInfo = new ThemeInfo();

            themeInfo.Name = jObject.SelectToken("extensionName", true).ToString();
            themeInfo.DisplayName = jObject.SelectToken("displayName", true).ToString();
            themeInfo.Description = jObject.SelectToken("shortDescription", true).ToString();
            themeInfo.PublisherName = jObject.SelectToken("publisher.publisherName", true).ToString();
            themeInfo.PublisherDisplayName = jObject.SelectToken("publisher.displayName", true).ToString();

            themeInfo.Id = $"{themeInfo.Name}.{themeInfo.PublisherName}";

            themeInfo = ProcessVersionData(themeInfo, (JObject)jObject.SelectToken("versions[0]", true));
            themeInfo = ProcessStatistics(themeInfo, (JArray)jObject.SelectToken("statistics", true));

            return themeInfo;
        }

        private static ThemeInfo ProcessVersionData(ThemeInfo themeInfo, JObject version)
        {
            themeInfo.Version = (string) version.SelectToken("version", true);
            themeInfo.AssetUri = (string) version.SelectToken("assetUri", true);
            themeInfo.FallbackAssetUri = (string) version.SelectToken("fallbackAssetUri", true);

            var lastUpdatedStr = (string) version.SelectToken("lastUpdated", true);
            themeInfo.LastUpdated = DateTime.Parse(
                lastUpdatedStr,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal);

            var assets = ToDictionary((JArray)version.SelectToken("files"),"assetType","source");
            themeInfo.IconDefault = assets.GetValueOrDefault("Microsoft.VisualStudio.Services.Icons.Default");
            themeInfo.IconSmall = assets.GetValueOrDefault("Microsoft.VisualStudio.Services.Icons.Small");

            return themeInfo;
        }

        private static ThemeInfo ProcessStatistics(ThemeInfo themeInfo, JArray statistics)
        {
            var statisticDict = ToDictionary(statistics, "statisticName", "value");

            themeInfo.InstallCount = Convert.ToInt32(statisticDict.GetValueOrDefault("install"));
            themeInfo.UpdateCount = Convert.ToInt32(statisticDict.GetValueOrDefault("updateCount"));
            themeInfo.AverageRating = Convert.ToDouble(statisticDict.GetValueOrDefault("averagerating"));
            themeInfo.WeightedRating = Convert.ToDouble(statisticDict.GetValueOrDefault("weightedRating"));
            themeInfo.RatingCount = Convert.ToInt32(statisticDict.GetValueOrDefault("ratingcount"));
            themeInfo.TrendingDaily = Convert.ToDouble(statisticDict.GetValueOrDefault("trendingdaily"));
            themeInfo.TrendingWeekly = Convert.ToDouble(statisticDict.GetValueOrDefault("trendingweekly"));
            themeInfo.TrendingMonthly = Convert.ToDouble(statisticDict.GetValueOrDefault("trendingmonthly"));

            return themeInfo;
        }

        private static IDictionary<string, string> ToDictionary(
            JArray jArray,
            string keyIdentifier,
            string valueIdentifier)
        {
            if (jArray == null)
                return new Dictionary<string, string>();

            var keyValuePairs = jArray
                .Select(
                    item => new KeyValuePair<string, string>(
                        item[keyIdentifier].ToString(),
                        item[valueIdentifier].ToString()
                    )
                )
                .ToList();

            return new Dictionary<string, string>(keyValuePairs);
        }
    }
}
