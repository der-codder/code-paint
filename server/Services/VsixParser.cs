using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CodePaint.WebApi.Domain.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace CodePaint.WebApi.Services
{
    public class VsixParser
    {
        private class ThemeMetadata
        {
            public string Label { get; set; }
            public string ThemeType { get; set; }
            public string Path { get; set; }
        }

        private readonly string _pathToExtension;

        public VsixParser(string pathToExtension) => _pathToExtension = pathToExtension;

        public async Task<VSCodeTheme> ParseVSCodeTheme()
        {
            Log.Information($"Started processing folder: {_pathToExtension}.");

            try
            {
                var pathToPackageJson = Path.Combine(_pathToExtension, "extension", "package.json");

                var jPackageJson = await LoadJsonFile(pathToPackageJson);

                var extensionMetadata = ParseExtensionMetadata(jPackageJson);

                var themes = await ParseColorThemes(ParseThemesMetadata(jPackageJson));

                var result = new VSCodeTheme
                {
                    Id = extensionMetadata.Id,
                    Version = extensionMetadata.Version,
                    Themes = themes
                };

                Log.Information($"Completed processing folder: {_pathToExtension}.");

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while handling '{_pathToExtension}' folder.");
                throw;
            }
        }

        private VSCodeTheme ParseColorThemeMetadata(JObject jObject)
        {
            var colorTheme = new VSCodeTheme();

            var extPublisher = jObject.SelectToken("publisher", true).ToString();
            var extName = jObject.SelectToken("name", true).ToString();
            colorTheme.Id = $"{extPublisher}.{extName}";
            colorTheme.Version = jObject.SelectToken("version", true).ToString();

            return colorTheme;
        }

        private (string Id, string Version) ParseExtensionMetadata(JObject jObject)
        {
            var extPublisher = jObject.SelectToken("publisher", true).ToString();
            var extName = jObject.SelectToken("name", true).ToString();
            var extVersion = jObject.SelectToken("version", true).ToString();

            return ($"{extPublisher}.{extName}", extVersion);
        }

        private List<ThemeMetadata> ParseThemesMetadata(JObject jObject)
        {
            var metadata = new List<ThemeMetadata>();
            var jThemes = (JArray) jObject.SelectToken("contributes.themes");

            if (jThemes == null)
            {
                return metadata;
            }

            foreach (var jTheme in jThemes)
            {
                var themeMetadata = new ThemeMetadata
                {
                    Label = jTheme.SelectToken("label", true).ToString(),
                    ThemeType = jTheme.SelectToken("uiTheme", true).ToString(),
                    Path = Path
                        .GetFullPath(
                            Path.Combine(
                                _pathToExtension,
                                "extension",
                                jTheme.SelectToken("path", true).ToString()
                            )
                        )
                };

                metadata.Add(themeMetadata);
            }

            return metadata;
        }

        private async Task<List<Theme>> ParseColorThemes(List<ThemeMetadata> themesMetadata)
        {
            var themes = new List<Theme>();
            foreach (var metadata in themesMetadata)
            {
                themes.Add(await LoadTheme(metadata));
            }
            return themes;
        }

        private async Task<Theme> LoadTheme(ThemeMetadata metadata)
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
            var jTheme = await LoadJsonFile(pathToJson);
            var rootThemeFileFolder = Path.GetDirectoryName(pathToJson);

            return await MergeExternalThemeFile(jTheme, rootThemeFileFolder);
        }

        private async Task<JObject> MergeExternalThemeFile(JObject jTheme, string rootThemeFileFolder)
        {
            Log.Information("Started merging external theme file.");
            var include = (string) jTheme.SelectToken("include");
            if (include != null)
            {
                var includeFile = Path
                    .GetFullPath(
                        Path.Combine(rootThemeFileFolder, include)
                    );
                Log.Information($"------ rootThemeFileFolder: '{rootThemeFileFolder}'; include: '{include}'; includeFile: '{includeFile}'");

                Log.Information($"Merging external theme file: '{includeFile}'.");
                var jThemeToInclude = await LoadJsonFile(includeFile);

                jThemeToInclude = await MergeExternalThemeFile(jThemeToInclude, rootThemeFileFolder);

                jTheme.Merge(jThemeToInclude);

                Log.Information($"Merging with '{includeFile}' completed.");
                return jTheme;
            }
            Log.Information("Finished merging external theme file.");
            return jTheme;
        }

        private async Task<JObject> LoadJsonFile(string path)
        {
            Console.WriteLine($"---- Loading json file: '{path}'.");

            using (var sr = new StreamReader(File.OpenRead(path)))
            using (var reader = new JsonTextReader(sr))
            {
                var result = await JObject.LoadAsync(reader);

                Console.WriteLine($"---- Loading completed.");
                return result;
            }
        }
    }
}
