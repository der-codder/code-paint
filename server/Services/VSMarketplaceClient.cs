using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CodePaint.WebApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CodePaint.WebApi.Services {

    public class VSMarketplaceClient : IVSMarketplaceClient {
        private const string _marketplaceUri = "https://marketplace.visualstudio.com/";
        private readonly HttpClient _client;

        public VSMarketplaceClient(HttpClient httpClient) {
            httpClient.BaseAddress = new Uri(_marketplaceUri);

            _client = httpClient;
        }

        public async Task<IEnumerable<ThemeInfo>> GetThemesInfoAsync(int pageNumber, int pageSize) {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept
                .Add(
                    new MediaTypeWithQualityHeaderValue("application/json") {
                        Parameters = { new NameValueHeaderValue("api-version", "3.0-preview.1") }
                    }
                );

            try {
                var response = await _client.PostAsync(
                    "/_apis/public/gallery/extensionquery",
                    GetRequestContent(pageNumber, pageSize)
                );

                if (!response.IsSuccessStatusCode) {
                    Console.WriteLine($"\nResponse is unsuccessful: {response.StatusCode}, {response.RequestMessage}");
                    return await Task.FromResult(new List<ThemeInfo>());
                }

                return await ProcessResponseContent(response.Content);
            }
            catch (Exception ex) {
                Console.WriteLine("Caught exception : " + ex);
                return await Task.FromResult(new List<ThemeInfo>());
            }
        }

        public async Task<Stream> GetVsixFileStream(string publisherName, string vsExtensionName, string version) {
            if (string.IsNullOrWhiteSpace(publisherName))
                throw new ArgumentNullException("publisherName");
            if (string.IsNullOrWhiteSpace(vsExtensionName))
                throw new ArgumentNullException("vsExtensionName");
            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentNullException("version");

            _client.DefaultRequestHeaders.Clear();

            try {
                var uri = $"/_apis/public/gallery/publishers/{publisherName}" +
                    $"/vsextensions/{vsExtensionName}/{version}/vspackage";

                Console.WriteLine($"---- Sending reguest to: {uri}");

                return await _client.GetStreamAsync(uri);

                // if (!response.IsSuccessStatusCode) {
                //     Console.WriteLine($"\nResponse is unsuccessful: {response.StatusCode}, {response.RequestMessage}");
                //     throw new Exception();
                // }

                // return await response.Content.ReadAsStreamAsync();
            }
            catch (Exception ex) {
                Console.WriteLine("Caught exception : " + ex);
                throw;
            }
        }

        private async Task<IEnumerable<ThemeInfo>> ProcessResponseContent(HttpContent content) {
            using(Stream s = await content.ReadAsStreamAsync())
            using(StreamReader sr = new StreamReader(s))
            using(JsonReader reader = new JsonTextReader(sr)) {
                var jObject = await JObject.LoadAsync(reader);

                return ProcessExtensions((JArray) jObject.SelectToken("results[0].extensions"));
            }
        }

        private IEnumerable<ThemeInfo> ProcessExtensions(JArray extensions) {
            return extensions
                .Select(ext => ThemeInfo.FromJson((JObject) ext))
                .ToList();
        }

        private StringContent GetRequestContent(int pageNumber, int pageSize) {
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
