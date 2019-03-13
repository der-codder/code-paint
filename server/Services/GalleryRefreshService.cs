using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using CodePaint.WebApi.Models;

namespace CodePaint.WebApi.Services
{
    public class GalleryRefreshService
    {
        private readonly IGalleryRepository _repository;
        private readonly IVSMarketplaceClient _marketplaceClient;

        public GalleryRefreshService(
            IGalleryRepository repository,
            IVSMarketplaceClient marketplaceClient)
        {

            _repository = repository;
            _marketplaceClient = marketplaceClient;
        }

        public async Task RefreshGallery()
        {
            // var list = await _marketplaceClient.GetThemesInfoAsync(1, 50);
            // foreach (var item in list) {
            //     Console.WriteLine(item.Id);
            // }

            Console.WriteLine("---- begin");
            var stream = await _marketplaceClient.GetVsixFileStream("zhuangtongfa", "Material-theme", "2.19.3");
            ProcessVsixFileStream(stream);
            Console.WriteLine("---- end");
        }

        private void ProcessVsixFileStream(Stream stream)
        {
            var tempFolder = Path
                .Combine(Path.GetTempPath(), Convert.ToString(Guid.NewGuid()));
            Directory.CreateDirectory(tempFolder);
            Console.WriteLine($"---- Create temp folder: {tempFolder}");

            try
            {
                using(ZipArchive archive = new ZipArchive(stream))
                {
                    archive.ExtractToDirectory(tempFolder);
                }

                string readText = File.ReadAllText(Path.Combine(tempFolder, "extension", "package") + ".json");
                Console.WriteLine(readText);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Caught exception : " + ex);
                throw;
            }
            finally
            {
                RemoveFolder(tempFolder);
            }
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
            Console.WriteLine($"---- Folder removed: {path}");
        }
    }
}
