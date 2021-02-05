using System.Collections.Generic;
using System.Linq;

namespace TehGM.PoE.QualityRecipesCalculator
{
    public static class Permutator
    {
        // based on
        // https://www.codeproject.com/Articles/43767/A-C-List-Permutation-Iterator
        private static void RotateRight<T>(IList<T> sequence, int count)
        {
            T tmp = sequence.ElementAt(count - 1);
            sequence.RemoveAt(count - 1);
            sequence.Insert(0, tmp);
        }

        public static IEnumerable<IList<T>> GetPermutations<T>(IList<T> sequence)
            => GetPermutations(sequence, sequence.Count);

        public static IEnumerable<IList<T>> GetPermutations<T>(IList<T> sequence, int count)
        {
            if (count == 1) yield return sequence;
            else
            {
                for (int i = 0; i < count; i++)
                {
                    foreach (var perm in GetPermutations(sequence, count - 1))
                        yield return perm;
                    RotateRight(sequence, count);
                }
            }
        }

        // based on
        // https://stackoverflow.com/questions/52863636/c-sharp-all-unique-combinations-of-liststring
        public static IEnumerable<IEnumerable<T>> GetCombinations<T>(IEnumerable<T> sequence, int maxItems = 60)
        {
            if (sequence.Count() == 1)
                yield return sequence;
            else
            {
                T head = sequence.First();
                IEnumerable<T> tail = sequence.Skip(1);
                foreach (IEnumerable<T> s in GetCombinations(tail))
                {
                    int count = s.Count();
                    if (count <= maxItems)
                        yield return s; // Without first
                    else 
                        continue;
                    if (count + 1 <= maxItems)
                        yield return s.Prepend(head);
                }
            }
        }
    }
}
