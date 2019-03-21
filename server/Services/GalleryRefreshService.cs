using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using CodePaint.WebApi.Domain.Models;
using CodePaint.WebApi.Domain.Repositories;
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
        private readonly IGalleryItemsRepository _galleryItemsRepository;
        private readonly IGalleryStatisticsRepository _galleryStatisticsRepository;
        private readonly IVSMarketplaceClient _marketplaceClient;

        public GalleryRefreshService(
            IVSMarketplaceClient marketplaceClient,
            IGalleryItemsRepository galleryInfoRepository,
            IGalleryStatisticsRepository galleryStatisticsRepository)
        {
            _marketplaceClient = marketplaceClient;
            _galleryItemsRepository = galleryInfoRepository;
            _galleryStatisticsRepository = galleryStatisticsRepository;
        }

        public async Task RefreshGallery()
        {
            Log.Information("Gallery Refreshing Started.");

            var metadata = await _marketplaceClient.GetGalleryMetadata(1, 10);
            Log.Information($"RequestResultTotalCount = {metadata.RequestResultTotalCount}.");

            RefreshGalleryInfo(metadata);

            // if (await _galleryItemsRepository.ChangeGalleryItemType("vscode-icons-team.vscode-icons", GalleryItemType.NoThemes))
            // {
            //     Log.Information("Changed GalleryItem 'vscode-icons-team.vscode-icons' type to GalleryItemType.NoThemes.");
            // }

            // var stream = await _marketplaceClient.GetVsixFileStream("zhuangtongfa", "Material-theme", "2.19.3");
            // ProcessVsixFileStream(stream);

            Log.Information("Gallery Refreshing Completed.");
        }

        // private async Task RefreshGalleryStore(List<GalleryItem> galleryItems)
        // {

        // }

        private void RefreshGalleryInfo(ExtensionQueryResponseMetadata metadata)
        {
            Log.Information("Gallery Items Refreshing Started.");
            Task.WaitAll(
                metadata.Items
                    .Select(m => RefreshGalleryItem(m.GalleryItem))
                    .ToArray()
            );
            Log.Information("Gallery Items Refreshing Completed.");

            Log.Information("Gallery Statistics Refreshing Started.");
            Task.WaitAll(
                metadata.Items
                    .Select(m => UpdateGalleryStatistics(m.Statistic))
                    .ToArray()
            );
            Log.Information("Gallery Statistics Refreshing Completed.");
        }

        private async Task RefreshGalleryItem(GalleryItem freshGalleryItem)
        {
            try
            {
                var galleryItem = await _galleryItemsRepository.GetGalleryItem(freshGalleryItem.Id);

                if (galleryItem == null)
                {
                    await CreateGalleryItem(freshGalleryItem);
                }
                else if (galleryItem.LastUpdated != freshGalleryItem.LastUpdated)
                {
                    await UpdateGalleryItem(freshGalleryItem);
                }
                // else
                // {
                //     Log.Information($"GalleryItem: '{galleryItem.Id}' does not changed.");
                // }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while refreshing gallery item: '{freshGalleryItem.Id}'.");
            }
        }

        private async Task CreateGalleryItem(GalleryItem galleryItem)
        {
            Log.Information($"Create gallery item: '{galleryItem.Id}'.");
            await _galleryItemsRepository.Create(galleryItem);
        }

        private async Task UpdateGalleryItem(GalleryItem theme)
        {
            var result = await _galleryItemsRepository.Update(theme);

            if (result)
            {
                Log.Information($"Successfuly updated '{theme.Id}'.");
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
