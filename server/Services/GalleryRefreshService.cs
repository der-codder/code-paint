﻿using System;
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
        private readonly IGalleryItemsRepository _galleryItemsRepository;
        private readonly IGalleryStatisticsRepository _galleryStatisticsRepository;
        private readonly IThemeStoreRefreshService _themeStoreRefreshService;

        public GalleryRefreshService(
            IVSMarketplaceClient marketplaceClient,
            IGalleryItemsRepository galleryInfoRepository,
            IGalleryStatisticsRepository galleryStatisticsRepository,
            IThemeStoreRefreshService themeStoreRefreshService)
        {
            _marketplaceClient = marketplaceClient;
            _galleryItemsRepository = galleryInfoRepository;
            _galleryStatisticsRepository = galleryStatisticsRepository;
            _themeStoreRefreshService = themeStoreRefreshService;
        }

        public async Task RefreshGallery()
        {
            Log.Information("---- Gallery Refreshing Started.");

            var policy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    5,
                    _ => TimeSpan.FromSeconds(5),
                    (_, ts) => Log.Error($"Error connecting to 'https://marketplace.visualstudio.com/'. Retrying in {ts.Seconds} sec.")
                );

            var pageNumber = 3;
            const int pageSize = 10;
            var requestResultTotalCount = (pageNumber * pageSize);

            while (requestResultTotalCount - (pageNumber * pageSize) >= 0)
            {
                var responseMetadata = await policy.ExecuteAsync(
                    async () => await _marketplaceClient.GetGalleryMetadata(pageNumber, pageSize)
                );

                await RefreshGalleryInfo(responseMetadata);

                await _themeStoreRefreshService.RefreshGalleryStore(
                    responseMetadata.Items
                        .Select(i => i.GalleryItem)
                        .ToList()
                );

                Log.Information("---- Processed {UpdatedCount} of {TotalCount} items.",
                    ((pageNumber - 1) * pageSize) + responseMetadata.Items.Count,
                    responseMetadata.RequestResultTotalCount
                );

                pageNumber++;
                requestResultTotalCount = responseMetadata.RequestResultTotalCount;
                break;
            }

            // if (await _galleryItemsRepository.ChangeGalleryItemType("vscode-icons-team.vscode-icons", GalleryItemType.NoThemes))
            // {
            //     Log.Information("Changed GalleryItem 'vscode-icons-team.vscode-icons' type to GalleryItemType.NoThemes.");
            // }

            // var stream = await _marketplaceClient.GetVsixFileStream("zhuangtongfa", "Material-theme", "2.19.3");
            // ProcessVsixFileStream(stream);

            Log.Information("---- Gallery Refreshing Completed.");
        }

        private async Task RefreshGalleryInfo(ExtensionQueryResponseMetadata metadata)
        {
            Log.Information("Gallery Items Refreshing Started.");
            await Task.WhenAll(
                metadata.Items
                    .Select(m => RefreshGalleryItem(m.GalleryItem))
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
