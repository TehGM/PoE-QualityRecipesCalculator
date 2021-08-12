using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Serilog;

namespace TehGM.PoE.QualityRecipesCalculator.Calculators
{
    public abstract class RecipeCalculatorBase : IRecipeCalculator
    {
        public const int DefaultMaxItems = 60;
        public const int DefaultTargetQuality = 40;

        public ProcessStatus Status { get; protected set; }
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public event EventHandler<ProcessStatus> StatusUpdated;
        public event EventHandler<IEnumerable<Item>> ItemsFound;
        public event EventHandler<IEnumerable<IEnumerable<KeyValuePair<Item, int>>>> PermutationsFound;

        public abstract CalculationsResult Calculate(StashTab tab);

        protected CalculationsResult CheckRecipe(IEnumerable<Item> items, StashTab tab, int maxItems = DefaultMaxItems, int targetQuality = DefaultTargetQuality)
        {
            if (items?.Any() != true)
                return new CalculationsResult(tab, targetQuality);

            this._stopwatch.Restart();

            // prepare qualities and combinations
            IReadOnlyDictionary<Item, int> qualities = RecipeCombination.ExtractItemQualities(items);
            Log.Verbose("Generating permutations");
            IEnumerable<IEnumerable<KeyValuePair<Item, int>>> permutations = Permutator.GetCombinations(qualities, maxItems);
            Log.Verbose("Permutations generated in {Time} ms", this._stopwatch.ElapsedMilliseconds);
            this.PermutationsFound?.Invoke(this, permutations);

            // calculate total quality of each combination
            Log.Verbose("Calculating combinations");
            int count = 0;
            int total = permutations.Count();
            this.UpdateProgress(count, total);

            List<RecipeCombination> perfectCombinations = new List<RecipeCombination>();
            List<RecipeCombination> validCombinations = new List<RecipeCombination>();
            List<RecipeCombination> invalidCombinations = new List<RecipeCombination>();

            foreach (IEnumerable<KeyValuePair<Item, int>> sequence in permutations)
            {
                RecipeCombination combination = RecipeCombination.Calculate(sequence, targetQuality);

                if (combination.TotalQuality == targetQuality)
                    perfectCombinations.Add(combination);
                else if (combination.TotalQuality > targetQuality)
                    validCombinations.Add(combination);
                else
                    invalidCombinations.Add(combination);
                this.UpdateProgress(++count, total);
            }

            this._stopwatch.Stop();
            this.UpdateProgress(total, total);
            Log.Verbose("Done checking stash tab {TabName} ({Time} ms)", tab.Name, this._stopwatch.ElapsedMilliseconds);

            return new CalculationsResult(tab, targetQuality,
                perfectCombinations, validCombinations, invalidCombinations);
        }

        protected void RaiseItemsFound(IEnumerable<Item> items)
            => this.ItemsFound?.Invoke(this, items);

        protected void UpdateSubStatus(string text)
        {
            this.Status.SubText = text;
            this.StatusUpdated?.Invoke(this, this.Status);
        }

        protected void UpdateProgress(int currentProgress, int total)
        {
            this.Status.CurrentProgress = currentProgress;
            this.Status.MaxProgress = total;

            this.StatusUpdated?.Invoke(this, this.Status);
        }
    }
}
