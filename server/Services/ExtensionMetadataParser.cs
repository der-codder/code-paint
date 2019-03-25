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
    public interface IExtensionMetadataParser
    {
        string GetExtensionVersion(JObject jObject);
        List<ThemeMetadata> GetThemesMetadata(JObject jObject, string pathToExtension);
    }

    public class ExtensionMetadataParser : IExtensionMetadataParser
    {
        public string GetExtensionVersion(JObject jObject)
        {
            return jObject.SelectToken("version", true).ToString();
        }

        public List<ThemeMetadata> GetThemesMetadata(JObject jObject, string pathToExtension)
        {
            var metadata = new List<ThemeMetadata>();
            var jThemes = (JArray) jObject.SelectToken("contributes.themes");

            if (jThemes == null)
            {
                return metadata;
            }

            foreach (var jTheme in jThemes)
            {
                var path = (string) jTheme.SelectToken("path");
                if (!string.IsNullOrWhiteSpace(path))
                {
                    path = Path.GetFullPath(
                        Path.Combine(
                            pathToExtension,
                            "extension",
                            path
                        )
                    );
                }
                var themeType = (string) jTheme.SelectToken("uiTheme") ?? "vs-dark";
                var label = (string) jTheme.SelectToken("label") ?? Path.GetFileName(path);

                var themeMetadata = new ThemeMetadata
                {
                    Label = label,
                    ThemeType = themeType,
                    Path = path
                };

                metadata.Add(themeMetadata);
            }

            return metadata;
        }
    }
}
