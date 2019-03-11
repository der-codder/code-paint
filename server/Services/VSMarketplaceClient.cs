using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CodePaint.WebApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CodePaint.WebApi.Services
{
    public class VSMarketplaceClient
    {
        private const string _marketplaceUri = "https://marketplace.visualstudio.com/";
        private readonly HttpClient _client;

        public VSMarketplaceClient(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri(_marketplaceUri);

            var mthv = new MediaTypeWithQualityHeaderValue("application/json");
            mthv.Parameters.Add(new NameValueHeaderValue("api-version", "3.0-preview.1"));
            httpClient.DefaultRequestHeaders.Accept.Add(mthv);

            _client = httpClient;
        }

        public async Task<IEnumerable<ThemeInfo>> GetThemesInfoAsync(int pageNumber, int pageSize)
        {
            try
            {
                var response  = await _client.PostAsync(
                    "/_apis/public/gallery/extensionquery",
                    GetRequestContent(pageNumber, pageSize)
                );

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"\nResponse is unsuccessful: {response.StatusCode}, {response.RequestMessage}");
                    return await Task.FromResult(new List<ThemeInfo>());
                }

                using (Stream s = await response.Content.ReadAsStreamAsync())
                using (StreamReader sr = new StreamReader(s))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    var jObject = await JObject.LoadAsync(reader);

                    return ProcessExtensions((JArray)jObject.SelectToken("results[0].extensions"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Caught exception : " + ex);
                return await Task.FromResult(new List<ThemeInfo>());
            }
        }

        private IEnumerable<ThemeInfo> ProcessExtensions(JArray extensions)
        {
            return extensions
                .Select(ext => ThemeInfo.FromJson((JObject)ext))
                .ToList();
        }

        private StringContent GetRequestContent(int pageNumber, int pageSize)
        {
            var body = "{\"filters\":[{\"criteria\":[{\"filterType\":8,\"value\":\"Microsoft.VisualStudio.Code\"}," +
                "{\"filterType\":12,\"value\":\"4096\"},{\"filterType\":5,\"value\":\"themes\"}]," +
                $"\"pageNumber\":{pageNumber},\"pageSize\":{pageSize},\"sortBy\":4,\"sortOrder\":0" +
                "}],\"assetTypes\":[\"Microsoft.VisualStudio.Services.Icons.Small\"," +
                "\"Microsoft.VisualStudio.Services.Icons.Default\"],\"flags\":898}";

            return new StringContent(
                body,
                Encoding.UTF8,
                "application/json"
            );
        }
    }
}
