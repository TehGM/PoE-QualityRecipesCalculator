﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Serilog;

namespace TehGM.PoE.QualityRecipesCalculator
{
    public class RecipesCalculator
    {
        private readonly IEnumerable<StashTab> _stashTabs;
        private readonly Options _options;
        private readonly Stopwatch _stopwatch;

        public RecipesCalculator(IEnumerable<StashTab> stashTabs, Options options)
        {
            this._stashTabs = stashTabs;
            this._options = options;
            this._stopwatch = new Stopwatch();
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
                    && i.TryGetProperty("Consumes {0} of {1} Charges on use", out _)
                    && i.TryGetProperty("Quality", out _));
                if (!items.Any())
                    continue;
                Log.Debug("Found {Count} valid items in tab {TabName}, checking qualities", items.Count(), tab.Name);
                CheckRecipe(items, tab, new CombinationRequirements()
                {
                    MaxItems = 30
                });
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
                    && i.TryGetProperty("Quality", out _));
                if (!items.Any())
                    continue;
                Log.Debug("Found {Count} valid items in tab {TabName}, checking qualities", items.Count(), tab.Name);
                CheckRecipe(items, tab, new CombinationRequirements()
                {
                    MaxItems = 60
                });
            }
        }

        private void CheckRecipe(IEnumerable<Item> items, StashTab tab, CombinationRequirements requirements)
        {
            LogLargeWarning(items.Count(), tab.Name);
            this._stopwatch.Restart();

            // prepare qualities and combinations
            IDictionary<Item, int> qualities = RecipeCombination.ExtractItemQualities(items);
            Log.Verbose("Generating permutations");
            IEnumerable<IEnumerable<KeyValuePair<Item, int>>> permutations = Permutator.GetCombinations(qualities, requirements.MaxItems);

            Log.Verbose("Creating hashset to keep track of valid combinations");
            // track already done to reduce spam in output
            HashSet<RecipeCombination> alreadyDone = new HashSet<RecipeCombination>();
            Log.Verbose("Created hashset!");
            // only output tab name the first time
            bool tabNameShown = false;
            // skip showing invalid if capacity is exceeded
            //bool exceedsCapacity = permutations.LongCount() > int.MaxValue / 2;
            bool exceedsCapacity = true;
            Log.Verbose("Checking if exceeds capacity");
            if (exceedsCapacity && _options.ShowInvalid)
            {
                Log.Verbose("Exceeds capacity!");
                Log.Warning("Possible combinations count exceed capacity - logging of invalid combinations will be disabled");
            }
            Log.Verbose("Permutations generated in {Time} ms", this._stopwatch.ElapsedMilliseconds);

            // calculate total quality of each combination
            Log.Verbose("Calculating combinations");

            Log.Verbose("Total of {PermutationCount} permutations; {IntMax} is IntMax", permutations.LongCount(), int.MaxValue);
            int lastIter = permutations.Count();
            for (int iter = 0; iter < lastIter; iter++)
            {
                /*
                if ((iter + 1) % 1000 == 0)
                    Log.Verbose("Calculated {xThousand}k combinations..", iter / 1000);
                */
                Log.Verbose($"Combination {iter}");
                IEnumerable<KeyValuePair<Item, int>> sequence = permutations.ElementAt(iter);
                RecipeCombination combination = RecipeCombination.Calculate(sequence, requirements.TargetQuality);

                // determine if set should be shown
                if (_options.OnlyExact && combination.TotalQuality != requirements.TargetQuality)
                    continue;
                if ((!_options.ShowInvalid || exceedsCapacity) && combination.TotalQuality < requirements.TargetQuality)
                    continue;

                // ensure this set wasn't already calculated if not removing used combinations, based just on items qualities
                if (!_options.RemoveUsed && !alreadyDone.Add(combination))
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
                if (combination.TotalQuality == requirements.TargetQuality)
                    Console.ForegroundColor = ConsoleColor.Green;
                else if (combination.TotalQuality > requirements.TargetQuality)
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                else
                    Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(combination.TotalQuality);
                Console.ForegroundColor = previousColor;
                if (_options.ShowItemNames)
                    Console.Write($" ({string.Join(", ", combination.Items)})");
                Console.WriteLine();

                if (_options.RemoveUsed)
                {
                    Log.Verbose("Removing used qualities");
                    foreach (int quality in combination.Qualities)
                    {
                        foreach (KeyValuePair<Item, int> qualItem in qualities)
                        {
                            if (quality == qualItem.Value)
                            {
                                qualities.Remove(qualItem);
                                break;
                            }
                        }
                    }
                    Log.Verbose("Generating new permutations");
                    permutations = Permutator.GetCombinations(qualities, requirements.MaxItems);

                    iter--;
                    lastIter = permutations.Count();
                }
            }

            Log.Verbose("Done checking stash tab {TabName} ({Time} ms)", tab.Name, this._stopwatch.ElapsedMilliseconds);
        }

        private void LogLargeWarning(int itemsCount, string tabName)
        {
            if (itemsCount < _options.LargeBatchSize)
                return;
            string message = "{Count} valid items in tab {TabName} - calculations might take a long time";
            if (!_options.OnlyExact)
                message += ", consider running with --only-exact flag - might improve performance by about 30%";
            Log.Warning(message, itemsCount, tabName);
            return;
        }
    }
}
