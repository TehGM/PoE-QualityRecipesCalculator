using System.Collections.Generic;
using System.Linq;

namespace TehGM.PoE.QualityRecipesCalculator
{
    public class CalculationsResult
    {
        public StashTab Tab { get; }
        public int TargetQuality { get; }

        public IEnumerable<RecipeCombination> PerfectCombinations { get; init; }
        public IEnumerable<RecipeCombination> ValidCombinations { get; init; }
        public IEnumerable<RecipeCombination> InvalidCombinations { get; init; }

        public CalculationsResult(StashTab tab, int targetQuality,
            IEnumerable<RecipeCombination> perfectCombinations, IEnumerable<RecipeCombination> validCombinations, IEnumerable<RecipeCombination> invalidCombinations)
        {
            this.Tab = tab;
            this.TargetQuality = targetQuality;

            this.PerfectCombinations = perfectCombinations ?? Enumerable.Empty<RecipeCombination>();
            this.ValidCombinations = validCombinations ?? Enumerable.Empty<RecipeCombination>();
            this.InvalidCombinations = invalidCombinations ?? Enumerable.Empty<RecipeCombination>();
        }

        public CalculationsResult(StashTab tab, int targetQuality)
            : this(tab, targetQuality, null, null, null) { }
    }
}
