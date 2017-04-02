using KitchenSink.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KitchenSink.Extensions
{
    public static class DictionaryExtensions
    {
        public static ILookup<TKey, TValue> AsLookup<TKey, TValue>(
            this IDictionary<TKey, IEnumerable<TValue>> dict)
        {
            return new DictionaryLookup<TKey, TValue>(dict);
        }
    }
}
