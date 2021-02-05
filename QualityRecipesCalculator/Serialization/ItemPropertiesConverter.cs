using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TehGM.PoE.QualityRecipesCalculator.Serialization
{
    public class ItemPropertiesConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IEnumerable<ItemProperty>).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray jsonArray = JArray.Load(reader);
            List<ItemProperty> results = new List<ItemProperty>(jsonArray.Count);
            foreach (JToken obj in jsonArray)
            {
                string template = obj["name"].Value<string>();
                JArray jsonValues = obj["values"] as JArray;
                string[] values = new string[jsonValues.Count];
                for (int i = 0; i < jsonValues.Count; i++)
                    values[i] = jsonValues[i].First.ToString();
                results.Add(new ItemProperty(template, values));
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
