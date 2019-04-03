using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace CodePaint.WebApi.Domain.Models
{
    public class ExtensionsQuery
    {
        private const int DEFAULT_PAGE_NUMBER = 1;
        private const int DEFAULT_PAGE_SIZE = 50;
        private readonly Dictionary<string, SortDefinition<ExtensionMetadata>> _sortings;

        public string SortBy { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }

        public ExtensionsQuery()
        {
            _sortings = new Dictionary<string, SortDefinition<ExtensionMetadata>>
            {
                ["Downloads"] = Builders<ExtensionMetadata>
                    .Sort.Descending(x => x.Statistics.Downloads),
                ["UpdatedDate"] = Builders<ExtensionMetadata>
                    .Sort.Descending(x => x.LastUpdated),
                ["Publisher"] = Builders<ExtensionMetadata>
                    .Sort.Ascending(x => x.PublisherDisplayName),
                ["Name"] = Builders<ExtensionMetadata>
                    .Sort.Ascending(x => x.DisplayName),
                ["Rating"] = Builders<ExtensionMetadata>
                    .Sort.Descending(x => x.Statistics.WeightedRating),
                ["TrendingWeekly"] = Builders<ExtensionMetadata>
                    .Sort.Descending(x => x.Statistics.TrendingWeekly)
            };
        }

        public void NormalizeQueryParams()
        {
            if (!PageNumber.HasValue || PageNumber.Value < DEFAULT_PAGE_NUMBER)
            {
                PageNumber = DEFAULT_PAGE_NUMBER;
            }

            if (!PageSize.HasValue)
            {
                PageSize = DEFAULT_PAGE_SIZE;
            }
        }

        public SortDefinition<ExtensionMetadata> GetSorting()
        {
            if (string.IsNullOrWhiteSpace(SortBy)
                || !_sortings.Keys.Any(key => key == SortBy))
            {
                return _sortings["Downloads"];
            }

            return _sortings[SortBy];
        }
    }
}
