using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TehGM.PoE.QualityRecipesCalculator
{
    public interface IPoeClient
    {
        Task<IEnumerable<StashTab>> GetStashTabsAsync(string league, CancellationToken cancellationToken = default);
        Task<IEnumerable<Item>> GetStashTabContentsAsync(string league, int tabIndex, CancellationToken cancellationToken = default);

        ProcessStatus Status { get; }
        event EventHandler<ProcessStatus> StatusUpdated;
    }
}
