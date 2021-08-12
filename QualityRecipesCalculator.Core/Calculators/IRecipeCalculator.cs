using System;
using System.Collections.Generic;

namespace TehGM.PoE.QualityRecipesCalculator.Calculators
{
    public interface IRecipeCalculator
    {
        ProcessStatus Status { get; }

        event EventHandler<ProcessStatus> StatusUpdated;
        event EventHandler<IEnumerable<Item>> ItemsFound;
        event EventHandler<IEnumerable<IEnumerable<KeyValuePair<Item, int>>>> PermutationsFound;

        CalculationsResult Calculate(StashTab tab);
    }
}
