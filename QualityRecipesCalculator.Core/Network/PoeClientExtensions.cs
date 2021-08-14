using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TehGM.PoE.QualityRecipesCalculator
{
    public static class PoeClientExtensions
    {
        public static Task<IEnumerable<Item>> GetStashTabContentsAsync(this IPoeClient client, string league, StashTab tab, CancellationToken cancellationToken = default)
            => client.GetStashTabContentsAsync(league, tab.Index, cancellationToken);
    }
}
