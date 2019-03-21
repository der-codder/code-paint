using System;
using System.Collections.Generic;
using System.Globalization;
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
        Task<ExtensionQueryResponseMetadata> GetGalleryMetadata(int pageNumber, int pageSize);
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

        public async Task<ExtensionQueryResponseMetadata> GetGalleryMetadata(int pageNumber, int pageSize)
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
                    GetExtensionQueryRequestContent(pageNumber, pageSize)
                );

                if (!response.IsSuccessStatusCode)
                {
                    Log.Information($"Response is unsuccessful: {response.StatusCode}, {response.RequestMessage}");
                    throw new Exception("Ooooops!");
                }

                var result = await ProcessResponseContent(response.Content);
                Log.Information($"Successfully processed {result.Items.Count} gallery items");

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while requesting gallery metadata (pageNumber={pageNumber} & pageSize={pageSize}).");
                throw;
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

            var uri = $"/_apis/public/gallery/publishers/{publisherName}" +
                $"/vsextensions/{vsExtensionName}/{version}/vspackage";

            try
            {
                Log.Information($"Requesting vsix file stream from: {uri}");

                return await _client.GetStreamAsync(uri);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while requesting vsix file stream from: '{uri}'.");
                throw;
            }
        }

        private async Task<ExtensionQueryResponseMetadata> ProcessResponseContent(HttpContent content)
        {
            using (var s = await content.ReadAsStreamAsync())
            using (var sr = new StreamReader(s))
            using (var reader = new JsonTextReader(sr))
            {
                var jObject = await JObject.LoadAsync(reader);

                var requestResultCount = Convert.ToInt32(
                    jObject.SelectToken("results[0].resultMetadata[0].metadataItems[0].count"),
                    CultureInfo.InvariantCulture
                );

                var items = ((JArray) jObject.SelectToken("results[0].extensions"))
                    .Select(ext => ParseGalleryItemMetadata((JObject) ext));

                var metadata = new ExtensionQueryResponseMetadata { RequestResultTotalCount = requestResultCount };
                metadata.Items.AddRange(items);

                return metadata;
            }
        }

        private (GalleryItem, GalleryItemStatistic) ParseGalleryItemMetadata(JObject jObject)
        {
            // Log.Information($"Parsing Started: '{ext.ToString()}'");
            var itemInfo = GalleryItem.FromJson(jObject);
            var itemStatistic = GalleryItemStatistic.FromJson(jObject);

            Log.Information($"Parsed metadata for '{itemInfo.Id}'");

            return (itemInfo, itemStatistic);
        }

        private StringContent GetExtensionQueryRequestContent(int pageNumber, int pageSize)
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
