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
        private readonly IGalleryItemsRepository _galleryInfoRepository;
        private readonly IGalleryStatisticsRepository _galleryStatisticsRepository;
        private readonly IVSMarketplaceClient _marketplaceClient;

        public GalleryRefreshService(
            IVSMarketplaceClient marketplaceClient,
            IGalleryItemsRepository galleryInfoRepository,
            IGalleryStatisticsRepository galleryStatisticsRepository)
        {
            _marketplaceClient = marketplaceClient;
            _galleryInfoRepository = galleryInfoRepository;
            _galleryStatisticsRepository = galleryStatisticsRepository;
        }

        public async Task RefreshGallery()
        {
            Log.Information("Gallery Refreshing Started.");

            var metadata = await _marketplaceClient.GetGalleryMetadata(1, 10);
            await RefreshGalleryInfo(metadata);
            await UpdateGalleryStatistics(metadata);

            // var stream = await _marketplaceClient.GetVsixFileStream("zhuangtongfa", "Material-theme", "2.19.3");
            // ProcessVsixFileStream(stream);

            Log.Information("Gallery Refreshing Completed.");
        }

        private async Task RefreshGalleryInfo(List<GalleryItemMetadata> metadata)
        {
            foreach (var meta in metadata)
            {
                await CreateOrUpdateThemeInfo(meta.ThemeInfo);
            }
        }

        private async Task UpdateGalleryStatistics(List<GalleryItemMetadata> metadata)
        {
            foreach (var meta in metadata)
            {
                var result = await _galleryStatisticsRepository
                    .UpdateThemeStatistics(
                        meta.ThemeStatistic
                    );

                if (result.IsAcknowledged && result.ModifiedCount == 1)
                {
                    Log.Information($"Modified statistics for '{meta.ThemeStatistic.GalleryItemId}'.");
                }
                else if (result.IsAcknowledged && result.UpsertedId != null)
                {
                    Log.Information($"Upserted (Id='{result.UpsertedId}') statistics for '{meta.ThemeStatistic.GalleryItemId}'.");
                }
                // else
                // {
                //     Log.Information($"Statistics for '{meta.GalleryItemStatistic.GalleryItemId}' does not changed.");
                // }
            }
        }

        private async Task CreateOrUpdateThemeInfo(GalleryItem theme)
        {
            try
            {
                var themeInfo = await _galleryInfoRepository.GetGalleryItem(theme.Id);

                if (themeInfo == null)
                {
                    Log.Information($"Create GalleryItem: '{theme.Id}'.");
                    await _galleryInfoRepository.Create(theme);
                }
                else if (themeInfo.LastUpdated != theme.LastUpdated)
                {
                    var result = await _galleryInfoRepository.Update(theme);

                    if (result)
                    {
                        Log.Information($"Successfuly updated '{theme.Id}'.");
                    }
                    else
                    {
                        Log.Warning($"Unsuccessfuly updated '{theme.Id}'.");
                    }
                }
                // else
                // {
                //     Log.Information($"GalleryItem: '{theme.Id}' does not changed.");
                // }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Caught exception");
            }
        }

        private void ProcessVsixFileStream(Stream stream)
        {
            Log.Information("Start Processing Extension.");

            var tempFolder = Path.Combine(
                Path.GetTempPath(),
                Convert.ToString(Guid.NewGuid()));
            Log.Information("Create temp folder: '{TempFolder}'", tempFolder);
            Directory.CreateDirectory(tempFolder);

            try
            {
                using (ZipArchive archive = new ZipArchive(stream))
                {
                    Log.Information("Extracting archive.");
                    archive.ExtractToDirectory(tempFolder);
                }

                string readText = File.ReadAllText(Path.Combine(tempFolder, "extension", "package.json"));
                Log.Information("---- extension.package.json: '{}'", readText);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Caught exception");
                throw;
            }
            finally
            {
                Log.Information("Remove temp folder: '{TempFolder}'", tempFolder);
                RemoveFolder(tempFolder);
            }

            Log.Information("Complete Processing Extension.");
        }

        private void RemoveFolder(string path)
        {
            var di = new DirectoryInfo(path);

            foreach (FileInfo file in di.EnumerateFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.EnumerateDirectories())
            {
                dir.Delete(true);
            }
            Log.Information("Folder removed: '{Path}'", path);
        }
    }
}
