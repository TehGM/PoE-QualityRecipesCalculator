using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TehGM.PoE.QualityRecipesCalculator.Serialization
{
    class ItemPropertiesConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IEnumerable<KeyValuePair<string, ItemProperty>>).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray jsonArray = JArray.Load(reader);
            Dictionary<string, ItemProperty> results = new Dictionary<string, ItemProperty>(jsonArray.Count, StringComparer.OrdinalIgnoreCase);
            foreach (JToken obj in jsonArray)
            {
                string name = obj["name"].Value<string>();
                JArray jsonValues = obj["values"] as JArray;
                string[] values = new string[jsonValues.Count];
                for (int i = 0; i < jsonValues.Count; i++)
                    values[i] = jsonValues[i].First.ToString();
                results.Add(name, new ItemProperty(name, values));
            }
            return results;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // with current impl, cannot add JSON, as the reader skips some data that is not needed but is in the original JSON
            throw new NotImplementedException();
        }
    }
}
