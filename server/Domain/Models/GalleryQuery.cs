using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

namespace CodePaint.WebApi.Domain.Models
{
    public class GalleryQuery
    {
        private const int DEFAULT_PAGE_NUMBER = 1;
        private const int DEFAULT_PAGE_SIZE = 50;
        private const string SORT_BY_DOWNLOADS = "Downloads";
        private const string SORT_BY_UPDATED_DATE = "UpdatedDate";
        private const string SORT_BY_PUBLISHER = "Publisher";
        private const string SORT_BY_NAME = "Name";
        private const string SORT_BY_RATING = "Rating";
        private const string SORT_BY_TRENDING_WEEKLY = "TrendingWeekly";
        private const string SORT_BY_RELEVANCE = "Relevance";
        private readonly Dictionary<string, SortDefinition<ExtensionMetadata>> _sortings;

        public string SearchTerm { get; set; }
        public string SortBy { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }

        public FilterDefinition<ExtensionMetadata> Filter
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SearchTerm))
                {
                    return Builders<ExtensionMetadata>.Filter
                            .Eq(extension => extension.Type, ExtensionType.Default);
                }

                return Builders<ExtensionMetadata>.Filter.And(
                    Builders<ExtensionMetadata>.Filter.Eq(
                        extension => extension.Type, ExtensionType.Default),
                    Builders<ExtensionMetadata>.Filter.Text(SearchTerm)
                );
            }
        }

        public SortDefinition<ExtensionMetadata> Sorting
        {
            get
            {
                // ?
                if (string.IsNullOrWhiteSpace(SortBy) && string.IsNullOrWhiteSpace(SearchTerm))
                {
                    return _sortings[SORT_BY_DOWNLOADS];
                }
                // ?sortBy=NotValid&searchTerm=
                if (string.IsNullOrWhiteSpace(SearchTerm) && !_sortings.Keys.Any(key => key == SortBy))
                {
                    return _sortings[SORT_BY_DOWNLOADS];
                }
                // ?searchTerm=TextForSearching&sortBy=
                if (!string.IsNullOrWhiteSpace(SearchTerm) && string.IsNullOrWhiteSpace(SortBy))
                {
                    return _sortings[SORT_BY_RELEVANCE];
                }

                return _sortings[SortBy];
            }
        }

        public GalleryQuery()
        {
            _sortings = new Dictionary<string, SortDefinition<ExtensionMetadata>>
            {
                [SORT_BY_DOWNLOADS] = Builders<ExtensionMetadata>
                    .Sort.Descending(x => x.Statistics.Downloads),
                [SORT_BY_UPDATED_DATE] = Builders<ExtensionMetadata>
                    .Sort.Descending(x => x.LastUpdated),
                [SORT_BY_PUBLISHER] = Builders<ExtensionMetadata>
                    .Sort.Ascending(x => x.PublisherDisplayName),
                [SORT_BY_NAME] = Builders<ExtensionMetadata>
                    .Sort.Ascending(x => x.DisplayName),
                [SORT_BY_RATING] = Builders<ExtensionMetadata>
                    .Sort.Descending(x => x.Statistics.WeightedRating),
                [SORT_BY_TRENDING_WEEKLY] = Builders<ExtensionMetadata>
                    .Sort.Descending(x => x.Statistics.TrendingWeekly),
                [SORT_BY_RELEVANCE] = null
            };
        }

        public void NormalizeQueryParams()
        {
            if (!PageNumber.HasValue || PageNumber.Value < DEFAULT_PAGE_NUMBER)
            {
                PageNumber = DEFAULT_PAGE_NUMBER;
            }

            if (!PageSize.HasValue || PageSize.Value < 1)
            {
                PageSize = DEFAULT_PAGE_SIZE;
            }
        }
    }
}
