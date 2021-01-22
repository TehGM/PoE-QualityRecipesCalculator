using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Newtonsoft.Json;

namespace TehGM.PoE.QualityRecipesCalculator
{
    [DebuggerDisplay("PoE " + nameof(StashTab) + ": {ToString(),nq}")]
    class StashTab
    {
        [JsonProperty("n")]
        public string Name { get; private set; }
        [JsonProperty("i")]
        public int Index { get; private set; }
        [JsonProperty("id")]
        public string ID { get; private set; }
        [JsonProperty("type")]
        public TabType Type { get; private set; }
        [JsonProperty("selected")]
        public bool IsSelected { get; private set; }
        [JsonProperty("colour")]
        public Color Colour { get; private set; }
        [JsonProperty("items")]
        public IEnumerable<Item> Items { get; private set; }

        public enum TabType
        {
            NormalStash,
            CurrencyStash,
            DivinationCardStash,
            MapStash,
            FragmentStash,
            QuadStash,
            PremiumStash,
            BlightStash,
            MetamorphStash,
            EssenceStash,
            DeliriumStash,
            DelveStash,
            UniqueStash
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
