using System.Diagnostics;
using Newtonsoft.Json;

namespace TehGM.PoeQualityPermutations
{
    [DebuggerDisplay("PoE " + nameof(Item) + ": {ToString(),nq}")]
    class Item
    {
        [JsonProperty("id")]
        public string ID { get; private set; }
        [JsonProperty("icon")]
        public string IconURL { get; private set; }
        [JsonProperty("w")]
        public uint Width { get; private set; }
        [JsonProperty("h")]
        public uint Height { get; private set; }
        [JsonProperty("stackSize")]
        public uint StackedCount { get; private set; }
        [JsonProperty("maxStackSize")]
        public uint StackMaxCount { get; private set; }
        [JsonProperty("league")]
        public string League { get; private set; }
        [JsonProperty("name")]
        public string Name { get; private set; }
        [JsonProperty("typeLine")]
        public string TypeName { get; private set; }
        [JsonProperty("identified")]
        public bool IsIdentified { get; private set; }
        [JsonProperty("ilvl")]
        public int Level { get; private set; }
        [JsonProperty("descrText")]
        public string Description { get; private set; }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return TypeName;
            return $"{Name}, {TypeName}";
        }
    }
}
