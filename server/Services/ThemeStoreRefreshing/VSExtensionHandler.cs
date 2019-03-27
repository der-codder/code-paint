using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using CodePaint.WebApi.Domain.Models;
using Serilog;

namespace CodePaint.WebApi.Services.ThemeStoreRefreshing
{
    public interface IVSExtensionHandler
    {
        Task<VSCodeTheme> ProcessExtension(string extensionId, Stream vsixStream);
    }

    public class VSExtensionHandler : IVSExtensionHandler
    {
        private readonly IExtensionParsingService _extensionParsingServise;

        public VSExtensionHandler(IExtensionParsingService extensionParsingServise) =>
            _extensionParsingServise = extensionParsingServise;

        public async Task<VSCodeTheme> ProcessExtension(string extensionId, Stream vsixStream)
        {
            Log.Information("Start Processing Extension.");

            var tempFolder = Path.Combine(
                Path.GetTempPath(),
                Convert.ToString(Guid.NewGuid()),
                extensionId
            );
            Directory.CreateDirectory(tempFolder);

            try
            {
                Log.Information($"Created temp folder: '{tempFolder}'.");

                using (var archive = new ZipArchive(vsixStream))
                {
                    Log.Information("Extracting archive.");
                    await Task.Run(() => archive.ExtractToDirectory(tempFolder));
                }

                return await _extensionParsingServise.ParseExtension(extensionId, tempFolder);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while processing extension.");
                throw;
            }
            finally
            {
                await Task.Run(() => RemoveFolder(tempFolder));
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
            Log.Information("Folder removed: '{Path}'", path);
        }
    }
}
