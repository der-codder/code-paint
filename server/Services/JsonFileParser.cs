using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace CodePaint.WebApi.Services
{
    public interface IJsonFileLoader
    {
        Task<JObject> Load(string path);
    }

    public class JsonFileLoader : IJsonFileLoader
    {
        public async Task<JObject> Load(string path)
        {
            Log.Information($"Loading json file: '{path}'.");

            using (var sr = new StreamReader(File.OpenRead(path)))
            using (var reader = new JsonTextReader(sr))
            {
                var result = await JObject.LoadAsync(reader);

                Log.Information("Loading completed.");
                return result;
            }
        }
    }
}
