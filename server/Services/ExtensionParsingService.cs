using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CodePaint.WebApi.Domain.Models;
using Serilog;

namespace CodePaint.WebApi.Services
{
    public interface IExtensionParsingService
    {
        Task<VSCodeTheme> ParseExtension(string extensionId, string pathToExtension);
    }

    public class ExtensionParsingService : IExtensionParsingService
    {
        private readonly IJsonFileLoader _jsonFileLoader;
        private readonly IExtensionMetadataParser _extensionMetadataParser;
        private readonly IVSCodeThemeParser _vsCodeThemeParser;

        public ExtensionParsingService(
            IExtensionMetadataParser extensionMetadataParser,
            IJsonFileLoader jsonFileLoader,
            IVSCodeThemeParser vsCodeThemeParser)
        {
            _extensionMetadataParser = extensionMetadataParser;
            _jsonFileLoader = jsonFileLoader;
            _vsCodeThemeParser = vsCodeThemeParser;
        }

        public async Task<VSCodeTheme> ParseExtension(string extensionId, string pathToExtension)
        {
            Log.Information($"Started processing folder: {pathToExtension}.");

            try
            {
                var pathToPackageJson = Path.Combine(pathToExtension, "extension", "package.json");

                var jPackageJson = await _jsonFileLoader.Load(pathToPackageJson);

                var themesMetadata = _extensionMetadataParser.GetThemesMetadata(
                    jPackageJson,
                    pathToExtension
                );
                var themes = await GetColorThemes(themesMetadata);

                var result = new VSCodeTheme
                {
                    Id = extensionId,
                    Version = _extensionMetadataParser.GetExtensionVersion(jPackageJson),
                    Themes = themes
                };

                Log.Information($"Completed processing folder: {pathToExtension}.");

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while handling '{pathToExtension}' folder.");
                throw;
            }
        }

        private async Task<List<Theme>> GetColorThemes(List<ThemeMetadata> themesMetadata)
        {
            var themes = new List<Theme>();
            foreach (var metadata in themesMetadata)
            {
                if (!string.IsNullOrWhiteSpace(metadata.Path))
                {
                    themes.Add(await _vsCodeThemeParser.LoadTheme(metadata));
                }
            }
            return themes;
        }
    }
}
