using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using CodePaint.WebApi.Domain.Models;
using CodePaint.WebApi.Domain.Repositories;
using CodePaint.WebApi.Services.ThemeStoreRefreshing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CodePaint.WebApi.Services
{
    public interface IGalleryRefreshService
    {
        Task RefreshGallery();
    }

    public class GalleryRefreshService : IGalleryRefreshService
    {
        private readonly IVSMarketplaceClient _marketplaceClient;
        private readonly IGalleryMetadataRepository _galleryMetadataRepository;
        private readonly IThemeStoreRefreshService _themeStoreRefreshService;

        public GalleryRefreshService(
            IVSMarketplaceClient marketplaceClient,
            IGalleryMetadataRepository galleryMetadataRepository,
            IThemeStoreRefreshService themeStoreRefreshService)
        {
            _marketplaceClient = marketplaceClient;
            _galleryMetadataRepository = galleryMetadataRepository;
            _themeStoreRefreshService = themeStoreRefreshService;
        }

        public async Task RefreshGallery()
        {
            Log.Information("---- Gallery Refreshing Started.");

            var pageNumber = 1;
            const int pageSize = 20;
            var requestResultTotalCount = (pageNumber * pageSize);
            var updatedCount = 0;

            while ((pageNumber * pageSize) - requestResultTotalCount < pageSize)
            {
                var responseMetadata = await _marketplaceClient.GetGalleryMetadata(pageNumber, pageSize);
                updatedCount += responseMetadata.Items.Count;

                await RefreshGalleryInfo(responseMetadata);

                await _themeStoreRefreshService.RefreshGalleryStore(responseMetadata.Items);

                Log.Information("------------------ Processed {UpdatedCount} of {TotalCount} items.",
                    updatedCount,
                    responseMetadata.RequestResultTotalCount
                );

                pageNumber++;
                requestResultTotalCount = responseMetadata.RequestResultTotalCount;
                // break;
            }

            Log.Information("---- Gallery Refreshing Completed.");
        }

        private async Task RefreshGalleryInfo(ExtensionQueryResponseMetadata metadata)
        {
            await Task.WhenAll(
                metadata.Items
                    .Select(m => RefreshExtensionMetadata(m))
                    .ToArray()
            );
        }

        private async Task RefreshExtensionMetadata(ExtensionMetadata freshMetadata)
        {
            try
            {
                var extensionMetadata = await _galleryMetadataRepository
                    .GetExtensionMetadata(freshMetadata.Id);

                if (extensionMetadata == null)
                {
                    await CreateExtensionMetadata(freshMetadata);
                }
                else if (extensionMetadata.LastUpdated != freshMetadata.LastUpdated)
                {
                    await UpdateExtensionMetadata(freshMetadata);
                }
                else
                {
                    await UpdateGalleryStatistics(freshMetadata.Id, freshMetadata.Statistics);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while refreshing extension metadata: '{freshMetadata.Id}'.");
            }
        }

        private async Task CreateExtensionMetadata(ExtensionMetadata extensionMetadata)
        {
            Log.Information($"Create extension metadata: '{extensionMetadata.Id}'.");
            await _galleryMetadataRepository.Create(extensionMetadata);
        }

        private async Task UpdateExtensionMetadata(ExtensionMetadata extensionInfo)
        {
            var result = await _galleryMetadataRepository.Update(extensionInfo);

            if (result)
            {
                Log.Information($"Successfully updated '{extensionInfo.Id}'.");
            }
            else
            {
                Log.Warning($"Update unsuccessful '{extensionInfo.Id}'.");
            }
        }

        private async Task UpdateGalleryStatistics(string extensionId, Statistics freshStatistics)
        {
            try
            {
                var result = await _galleryMetadataRepository
                    .UpdateStatistics(extensionId, freshStatistics);

                if (result)
                {
                    Log.Information($"Successfully updated statistics '{extensionId}'.");
                }
                else
                {
                    Log.Warning($"Statistics update unsuccessful '{extensionId}'.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while refreshing statistic for '{extensionId}'.");
            }
        }
    }
}
