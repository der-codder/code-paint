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
        (string Id, string Version) GetExtensionMetadata(JObject jObject);
        List<ThemeMetadata> GetThemesMetadata(JObject jObject, string pathToExtension);
    }

    public class ExtensionMetadataParser : IExtensionMetadataParser
    {
        public (string Id, string Version) GetExtensionMetadata(JObject jObject)
        {
            var extPublisher = jObject.SelectToken("publisher", true).ToString();
            var extName = jObject.SelectToken("name", true).ToString();
            var extVersion = jObject.SelectToken("version", true).ToString();

            return ($"{extPublisher}.{extName}", extVersion);
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

                var themeMetadata = new ThemeMetadata
                {
                    Label = jTheme.SelectToken("label", true).ToString(),
                    ThemeType = themeType,
                    Path = path
                };

                metadata.Add(themeMetadata);
            }

            return metadata;
        }
    }
}
