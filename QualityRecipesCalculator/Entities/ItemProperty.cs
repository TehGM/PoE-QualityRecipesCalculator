using System.Collections.Generic;

namespace TehGM.PoE.QualityRecipesCalculator
{
    class ItemProperty
    {
        public string Name { get; }
        public IEnumerable<string> Values { get; }

        public ItemProperty(string name, IEnumerable<string> values)
        {
            this.Name = name;
            this.Values = values;
        }
    }
}
