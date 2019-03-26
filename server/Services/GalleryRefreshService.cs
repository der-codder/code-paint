using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using CodePaint.WebApi.Domain.Models;
using CodePaint.WebApi.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Serilog;

namespace CodePaint.WebApi.Services
{
    public interface IGalleryRefreshService
    {
        Task RefreshGallery(int pageNumber);
    }

    public class GalleryRefreshService : IGalleryRefreshService
    {
        private readonly AsyncRetryPolicy _transientExceptionHandlingPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                5,
                _ => TimeSpan.FromSeconds(5),
                (_, ts) => Log.Error($"Error while connecting to 'https://marketplace.visualstudio.com/'. Retrying in {ts.Seconds} sec.")
            );

        private readonly IVSMarketplaceClient _marketplaceClient;
        private readonly IGalleryMetadataRepository _galleryMetadataRepository;
        private readonly IGalleryStatisticsRepository _galleryStatisticsRepository;
        private readonly IThemeStoreRefreshService _themeStoreRefreshService;

        public GalleryRefreshService(
            IVSMarketplaceClient marketplaceClient,
            IGalleryMetadataRepository galleryMetadataRepository,
            IGalleryStatisticsRepository galleryStatisticsRepository,
            IThemeStoreRefreshService themeStoreRefreshService)
        {
            _marketplaceClient = marketplaceClient;
            _galleryMetadataRepository = galleryMetadataRepository;
            _galleryStatisticsRepository = galleryStatisticsRepository;
            _themeStoreRefreshService = themeStoreRefreshService;
        }

        public async Task RefreshGallery(int pageNumber)
        {
            Log.Information("---- Gallery Refreshing Started.");

            // var pageNumber = 12;
            const int pageSize = 20;
            var requestResultTotalCount = (pageNumber * pageSize);

            while (requestResultTotalCount - (pageNumber * pageSize) >= 0)
            {
                var responseMetadata = await _transientExceptionHandlingPolicy.ExecuteAsync(
                    async () => await _marketplaceClient.GetGalleryMetadata(pageNumber, pageSize)
                );

                await RefreshGalleryInfo(responseMetadata);

                await _themeStoreRefreshService.RefreshGalleryStore(
                    responseMetadata.Items
                        .Select(i => i.Metadata)
                        .ToList()
                );

                Log.Information("------------------ Processed {UpdatedCount} of {TotalCount} items.",
                    ((pageNumber - 1) * pageSize) + responseMetadata.Items.Count,
                    responseMetadata.RequestResultTotalCount
                );

                pageNumber++;
                requestResultTotalCount = responseMetadata.RequestResultTotalCount;
                break;
            }

            Log.Information("---- Gallery Refreshing Completed.");
        }

        private async Task RefreshGalleryInfo(ExtensionQueryResponseMetadata metadata)
        {
            Log.Information("Gallery Items Refreshing Started.");
            await Task.WhenAll(
                metadata.Items
                    .Select(m => RefreshExtensionMetadata(m.Metadata))
                    .ToArray()
            );
            Log.Information("Gallery Items Refreshing Completed.");

            Log.Information("Gallery Statistics Refreshing Started.");
            await Task.WhenAll(
                metadata.Items
                    .Select(m => UpdateGalleryStatistics(m.Statistic))
                    .ToArray()
            );
            Log.Information("Gallery Statistics Refreshing Completed.");
        }

        private async Task RefreshExtensionMetadata(ExtensionMetadata freshExtensionMetadata)
        {
            try
            {
                var extensionMetadata = await _galleryMetadataRepository
                    .GetExtensionMetadata(freshExtensionMetadata.Id);

                if (extensionMetadata == null)
                {
                    await CreateExtensionMetadata(freshExtensionMetadata);
                }
                else if (extensionMetadata.LastUpdated != freshExtensionMetadata.LastUpdated)
                {
                    await UpdateExtensionMetadata(freshExtensionMetadata);
                }
                // else
                // {
                //     Log.Information($"Extension: '{extensionMetadata.Id}' does not changed.");
                // }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while refreshing gallery item: '{freshExtensionMetadata.Id}'.");
            }
        }

        private async Task CreateExtensionMetadata(ExtensionMetadata extensionMetadata)
        {
            Log.Information($"Create gallery item: '{extensionMetadata.Id}'.");
            await _galleryMetadataRepository.Create(extensionMetadata);
        }

        private async Task UpdateExtensionMetadata(ExtensionMetadata theme)
        {
            var result = await _galleryMetadataRepository.Update(theme);

            if (result)
            {
                Log.Information($"Successfully updated '{theme.Id}'.");
            }
            else
            {
                Log.Warning($"Update unsuccessful '{theme.Id}'.");
            }
        }

        private async Task UpdateGalleryStatistics(GalleryItemStatistic freshStatistic)
        {
            try
            {
                var result = await _galleryStatisticsRepository
                    .UpdateThemeStatistics(freshStatistic);

                if (result.IsAcknowledged && result.ModifiedCount == 1)
                {
                    Log.Information($"Modified statistics for '{freshStatistic.Id}'.");
                }
                else if (result.IsAcknowledged && result.UpsertedId != null)
                {
                    Log.Information($"Upserted (Id='{result.UpsertedId}') statistics for '{freshStatistic.Id}'.");
                }
                // else
                // {
                //     Log.Information($"Statistics for '{freshStatistic.Id}' does not changed.");
                // }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while refreshing gallery item statistic for '{freshStatistic.Id}'.");
            }
        }
    }
}
