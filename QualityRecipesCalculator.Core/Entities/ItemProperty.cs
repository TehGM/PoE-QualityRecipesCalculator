using System.Collections.Generic;
using System.Linq;

namespace TehGM.PoE
{
    public class ItemProperty
    {
        public string Name { get; }
        public IEnumerable<string> Values { get; }
        public string Text => string.Format(this.Name, this.Values.ToArray());

        public ItemProperty(string name, IEnumerable<string> values)
        {
            this.Name = name;
            this.Values = values;
        }
    }
}
