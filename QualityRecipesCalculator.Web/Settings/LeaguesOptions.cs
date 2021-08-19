using System.Collections.Generic;

namespace TehGM.PoE.QualityRecipesCalculator.Settings
{
    public class LeaguesOptions
    {
        public IEnumerable<string> ListedLeagues { get; set; }
        public bool SupportSSF { get; set; } = true;
        public bool SupportHardcore { get; set; } = true;
    }
}
