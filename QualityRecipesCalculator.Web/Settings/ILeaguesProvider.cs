using System.Collections.Generic;

namespace TehGM.PoE.QualityRecipesCalculator.Settings
{
    public interface ILeaguesProvider
    {
        IEnumerable<string> GetLeaguesList();
    }
}
