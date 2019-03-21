using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using CodePaint.WebApi.Domain.Models;
using Serilog;

namespace CodePaint.WebApi.Services
{
    public class VSExtensionHandler : IDisposable
    {
        private readonly Stream _vsixStream;

        public VSCodeTheme VSCodeTheme { get; private set; }

        public VSExtensionHandler(Stream vsixStream) => _vsixStream = vsixStream;

        public async Task ProcessVSExtension()
        {
            Log.Information("Start Processing Extension.");

            var tempFolder = Path.Combine(
                Path.GetTempPath(),
                Convert.ToString(Guid.NewGuid())
            );

            try
            {
                Directory.CreateDirectory(tempFolder);
                Log.Information($"Created temp folder: '{tempFolder}'.");

                using (var archive = new ZipArchive(_vsixStream))
                {
                    Log.Information("Extracting archive.");
                    await Task.Run(() => archive.ExtractToDirectory(tempFolder));
                }

                // string readText = File.ReadAllText(Path.Combine(tempFolder, "extension", "package.json"));
                // Log.Information("---- extension.package.json: '{}'", readText);
                var parser = new VsixParser(tempFolder);
                VSCodeTheme = await parser.ParseVSCodeTheme();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while processing extension.");
            }
            finally
            {
                await Task.Run(() => RemoveFolder(tempFolder));
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

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _vsixStream.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
