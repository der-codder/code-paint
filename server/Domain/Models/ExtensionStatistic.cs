using System;
using System.Globalization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json.Linq;

using static CodePaint.WebApi.Utils.Extensions;

namespace CodePaint.WebApi.Domain.Models
{
    public class ExtensionStatistic
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }
        public int InstallCount { get; set; }
        public int UpdateCount { get; set; }
        public double AverageRating { get; set; }
        public double WeightedRating { get; set; }
        public int RatingCount { get; set; }
        public double TrendingDaily { get; set; }
        public double TrendingWeekly { get; set; }
        public double TrendingMonthly { get; set; }

        public static ExtensionStatistic FromJson(JObject jObject)
        {
            var publisherName = jObject.SelectToken("publisher.publisherName", true).ToString();
            var name = jObject.SelectToken("extensionName", true).ToString();

            var statistics = new ExtensionStatistic { Id = $"{publisherName}.{name}" };

            var statisticDict = ((JArray) jObject.SelectToken("statistics", true))
                .ToDictionary<string, string>("statisticName", "value");

            statistics.InstallCount = Convert.ToInt32(
                statisticDict.GetValueOrDefault("install"),
                CultureInfo.InvariantCulture);
            statistics.UpdateCount = Convert.ToInt32(
                statisticDict.GetValueOrDefault("updateCount"),
                CultureInfo.InvariantCulture);
            statistics.AverageRating = Convert.ToDouble(
                statisticDict.GetValueOrDefault("averagerating"),
                CultureInfo.InvariantCulture);
            statistics.WeightedRating = Convert.ToDouble(
                statisticDict.GetValueOrDefault("weightedRating"),
                CultureInfo.InvariantCulture);
            statistics.RatingCount = Convert.ToInt32(
                statisticDict.GetValueOrDefault("ratingcount"),
                CultureInfo.InvariantCulture);
            statistics.TrendingDaily = Convert.ToDouble(
                statisticDict.GetValueOrDefault("trendingdaily"),
                CultureInfo.InvariantCulture);
            statistics.TrendingWeekly = Convert.ToDouble(
                statisticDict.GetValueOrDefault("trendingweekly"),
                CultureInfo.InvariantCulture);
            statistics.TrendingMonthly = Convert.ToDouble(
                statisticDict.GetValueOrDefault("trendingmonthly"),
                CultureInfo.InvariantCulture);

            return statistics;
        }
    }
}
