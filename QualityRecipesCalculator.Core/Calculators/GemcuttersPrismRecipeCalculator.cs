using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace TehGM.PoE.QualityRecipesCalculator.Calculators
{
    public class GemcuttersPrismRecipeCalculator : RecipeCalculatorBase, IRecipeCalculator
    {
        public override CalculationsResult Calculate(StashTab tab)
        {
            base.Status = new ProcessStatus("Checking for Gemcutter's Prism recipe (Gems)");
            base.UpdateSubStatus($"Checking tab {tab.Name}");

            // gems always have frame type of 4
            // also we're only interested in gems with quality
            IEnumerable<Item> items = tab.Items.Where(i => i.FrameType == 4
                && i.TryGetProperty("Quality", out _));
            base.RaiseItemsFound(items);

            if (!items.Any())
                return new CalculationsResult(tab, DefaultTargetQuality);
            base.UpdateSubStatus($"Found {items.Count()} valid items in tab {tab.Name}, checking qualities");
            Log.Debug("Found {Count} valid items in tab {TabName}, checking qualities", items.Count(), tab.Name);
            return base.CheckRecipe(items, tab, DefaultMaxItems, DefaultTargetQuality);
        }
    }
}
