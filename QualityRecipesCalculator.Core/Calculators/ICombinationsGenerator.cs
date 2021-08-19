using System.Collections.Generic;

namespace TehGM.PoE.QualityRecipesCalculator.Calculators
{
    public interface ICombinationsGenerator
    {
        IEnumerable<IEnumerable<T>> GenerateCombinations<T>(IEnumerable<T> sequence, int maxItems);
    }
}
