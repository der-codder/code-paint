using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace CodePaint.WebApi.Utils
{
    public static class Extensions
    {
        /// <summary>
        /// Get a the value for a key. If the key does not exist, return default value.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="dictionary">The dictionary to call this method on.</param>
        /// <param name="key">The key to look up.</param>
        /// <returns>The key value. null if this key is not in the dictionary.</returns>
        public static TValue GetValueOrDefault<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key
        ) => dictionary.TryGetValue(key, out var result) ? result : default(TValue);

        /// <summary>
        /// Convert JArray object to Dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="keyIdentifier">Identifier of key in JArray item.</param>
        /// <param name="valueIdentifier">Identifier of value in JArray item.</param>
        /// <example> 
        /// JSON array to convert:
        /// <code>
        /// [{
        ///     "assetType": "Icons.Default",
        ///     "source: "url_to_default_icon"
        ///   }, {
        ///     "assetType": "Icons.Small",
        ///     "source: "url_to_small_icon"
        /// }]
        /// </code>
        /// Converts to something like this:
        /// <code>
        /// var myDict = new Dictionary<string, string>
        /// {
        ///     { "Icons.Default", "url_to_default_icon" },
        ///     { "Icons.Small", "url_to_small_icon" }
        /// };
        /// </code>
        /// </example>
        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(
            this JArray jArray,
            string keyIdentifier,
            string valueIdentifier)
        {
            if (jArray == null)
            {
                return new Dictionary<TKey, TValue>();
            }

            //var list = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, string>>>(jsonContent);
            //var dictionary = list.ToDictionary(x => x.Key, x => x.Value);
            var keyValuePairs = jArray
                .Select(
                    item => new KeyValuePair<TKey, TValue>(
                        item[keyIdentifier].Value<TKey>(),
                        item[valueIdentifier].Value<TValue>()
                    )
                )
                .ToList();

            return new Dictionary<TKey, TValue>(keyValuePairs);
        }
    }
}
