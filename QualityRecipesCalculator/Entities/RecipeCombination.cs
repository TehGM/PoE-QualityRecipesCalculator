using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TehGM.PoE.QualityRecipesCalculator
{
    public class RecipeCombination : IEnumerable<KeyValuePair<Item, int>>, IEquatable<RecipeCombination>
    {
        private readonly ICollection<KeyValuePair<Item, int>> _items;
        public IEnumerable<Item> Items => _items.Select(i => i.Key);
        public IEnumerable<int> Qualities => _items.Select(i => i.Value);
        public int TotalQuality { get; }

        public RecipeCombination(IEnumerable<KeyValuePair<Item, int>> items)
        {
            this._items = new List<KeyValuePair<Item, int>>(items.OrderBy(i => i.Value));
            this.TotalQuality = this._items.Sum(kvp => kvp.Value);
        }

        public RecipeCombination(IEnumerable<Item> items)
            : this(ExtractItemQualities(items)) { }

        public static RecipeCombination Calculate(IEnumerable<KeyValuePair<Item, int>> items, int targetQuality = 40)
        {
            int quality = 0;
            return new RecipeCombination(items.TakeWhile(i =>
            {
                if (quality < targetQuality)
                {
                    quality += i.Value;
                    return true;
                }
                // skip calculating further if target quality was reached
                return false;
            }));
        }

        public static RecipeCombination Calculate(IEnumerable<Item> items, int targetQuality = 40)
            => Calculate(ExtractItemQualities(items), targetQuality);

        public static IReadOnlyDictionary<Item, int> ExtractItemQualities(IEnumerable<Item> items)
        {
            Dictionary<Item, int> results = new Dictionary<Item, int>(items.Count());

            foreach (Item i in items)
            {
                if (!i.TryGetProperty("Quality", out ItemProperty prop))
                    throw new ArgumentException($"Item {i} has no quality property", nameof(items));
                results.Add(i, int.Parse(prop.Values.First().TrimStart('+').TrimEnd('%')));
            }
            return results;
        }

        public override string ToString()
            => string.Join(", ", Qualities.Select(i => $"+{i}%"));

        public IEnumerator<KeyValuePair<Item, int>> GetEnumerator()
            => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _items.GetEnumerator();

        public override bool Equals(object obj)
            => Equals(obj as RecipeCombination);

        public bool Equals(RecipeCombination other)
            => other != null && this.Qualities.SequenceEqual(other.Qualities);

        public override int GetHashCode()
        {
            // based on https://antonymale.co.uk/implementing-equals-and-gethashcode-in-csharp.html
            unchecked
            {
                int hash = 17;
                foreach (int quality in this.Qualities)
                    hash = hash * 23 + EqualityComparer<int>.Default.GetHashCode(quality);
                return hash;
            }
        }

        public static bool operator ==(RecipeCombination left, RecipeCombination right)
            => EqualityComparer<RecipeCombination>.Default.Equals(left, right);

        public static bool operator !=(RecipeCombination left, RecipeCombination right)
            => !(left == right);
    }
}
