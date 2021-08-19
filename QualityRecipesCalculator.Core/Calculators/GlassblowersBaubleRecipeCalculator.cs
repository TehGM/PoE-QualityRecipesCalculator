using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace TehGM.PoE.QualityRecipesCalculator.Calculators
{
    public class GlassblowersBaubleRecipeCalculator : RecipeCalculatorBase, IRecipeCalculator
    {
        public GlassblowersBaubleRecipeCalculator(ICombinationsGenerator combinationsGenerator, ILogger<GlassblowersBaubleRecipeCalculator> log)
            : base(combinationsGenerator, log) { }

        public GlassblowersBaubleRecipeCalculator()
            : this(new CombinationsGenerator(), null) { }

        public override CalculationsResult Calculate(StashTab tab)
        {
            base.Status = new ProcessStatus("Checking for Glassblower's Bauble recipe (Flasks)");
            base.UpdateSubStatus($"Checking tab {tab.Name}");

            // flasks can have frame type 0, 1 or 3. They also always have "Consumes {0} of {1} Charges on use" as one of properties
            // also we're only interested in flasks with quality
            IEnumerable<Item> items = tab.Items.Where(i => (i.FrameType == 0 || i.FrameType == 1 || i.FrameType == 3)
                && i.TryGetProperty("Consumes {0} of {1} Charges on use", out _)
                && i.TryGetProperty("Quality", out _));
            base.RaiseItemsFound(items);

            if (!items.Any())
                return new CalculationsResult(tab, DefaultTargetQuality);
            base.UpdateSubStatus($"Found {items.Count()} valid items in tab {tab.Name}, checking qualities");
            base.Log?.LogDebug("Found {Count} valid items in tab {TabName}, checking qualities", items.Count(), tab.Name);
            return base.CheckRecipe(items, tab, 30, DefaultTargetQuality);
        }
    }
}
