using System;
using System.Linq;

namespace TehGM.PoE
{
    public static class ItemExtensions
    {
        public static ItemProperty GetProperty(this Item item, Func<ItemProperty, bool> predicate)
            => item.Properties?.FirstOrDefault(predicate);

        public static bool TryGetProperty(this Item item, Func<ItemProperty, bool> predicate, out ItemProperty result)
        {
            result = GetProperty(item, predicate);
            return result != null;
        }

        public static ItemProperty GetProperty(this Item item, string name)
            => GetProperty(item, i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        public static bool TryGetProperty(this Item item, string name, out ItemProperty result)
        {
            result = GetProperty(item, name);
            return result != null;
        }
    }
}
