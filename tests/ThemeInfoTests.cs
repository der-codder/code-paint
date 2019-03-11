using System;
using System.Globalization;
using CodePaint.WebApi.Models;
using Newtonsoft.Json.Linq;
using Xunit;

namespace CodePaint.WebApi.Tests
{
    public class ThemeInfoTests
    {
        const string validJson = @"{
                'publisher': {
                    'publisherName': 'publisherName_test',
                    'displayName': 'publisherDisplayName_test'
                },
                'extensionName': 'extensionName_test',
                'displayName': 'displayName_test',
                'shortDescription': 'shortDescription_test',
                'versions': [
                    {
                        'version': '1.0.0',
                        'lastUpdated': '2019-01-01T00:00:00.0Z',
                        'files': [
                            {
                                'assetType': 'Microsoft.VisualStudio.Services.Icons.Default',
                                'source': 'iconDefault_test'
                            },
                            {
                                'assetType': 'Microsoft.VisualStudio.Services.Icons.Small',
                                'source': 'iconSmall_test'
                            }
                        ],
                        'assetUri': 'assetUri_test',
                        'fallbackAssetUri': 'fallbackAssetUri_test'
                    }
                ],
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
            var result = ThemeInfo.FromJson(JObject.Parse(validJson));

            Assert.Equal("extensionName_test.publisherName_test", result.Id);
            Assert.Equal("extensionName_test", result.Name);
            Assert.Equal("displayName_test", result.DisplayName);
            Assert.Equal("shortDescription_test", result.Description);
            Assert.Equal("publisherName_test", result.PublisherName);
            Assert.Equal("publisherDisplayName_test", result.PublisherDisplayName);
        }

        [Fact]
        public void FromJson_JObjectWithValidStatistics_ReturnsCorrectResult()
        {
            var result = ThemeInfo.FromJson(JObject.Parse(validJson));

            Assert.Equal(101, result.InstallCount);
            Assert.Equal(102, result.UpdateCount);
            Assert.Equal(4.77469158172607, result.AverageRating);
            Assert.Equal(4.7635946102485, result.WeightedRating);
            Assert.Equal(103, result.RatingCount);
            Assert.Equal(0.0134154704538845, result.TrendingDaily);
            Assert.Equal(3.56943172567113, result.TrendingWeekly);
            Assert.Equal(39.7426646124054, result.TrendingMonthly);
        }

        [Fact]
        public void FromJson_JObjectWithValidVersion_ReturnsCorrectResult()
        {
            var expextedDate = DateTime.Parse("2019-01-01T00:00:00.0Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            var result = ThemeInfo.FromJson(JObject.Parse(validJson));

            Assert.Equal("1.0.0", result.Version);
            Assert.Equal("assetUri_test", result.AssetUri);
            Assert.Equal("fallbackAssetUri_test", result.FallbackAssetUri);
            Assert.Equal(expextedDate, result.LastUpdated);
            Assert.Equal("iconDefault_test", result.IconDefault);
            Assert.Equal("iconSmall_test", result.IconSmall);
        }
    }
}
