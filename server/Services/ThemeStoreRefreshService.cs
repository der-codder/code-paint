using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodePaint.WebApi.Domain.Models;
using Polly;
using Polly.Retry;
using Serilog;

namespace CodePaint.WebApi.Services
{
    public interface IThemeStoreRefreshService
    {
        Task RefreshGalleryStore(List<GalleryItem> metadata);
        Task RefreshGalleryStoreTheme(GalleryItem freshThemeMetadata);
    }

    public class ThemeStoreRefreshService : IThemeStoreRefreshService
    {
        private readonly IThemeStoreRefresher _refresher;

        private readonly AsyncRetryPolicy _policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                5,
                _ => TimeSpan.FromSeconds(5),
                (_, ts) => Log.Error($"Error while processing fresh theme. Retrying in {ts.Seconds} sec.")
            );

        public ThemeStoreRefreshService(IThemeStoreRefresher refresher) => _refresher = refresher;

        public async Task RefreshGalleryStore(List<GalleryItem> metadata)
        {
            Log.Information("Gallery Store Refreshing Started.");
            const int numberOfItemsPerIteration = 10;
            for (var i = 0; numberOfItemsPerIteration * i <= metadata.Count; i++)
            {
                var itemsToRefresh = metadata
                    .Skip(numberOfItemsPerIteration * i)
                    .Take(numberOfItemsPerIteration);

                await Task.WhenAll(
                    itemsToRefresh
                        .Select(item => RefreshGalleryStoreTheme(item))
                        .ToArray()
                );
            }
            Log.Information("Gallery Store Refreshing Completed.");
        }

        public async Task RefreshGalleryStoreTheme(GalleryItem freshThemeMetadata)
        {
            Log.Information($"Start extension refreshing: Id='{freshThemeMetadata.Id}'.");

            var savedGalleryItemType = await _refresher.GetSavedGalleryItemType(freshThemeMetadata.Id);
            if (savedGalleryItemType != GalleryItemType.Default)
            {
                Log.Information($"Skip refreshing of theme: Id='{freshThemeMetadata.Id}'; Type='{savedGalleryItemType}'.");
                return;
            }

            Log.Information($"Refresh gallery store theme: Id='{freshThemeMetadata.Id}'.");
            var storedTheme = await _refresher.GetStoredTheme(freshThemeMetadata.Id);

            if (storedTheme != null && storedTheme.Version == freshThemeMetadata.Version)
            {
                Log.Information($"Theme '{storedTheme.Id}' does not changed.");
                return;
            }

            var freshTheme = await _policy.ExecuteAsync(
                async () => await _refresher.DownloadFreshTheme(freshThemeMetadata)
            );

            var freshThemeType = await _refresher.CheckAndUpdateFreshThemeType(freshTheme);
            if (freshThemeType == GalleryItemType.Default)
            {
                if (storedTheme == null)
                {
                    await _refresher.CreateTheme(freshTheme);
                }
                else
                {
                    await _refresher.UpdateTheme(freshTheme);
                }
            }
        }
    }
}
