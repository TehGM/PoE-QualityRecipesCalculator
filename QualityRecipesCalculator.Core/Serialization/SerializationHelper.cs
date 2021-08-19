using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TehGM.PoE.Serialization
{
    public static class SerializationHelper
    {
        /// <summary>Default serializer settings.</summary>
        public static readonly JsonSerializerSettings SerializerSettings;
        /// <summary>Default serializer.</summary>
        public static readonly JsonSerializer DefaultSerializer;

        static SerializationHelper()
        {
            SerializerSettings = new JsonSerializerSettings();
            SerializerSettings.Converters.Add(new ColorConverter());
            SerializerSettings.Converters.Add(new ItemPropertiesConverter());
            SerializerSettings.Formatting = Formatting.None;

            DefaultSerializer = JsonSerializer.CreateDefault(SerializerSettings);
        }

        /// <summary>Populates object with Json token's properties.</summary>
        /// <remarks>It's not recommended to use this class unless it's required for writing a custom serializer implementation.</remarks>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="token">Token to use for populating.</param>
        /// <param name="target">Object to populate.</param>
        /// <param name="childPath">Selector of the child token in the <paramref name="token"/>.</param>
        /// <param name="serializer">Serializer to use. If null, <see cref="DefaultSerializer"/> will be used.</param>
        public static void PopulateObject<T>(this JToken token, T target, string childPath = null, JsonSerializer serializer = null)
        {
            JToken source = childPath != null ? token.SelectToken(childPath) : token;
            // sometimes body can be an array - if target is not an enumerable, ignore
            bool a = source is JArray;
            bool b = target is IEnumerable;
            if (source is JArray && !(target is IEnumerable))
                return;
            if (source == null)
                return;
            using (JsonReader reader = source.CreateReader())
            {
                if (serializer == null)
                    SerializationHelper.DefaultSerializer.Populate(reader, target);
                else
                    serializer.Populate(reader, target);
            }
        }
    }
}