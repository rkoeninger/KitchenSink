using System.Collections.Generic;
using System.Linq;

namespace KitchenSink.Collections
{
    /// <summary>Utility methods for building Dictionaries.</summary>
    public static class Dictionary
    {
        /// <summary>Creates a new Dictionary from the properties of an object.</summary>
		/// <remarks>Intended to be used with an anonymous object, but can be used with any object.</remarks>
        public static Dictionary<string, object> Of(object obj)
        {
            return obj?.GetType()
                       .GetProperties()
                       .Where(x => x.GetIndexParameters().Length == 0)
                       .ToDictionary(x => x.Name, x => x.GetValue(obj, null)) ?? new Dictionary<string, object>();
        }
    }
}
