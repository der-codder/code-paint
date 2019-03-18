using System;
using CodePaint.WebApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace CodePaint.WebApi.Tests
{
    public class ThemeStatisticTests
    {
        private const string ValidJson = @"{
                'statistics': [
                    {
                        'statisticName': 'install',
                        'value': 101
                    },
                    {
                        'statisticName': 'updateCount',
                        'value': 102
                    },
                    {
                        'statisticName': 'averagerating',
                        'value': 4.7746915817260742
                    },
                    {
                        'statisticName': 'weightedRating',
                        'value': 4.7635946102484965
                    },
                    {
                        'statisticName': 'ratingcount',
                        'value': 103
                    },
                    {
                        'statisticName': 'trendingdaily',
                        'value': 0.013415470453884528
                    },
                    {
                        'statisticName': 'trendingweekly',
                        'value': 3.5694317256711274
                    },
                    {
                        'statisticName': 'trendingmonthly',
                        'value': 39.742664612405392
                    }
                ]
            }";

        [Fact]
        public void FromJson_JObjectWithValidBaseProperties_ReturnsCorrectResult()
        {
            var themeId = "themeId_test";
            var result = ThemeStatistic.FromJson(JObject.Parse(ValidJson), themeId);

            Assert.Equal(themeId, result.ThemeId);
            Assert.Equal(101, result.InstallCount);
            Assert.Equal(102, result.UpdateCount);
            Assert.Equal(4.77469158172607, result.AverageRating);
            Assert.Equal(4.7635946102485, result.WeightedRating);
            Assert.Equal(103, result.RatingCount);
            Assert.Equal(0.0134154704538845, result.TrendingDaily);
            Assert.Equal(3.56943172567113, result.TrendingWeekly);
            Assert.Equal(39.7426646124054, result.TrendingMonthly);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void FromJson_ThemeIdIsEmpty_ThrowsArgumentNullException(string themeId)
        {
            Assert.Throws<ArgumentNullException>(() =>
                ThemeStatistic.FromJson(JObject.Parse(ValidJson), themeId));
        }

        [Fact]
        public void FromJson_JObjectWithoutStatistics_ThrowsJsonException()
        {
            var ex = Assert.Throws<JsonException>(() =>
                ThemeStatistic.FromJson(JObject.Parse("{}"), "themeId_test"));

            Assert.Equal("Property 'statistics' does not exist on JObject.", ex.Message);
        }
    }
}
