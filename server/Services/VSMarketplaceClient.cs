using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CodePaint.WebApi.Domain.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace CodePaint.WebApi.Services
{
    public interface IVSMarketplaceClient
    {
        Task<List<GalleryItemMetadata>> GetGalleryMetadata(int pageNumber, int pageSize);
        Task<Stream> GetVsixFileStream(string publisherName, string vsExtensionName, string version);
    }

    public class VSMarketplaceClient : IVSMarketplaceClient
    {
        private const string _marketplaceUri = "https://marketplace.visualstudio.com/";
        private readonly MediaTypeWithQualityHeaderValue _extensionQueryHeader;
        private readonly HttpClient _client;

        public VSMarketplaceClient(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri(_marketplaceUri);
            _extensionQueryHeader = new MediaTypeWithQualityHeaderValue("application/json")
            {
                Parameters = { new NameValueHeaderValue("api-version", "3.0-preview.1") }
            };

            _client = httpClient;
        }

        public async Task<List<GalleryItemMetadata>> GetGalleryMetadata(int pageNumber, int pageSize)
        {
            _client.DefaultRequestHeaders
                .Clear();
            _client.DefaultRequestHeaders.Accept
                .Add(_extensionQueryHeader);

            try
            {
                Log.Information($"Requesting: pageNumber={pageNumber} & pageSize={pageSize}");

                var response = await _client.PostAsync(
                    "/_apis/public/gallery/extensionquery",
                    GetRequestContent(pageNumber, pageSize)
                );

                if (!response.IsSuccessStatusCode)
                {
                    Log.Information($"Response is unsuccessful: {response.StatusCode}, {response.RequestMessage}");
                    return await Task.FromResult(new List<GalleryItemMetadata>());
                }

                var result = await ProcessResponseContent(response.Content);
                Log.Information($"Response is successful.");

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Caught exception");
                return await Task.FromResult(new List<GalleryItemMetadata>());
            }
        }

        public async Task<Stream> GetVsixFileStream(
            string publisherName,
            string vsExtensionName,
            string version)
        {
            if (string.IsNullOrWhiteSpace(publisherName))
            {
                throw new ArgumentNullException(nameof(publisherName));
            }

            if (string.IsNullOrWhiteSpace(vsExtensionName))
            {
                throw new ArgumentNullException(nameof(vsExtensionName));
            }

            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentNullException(nameof(version));
            }

            _client.DefaultRequestHeaders.Clear();

            try
            {
                var uri = $"/_apis/public/gallery/publishers/{publisherName}" +
                    $"/vsextensions/{vsExtensionName}/{version}/vspackage";

                Log.Information("Sending reguest to: {Uri}", uri);

                return await _client.GetStreamAsync(uri);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Caught exception");
                throw;
            }
        }

        private async Task<List<GalleryItemMetadata>> ProcessResponseContent(HttpContent content)
        {
            using (var s = await content.ReadAsStreamAsync())
            using (var sr = new StreamReader(s))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                var jObject = await JObject.LoadAsync(reader);

                return ((JArray) jObject.SelectToken("results[0].extensions"))
                    .Select(
                        ext =>
                        {
                            // Log.Information($"Parsing Started: '{ext.ToString()}'");
                            var themeInfo = ThemeInfo.FromJson((JObject) ext);
                            var themeStatistic = ThemeStatistic.FromJson((JObject) ext, themeInfo.Id);
                            var result = new GalleryItemMetadata
                            {
                                ThemeInfo = themeInfo,
                                ThemeStatistic = themeStatistic
                            };

                            Log.Information($"Parsed metadata for '{themeInfo.Id}'");

                            return result;
                        })
                    .ToList();
            }
        }

        private StringContent GetRequestContent(int pageNumber, int pageSize)
        {
            var body = "{\"filters\":[{\"criteria\":[{\"filterType\":8,\"value\":\"Microsoft.VisualStudio.Code\"}," +
                "{\"filterType\":12,\"value\":\"4096\"},{\"filterType\":5,\"value\":\"themes\"}]," +
                $"\"pageNumber\":{pageNumber},\"pageSize\":{pageSize},\"sortBy\":4,\"sortOrder\":0" +
                "}],\"assetTypes\":[\"Microsoft.VisualStudio.Services.Icons.Small\"," +
                "\"Microsoft.VisualStudio.Services.Icons.Default\"],\"flags\":898}";

            return new StringContent(
                body, Encoding.UTF8,
                "application/json"
            );
        }
    }
}
