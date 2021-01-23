using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace TehGM.PoE.QualityRecipesCalculator
{
    public class RecipesCalculator
    {
        public readonly IEnumerable<StashTab> _stashTabs;
        public readonly Options _options;

        public RecipesCalculator(IEnumerable<StashTab> stashTabs, Options options)
        {
            this._stashTabs = stashTabs;
            this._options = options;
        }

        public void CheckGlassblowersBaubleRecipe()
        {
            Log.Information("Checking for Glassblower's Bauble recipe (Flasks)");
            foreach (StashTab tab in _stashTabs)
            {
                Log.Verbose("Checking tab {TabName}", tab.Name);
                // flasks can have frame type 0, 1 or 3. They also always have "Consumes {0} of {1} Charges on use" as one of properties
                // also we're only interested in flasks with quality
                IEnumerable<Item> items = tab.Items.Where(i => (i.FrameType == 0 || i.FrameType == 1 || i.FrameType == 3)
                    && i.Properties?.ContainsKey("Consumes {0} of {1} Charges on use") == true
                    && i.Properties?.ContainsKey("Quality") == true);
                if (!items.Any())
                    continue;
                Log.Debug("Found valid items in tab {TabName}, checking qualities", tab.Name);
                CheckRecipe(items, tab);
            }
        }

        public void CheckGemcuttersPrismRecipe()
        {
            Log.Information("Checking for Gemcutter's Prism recipe (Gems)");
            foreach (StashTab tab in _stashTabs)
            {
                Log.Verbose("Checking tab {TabName}", tab.Name);
                // gems always have frame type of 4
                // also we're only interested in gems with quality
                IEnumerable<Item> items = tab.Items.Where(i => i.FrameType == 4
                    && i.Properties?.ContainsKey("Quality") == true);
                if (!items.Any())
                    continue;
                Log.Debug("Found valid items in tab {TabName}, checking qualities", tab.Name);
                CheckRecipe(items, tab);
            }
        }

        private void CheckRecipe(IEnumerable<Item> items, StashTab tab, int targetQuality = 40)
        {
            // prepare qualities and combinations
            IReadOnlyDictionary<Item, int> qualities = RecipeCombination.ExtractItemQualities(items);
            Log.Verbose("Generating permutations");
            IEnumerable<IEnumerable<KeyValuePair<Item, int>>> permutations = Permutator.GetCombinations(qualities);

            // track already done just to reduce spam in output
            HashSet<RecipeCombination> alreadyDone = new HashSet<RecipeCombination>(permutations.Count());
            // only output tab name the first time
            bool tabNameShown = false;

            // calculate total quality of each combination
            foreach (IEnumerable<KeyValuePair<Item, int>> sequence in permutations)
            {
                RecipeCombination combination = RecipeCombination.Calculate(sequence, targetQuality);

                // ensure this set wasn't already calculated, based just on items qualities
                if (!alreadyDone.Add(combination))
                    continue;

                // determine if set should be shown
                if (!_options.ShowInvalid && combination.TotalQuality < targetQuality)
                    continue;
                if (_options.OnlyExact && combination.TotalQuality != targetQuality)
                    continue;

                // for the first item in set, notify user what tab it's in
                if (!tabNameShown)
                {
                    Log.Information("Found possible trades with items from tab {TabName}", tab);
                    tabNameShown = true;
                }

                // output the set and total quality
                Console.Write(combination.ToString() + ": ");
                ConsoleColor previousColor = Console.ForegroundColor;
                if (combination.TotalQuality == targetQuality)
                    Console.ForegroundColor = ConsoleColor.Green;
                else if (combination.TotalQuality > targetQuality)
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                else
                    Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(combination.TotalQuality);
                Console.ForegroundColor = previousColor;
                if (_options.ShowItemNames)
                    Console.Write($" ({string.Join(", ", combination.Items)})");
                Console.WriteLine();
            }
        }
    }
}
