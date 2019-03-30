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
    }

    public class VSMarketplaceClient : IVSMarketplaceClient
    {
        private readonly HttpClient _httpClient;

        public VSMarketplaceClient(HttpClient client)
        {
            client.BaseAddress = new Uri("https://marketplace.visualstudio.com/");
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
                {
                    Parameters = { new NameValueHeaderValue("api-version", "3.0-preview.1") }
                }
            );

            _httpClient = client;
        }

        public async Task<ExtensionQueryResponseMetadata> GetGalleryMetadata(int pageNumber, int pageSize)
        {
            try
            {
                Log.Information($"-- Requesting: pageNumber={pageNumber} & pageSize={pageSize}");

                var response = await _httpClient.PostAsync(
                    "_apis/public/gallery/extensionquery",
                    GetExtensionQueryRequestContent(pageNumber, pageSize)
                );

                response.EnsureSuccessStatusCode();

                var result = await ProcessResponseContent(response.Content);
                Log.Information($"Successfully processed {result.Items.Count} items");

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while requesting gallery metadata (pageNumber={pageNumber} & pageSize={pageSize}).");
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

                var metadata = new ExtensionQueryResponseMetadata
                {
                    RequestResultTotalCount = requestResultCount
                };

                foreach (var jExt in (JArray) jObject.SelectToken("results[0].extensions"))
                {
                    try
                    {
                        metadata.Items.Add(ParseExtensionMetadata((JObject) jExt));
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, $"Error while parsing json: '{jExt}'");
                    }
                }

                return metadata;
            }
        }

        private ExtensionMetadata ParseExtensionMetadata(JObject jObject)
        {
            var extensionInfo = ExtensionMetadata.FromJson(jObject);

            return extensionInfo;
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
