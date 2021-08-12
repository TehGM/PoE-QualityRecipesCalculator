using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Serilog;
using TehGM.PoE.QualityRecipesCalculator.Calculators;

namespace TehGM.PoE.QualityRecipesCalculator
{
    public class TerminalRecipesCalculator
    {
        private readonly IEnumerable<StashTab> _stashTabs;
        private readonly Options _options;
        private readonly Stopwatch _stopwatch;

        private readonly IRecipeCalculator _glassblowersBaubleCalculator;
        private readonly IRecipeCalculator _gemcuttersPrismCalculator;

        public TerminalRecipesCalculator(IEnumerable<StashTab> stashTabs, Options options)
        {
            this._stashTabs = stashTabs;
            this._options = options;
            this._stopwatch = new Stopwatch();

            this._glassblowersBaubleCalculator = new GlassblowersBaubleRecipeCalculator();
            this._gemcuttersPrismCalculator = new GemcuttersPrismRecipeCalculator();
        }

        public void CheckGlassblowersBaubleRecipe()
        {
            Log.Information("Checking for Glassblower's Bauble recipe (Flasks)");
            foreach (StashTab tab in _stashTabs)
            {
                Log.Verbose("Checking tab {TabName}", tab.Name);
                this.DisplayResults(tab, this._glassblowersBaubleCalculator);
            }
        }

        public void CheckGemcuttersPrismRecipe()
        {
            Log.Information("Checking for Gemcutter's Prism recipe (Gems)");
            foreach (StashTab tab in _stashTabs)
            {
                Log.Verbose("Checking tab {TabName}", tab.Name);
                this.DisplayResults(tab, this._gemcuttersPrismCalculator);
            }
        }

        private void DisplayResults(StashTab tab, IRecipeCalculator calculator)
        {
            bool exceedsCapacity = false;

            EventHandler<IEnumerable<Item>> itemsFoundCallback = (sender, args) =>
            {
                long itemsCount = args.LongCount();
                if (itemsCount < _options.LargeBatchSize)
                    return;
                string message = "{Count} valid items in tab {TabName} - calculations might take a long time";
                if (!_options.OnlyExact)
                    message += ", consider running with --only-exact flag - might improve performance by about 30%";
                Log.Warning(message, itemsCount, tab.Name);
            };
            EventHandler<IEnumerable<IEnumerable<KeyValuePair<Item, int>>>> permutationsFoundCallback = (sender, args) =>
            {
                exceedsCapacity = args.LongCount() > int.MaxValue / 2;
                // skip showing invalid if capacity is exceeded
                if (exceedsCapacity && _options.ShowInvalid)
                    Log.Warning("Possible combinations count exceed capacity - logging of invalid combinations will be disabled");
            };

            calculator.ItemsFound += itemsFoundCallback;
            calculator.PermutationsFound += permutationsFoundCallback;
            CalculationsResult result = calculator.Calculate(tab);
            calculator.ItemsFound -= itemsFoundCallback;
            calculator.PermutationsFound -= permutationsFoundCallback;

            // track already done just to reduce spam in output
            HashSet<RecipeCombination> alreadyDone = new HashSet<RecipeCombination>();

            bool showPerfect = result.PerfectCombinations?.Any() == true;
            bool showValid = !_options.OnlyExact && result.ValidCombinations?.Any() == true;
            bool showInvalid = _options.ShowInvalid && result.InvalidCombinations?.Any() == true;
            // show tab name if there's anything to show
            if (showPerfect || showValid || showInvalid)
            {
                Log.Information("Found possible trades with items from tab {TabName}", tab);

                if (showPerfect)
                    this.WriteResults(result.PerfectCombinations, alreadyDone, ConsoleColor.Green);
                if (showValid)
                    this.WriteResults(result.ValidCombinations, alreadyDone, ConsoleColor.DarkGreen);
                if (showInvalid)
                    this.WriteResults(result.InvalidCombinations, alreadyDone, ConsoleColor.Red);
            }
        }

        private void WriteResults(IEnumerable<RecipeCombination> combinations, HashSet<RecipeCombination> alreadyDone, ConsoleColor qualityColor)
        {
            foreach (RecipeCombination combination in combinations)
            {
                // ensure this set wasn't already calculated, based just on items qualities
                if (!alreadyDone.Add(combination))
                    continue;

                // output the set and total quality
                Console.Write(combination.ToString() + ": ");
                ConsoleColor previousColor = Console.ForegroundColor;
                Console.ForegroundColor = qualityColor;
                Console.Write(combination.TotalQuality);
                Console.ForegroundColor = previousColor;
                if (_options.ShowItemNames)
                    Console.Write($" ({string.Join(", ", combination.Items)})");
                Console.WriteLine();
            }
        }
    }
}
