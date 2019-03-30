using System;
using System.Globalization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json.Linq;

using static CodePaint.WebApi.Utils.Extensions;

namespace CodePaint.WebApi.Domain.Models
{
    public class Statistics
    {
        public int InstallCount { get; set; }
        public int UpdateCount { get; set; }
        public double AverageRating { get; set; }
        public double WeightedRating { get; set; }
        public int RatingCount { get; set; }
        public double TrendingDaily { get; set; }
        public double TrendingWeekly { get; set; }
        public double TrendingMonthly { get; set; }
    }
}
