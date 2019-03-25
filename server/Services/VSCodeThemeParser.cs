using System;
using System.IO;
using System.Threading.Tasks;
using CodePaint.WebApi.Domain.Models;
using Newtonsoft.Json.Linq;
using Serilog;

namespace CodePaint.WebApi.Services
{
    public interface IVSCodeThemeParser
    {
        Task<Theme> LoadTheme(ThemeMetadata metadata);
    }

    public class VSCodeThemeParser : IVSCodeThemeParser
    {
        private readonly IJsonFileLoader _jsonFileLoader;

        public VSCodeThemeParser(IJsonFileLoader jsonFileLoader) =>
            _jsonFileLoader = jsonFileLoader;

        public async Task<Theme> LoadTheme(ThemeMetadata metadata)
        {
            var theme = new Theme
            {
                Label = metadata.Label,
                ThemeType = metadata.ThemeType
            };

            try
            {
                if (Path.GetExtension(metadata.Path) == ".json")
                {
                    var jTheme = await LoadJsonThemesWithIncludes(metadata.Path);

                    theme.Colors = Theme.ParseColors(jTheme);
                    theme.TokenColors = Theme.ParseTokenColors(jTheme);
                }
                else
                {
                    Log.Warning($"It is not a json file: '{metadata.Path}'.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while processing VSCode extension theme: Label='{metadata.Label}'; Path='{metadata.Path}'.");
            }

            return theme;
        }

        private async Task<JObject> LoadJsonThemesWithIncludes(string pathToJson)
        {
            var jTheme = await _jsonFileLoader.Load(pathToJson);

            return await MergeExternalThemeFile(jTheme, pathToJson);
        }

        private async Task<JObject> MergeExternalThemeFile(JObject jTheme, string pathToJson)
        {
            Log.Information("Started merging external theme file.");
            var fileFolder = Path.GetDirectoryName(pathToJson);
            var include = (string) jTheme.SelectToken("include");
            if (include != null)
            {
                var pathToIncludedFile = Path
                    .GetFullPath(
                        Path.Combine(fileFolder, include)
                    );
                // Log.Information($"------ rootThemeFileFolder: '{fileFolder}'; include: '{include}'; pathToIncludedFile: '{pathToIncludedFile}'");

                Log.Information($"Merging external theme file: '{pathToIncludedFile}'.");
                var jThemeToInclude = await _jsonFileLoader.Load(pathToIncludedFile);

                jThemeToInclude = await MergeExternalThemeFile(jThemeToInclude, pathToIncludedFile);

                jTheme.Merge(jThemeToInclude);

                Log.Information($"Merging with '{pathToIncludedFile}' completed.");
                return jTheme;
            }
            Log.Information("Finished merging external theme file.");
            return jTheme;
        }
    }
}
