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

                var colorTheme = ParseColorThemeMetadata(jPackageJson);

                var result = await ParseThemesAndUpdateColorTheme(
                    colorTheme,
                    ParseThemesMetadata(jPackageJson)
                );

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

        private async Task<VSCodeTheme> ParseThemesAndUpdateColorTheme(
            VSCodeTheme colorThemeToUpdate,
            List<ThemeMetadata> themesMetadata)
        {
            foreach (var metadata in themesMetadata)
            {
                try
                {
                    var theme = await ParseTheme(metadata);
                    colorThemeToUpdate
                        .Themes.Add(theme);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error while handling {MessageType} message with id {MessageId}.");
                }
            }
            return colorThemeToUpdate;
        }

        private async Task<Theme> ParseTheme(ThemeMetadata metadata)
        {
            var jTheme = await LoadJsonFile(metadata.Path);
            var rootThemeFileFolder = Path.GetDirectoryName(metadata.Path);
            jTheme = await MergeExternalThemeFile(jTheme, rootThemeFileFolder);

            return Theme.FromJson(jTheme, metadata.Label, metadata.ThemeType);
        }

        private async Task<JObject> MergeExternalThemeFile(JObject jTheme, string rootThemeFileFolder)
        {
            Console.WriteLine("---- Started merging external theme file.");
            var include = (string) jTheme.SelectToken("include");
            if (include != null)
            {
                var includeFile = Path
                    .GetFullPath(
                        Path.Combine(rootThemeFileFolder, include)
                    );

                Console.WriteLine($"---- Merging external theme file: '{includeFile}'.");
                var jThemeToInclude = await LoadJsonFile(includeFile);

                jThemeToInclude = await MergeExternalThemeFile(jThemeToInclude, rootThemeFileFolder);

                jTheme.Merge(jThemeToInclude);

                Console.WriteLine($"---- Merging with '{includeFile}' completed.");
                return jTheme;
            }
            Console.WriteLine("---- Finished merging external theme file.");
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
