using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using CodePaint.WebApi.Models;
using Microsoft.Extensions.Logging;

namespace CodePaint.WebApi.Services
{
    public class GalleryRefreshService : IGalleryRefreshService
    {
        private readonly IGalleryRepository _repository;
        private readonly IVSMarketplaceClient _marketplaceClient;
        private readonly ILogger<GalleryRefreshService> _logger;

        public GalleryRefreshService(
            IGalleryRepository repository,
            IVSMarketplaceClient marketplaceClient,
            ILogger<GalleryRefreshService> logger)
        {
            _repository = repository;
            _marketplaceClient = marketplaceClient;
            _logger = logger;
        }

        public async Task RefreshGallery()
        {
            _logger.LogInformation("Start Gallery Refreshing.");

            var list = await _marketplaceClient.GetThemesInfoAsync(1, 50);
            foreach (var item in list)
            {
                Console.WriteLine(item.Id);
            }

            // var stream = await _marketplaceClient.GetVsixFileStream("zhuangtongfa", "Material-theme", "2.19.3");
            // ProcessVsixFileStream(stream);

            _logger.LogInformation("Complete Gallery Refreshing.");
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
