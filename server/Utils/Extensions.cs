using System;
using System.Collections.Generic;

namespace CodePaint.WebApi.Utils {

    public static class Extensions {
        /// <summary>
        /// Get a the value for a key. If the key does not exist, return default value;
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="dictionary">The dictionary to call this method on.</param>
        /// <param name="key">The key to look up.</param>
        /// <returns>The key value. null if this key is not in the dictionary.</returns>
        public static TValue GetValueOrDefault<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key
        ) {
            TValue result;
            return dictionary.TryGetValue(key, out result) ? result : default(TValue);
        }
    }
}
