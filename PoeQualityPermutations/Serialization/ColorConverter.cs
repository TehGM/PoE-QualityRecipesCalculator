using System;
using System.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TehGM.PoeQualityPermutations.Serialization
{
    public class ColorConverter : JsonConverter
    {
        public override bool CanConvert(Type typeToConvert)
            => typeof(Color) == typeToConvert || typeof(Color?) == typeToConvert;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;
            JObject json = JObject.Load(reader);
            return Color.FromArgb(json["r"].Value<byte>(), json["g"].Value<byte>(), json["b"].Value<byte>());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                return;
            Color color = (Color)value;
            JObject result = new JObject(
                new JProperty("r", color.R),
                new JProperty("g", color.G),
                new JProperty("b", color.B)
            );
            result.WriteTo(writer);
        }
    }
}
