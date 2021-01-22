using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using Serilog;

namespace TehGM.PoeQualityPermutations
{
    class Program
    {
        private static Options _options;

        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Debug()
                .CreateLogger();

            await Parser.Default.ParseArguments<Options>(args).WithParsedAsync(async (options) =>
            {
                _options = options;
                using PoeHttpClient client = new PoeHttpClient(options.SessionID, options.AccountName);
                client.Realm = options.Realm;
                IEnumerable<StashTab> tabs = await client.GetStashTabsAsync(options.League).ConfigureAwait(false);

                //CheckCartographersChiselRecipe(tabs);
                CheckGlassblowersBaubleRecipe(tabs);
                CheckGemcuttersPrismRecipe(tabs);
            });
            Log.Information("Done");
            Console.ReadLine();
        }

        private static void CheckGlassblowersBaubleRecipe(IEnumerable<StashTab> stashTabs)
        {
            Log.Information("Checking for Glassblower's Bauble recipe (Flasks)");
            foreach (StashTab tab in stashTabs)
            {
                Log.Verbose("Checking tab {TabName}", tab.Name);
                // flasks can have frame type 0, 1 or 3. They also always have "Consumes {0} of {1} Charges on use" as one of properties
                // also we're only interested in flasks with quality
                IEnumerable<Item> items = tab.Items.Where(i => (i.FrameType == 0 || i.FrameType == 1 || i.FrameType == 3)
                    && i.Properties.ContainsKey("Consumes {0} of {1} Charges on use")
                    && i.Properties.ContainsKey("Quality"));
                if (!items.Any())
                    continue;
                Log.Debug("Found valid items in tab {TabName}, checking qualities", tab.Name);
                CheckRecipe(items, tab);
            }
        }

        private static void CheckGemcuttersPrismRecipe(IEnumerable<StashTab> stashTabs)
        {
            Log.Information("Checking for Gemcutter's Prism recipe (Gems)");
            foreach (StashTab tab in stashTabs)
            {
                Log.Verbose("Checking tab {TabName}", tab.Name);
                // gems always have frame type of 4
                // also we're only interested in gems with quality
                IEnumerable<Item> items = tab.Items.Where(i => i.FrameType == 4
                    && i.Properties.ContainsKey("Quality"));
                if (!items.Any())
                    continue;
                Log.Debug("Found valid items in tab {TabName}, checking qualities", tab.Name);
                CheckRecipe(items, tab);
            }
        }

        private static void CheckRecipe(IEnumerable<Item> items, StashTab tab, int targetQuality = 40)
        {
            IDictionary<Item, int> currentSet = new Dictionary<Item, int>(5);
            IReadOnlyDictionary<Item, int> qualities = ExtractItemQualities(items);
            Log.Verbose("Generating permutations");
            IEnumerable<IEnumerable<KeyValuePair<Item, int>>> permutations = Permutator.GetCombinations(qualities);
            // track already done just to reduce spam in output
            HashSet<string> alreadyDone = new HashSet<string>(permutations.Count(), StringComparer.Ordinal);
            // only output tab name the first time
            bool tabNameShown = false;
            foreach (IEnumerable<KeyValuePair<Item, int>> sequence in permutations)
            {
                currentSet.Clear();
                int quality = 0;
                foreach (KeyValuePair<Item, int> q in sequence)
                {
                    quality += q.Value;
                    currentSet.Add(q);
                    if (quality >= targetQuality)
                        break;
                }

                string key = string.Join(", ", currentSet.Select(q => 
                {
                    //if (_options.ShowItemNames)
                    //    return $"{q.Key} +{q.Value}%";
                    return $"+{q.Value}%";
                }));
                if (!alreadyDone.Add(key))
                    continue;

                if (_options.OnlyValid && quality < targetQuality)
                    continue;
                if (_options.OnlyExact && quality != targetQuality)
                    continue;

                if (!tabNameShown)
                {
                    Log.Information("Found possible trades with items from tab {TabName}", tab);
                    tabNameShown = true;
                }

                Console.Write(key + ": ");
                ConsoleColor previousColor = Console.ForegroundColor;
                if (quality == targetQuality)
                    Console.ForegroundColor = ConsoleColor.Green;
                else if (quality > targetQuality)
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                else
                    Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(quality);
                Console.ForegroundColor = previousColor;
                if (_options.ShowItemNames)
                    Console.Write($" ({string.Join(", ", currentSet.Keys)})");
                Console.WriteLine();
            }
        }

        private static void CheckCartographersChiselRecipe(IEnumerable<StashTab> stashTabs)
        {
            Log.Information("Checking for Cartographer's Chisel recipe (Maps)");
            foreach (StashTab tab in stashTabs)
            {
                throw new NotImplementedException();
            }
        }

        private static IReadOnlyDictionary<Item, int> ExtractItemQualities(IEnumerable<Item> items)
        {
            Dictionary<Item, int> results = new Dictionary<Item, int>(items.Count());

            foreach (Item i in items)
            {
                if (!i.Properties.TryGetValue("Quality", out ItemProperty prop))
                    throw new ArgumentException($"Item {i} has no quality property", nameof(items));
                results.Add(i, int.Parse(prop.Values.First().TrimStart('+').TrimEnd('%')));
            }
            return results;
        }
    }
}
