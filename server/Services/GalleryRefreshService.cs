using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using CodePaint.WebApi.Models;
using Microsoft.Extensions.Logging;

namespace CodePaint.WebApi.Services
{
    public interface IGalleryRefreshService
    {
        Task RefreshGallery();
    }

    public class GalleryRefreshService : IGalleryRefreshService
    {
        private readonly IGalleryInfoRepository _galleryInfoRepository;
        private readonly IGalleryStatisticsRepository _galleryStatisticsRepository;
        private readonly IVSMarketplaceClient _marketplaceClient;
        private readonly ILogger<GalleryRefreshService> _logger;

        public GalleryRefreshService(
            IVSMarketplaceClient marketplaceClient,
            IGalleryInfoRepository galleryInfoRepository,
            IGalleryStatisticsRepository galleryStatisticsRepository,
            ILogger<GalleryRefreshService> logger)
        {
            _marketplaceClient = marketplaceClient;
            _galleryInfoRepository = galleryInfoRepository;
            _galleryStatisticsRepository = galleryStatisticsRepository;
            _logger = logger;
        }

        public async Task RefreshGallery()
        {
            _logger.LogInformation("Start Gallery Refreshing.");

            var metadata = await _marketplaceClient.GetGalleryMetadata(1, 10);
            await UpdateGalleryInfo(metadata);
            await UpdateGalleryStatistics(metadata);

            // var stream = await _marketplaceClient.GetVsixFileStream("zhuangtongfa", "Material-theme", "2.19.3");
            // ProcessVsixFileStream(stream);

            _logger.LogInformation("Complete Gallery Refreshing.");
        }

        private async Task UpdateGalleryInfo(List<GalleryItemMetadata> metadata)
        {
            foreach (var meta in metadata)
            {
                await CreateOrUpdateThemeInfo(meta.ThemeInfo);
            }
        }

        private async Task UpdateGalleryStatistics(List<GalleryItemMetadata> metadata)
        {
            _logger.LogInformation($"---- Before");
            foreach (var meta in metadata)
            {
                var result = await _galleryStatisticsRepository
                    .UpdateThemeStatistics(
                        meta.ThemeStatistic
                    );

                if (result.IsAcknowledged && result.ModifiedCount == 1)
                {
                    _logger.LogInformation($"Statistics of '{meta.ThemeStatistic.ThemeId}' is modified.");
                }
                else if (result.IsAcknowledged && result.UpsertedId != null)
                {
                    _logger.LogInformation($"Statistics of '{meta.ThemeStatistic.ThemeId}' is upserted (Id = '{result.UpsertedId.ToString()}').");
                }
                else
                {
                    _logger.LogInformation($"Statistics of '{meta.ThemeStatistic.ThemeId}' does not modified.");
                }
            }

            _logger.LogInformation($"---- After");
        }

        private async Task CreateOrUpdateThemeInfo(ThemeInfo theme)
        {
            try
            {
                var themeInfo = await _galleryInfoRepository.GetThemeInfo(theme.Id);

                if (themeInfo == null)
                {
                    _logger.LogInformation("Create ThemeInfo: {Id}.", theme.Id);
                    await _galleryInfoRepository.Create(theme);
                }
                else if (themeInfo.LastUpdated != theme.LastUpdated)
                {
                    _logger.LogInformation("Update ThemeInfo: {Id}.", theme.Id);
                    var result = await _galleryInfoRepository.Update(theme);
                    if (result == true)
                    {
                        _logger.LogInformation("Update successful.");
                    }
                    else
                    {
                        _logger.LogWarning("Update of '{Id}' unsuccessful.", theme.Id);
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Caught exception");
            }
        }

        private void ProcessVsixFileStream(Stream stream)
        {
            _logger.LogInformation("Start Processing Extension.");

            var tempFolder = Path.Combine(
                Path.GetTempPath(),
                Convert.ToString(Guid.NewGuid()));
            _logger.LogInformation("Create temp folder: '{TempFolder}'", tempFolder);
            Directory.CreateDirectory(tempFolder);

            try
            {
                using(ZipArchive archive = new ZipArchive(stream))
                {
                    _logger.LogInformation("Extracting archive.");
                    archive.ExtractToDirectory(tempFolder);
                }

                string readText = File.ReadAllText(Path.Combine(tempFolder, "extension", "package.json"));
                _logger.LogInformation("-- extension.package.json: '{}'", readText);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Caught exception");
                throw;
            }
            finally
            {
                _logger.LogInformation("Remove temp folder: '{TempFolder}'", tempFolder);
                RemoveFolder(tempFolder);
            }

            _logger.LogInformation("Complete Processing Extension.");
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
            _logger.LogInformation("Folder removed: '{Path}'", path);
        }
    }
}
