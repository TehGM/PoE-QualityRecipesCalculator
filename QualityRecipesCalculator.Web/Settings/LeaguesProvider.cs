using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TehGM.PoE.QualityRecipesCalculator.Settings.Services
{
    public class LeaguesProvider : ILeaguesProvider
    {
        private IEnumerable<string> _builtList = null;

        private readonly LeaguesOptions _leagues;
        private readonly ILogger _log;

        public LeaguesProvider(IOptions<LeaguesOptions> leaguesOptions, ILogger<LeaguesProvider> log)
        {
            this._leagues = leaguesOptions.Value;
            this._log = log;
        }

        public IEnumerable<string> GetLeaguesList()
        {
            if (this._builtList == null)
            {
                this._log?.LogTrace("Building leagues list");

                // booleans for easy access
                bool addHC = this._leagues.SupportHardcore;
                bool addSSF = this._leagues.SupportSSF;
                bool addSSFHC = addHC && addSSF;

                // build list with estimated capacity
                // +1 cause standard
                int rawCount = this._leagues.ListedLeagues.Count() + 1;
                int estimateCount = rawCount;
                if (addHC)
                    estimateCount += rawCount;
                if (addSSF)
                    estimateCount += rawCount;
                if (addSSFHC)
                    estimateCount += rawCount;
                List<string> leagues = new List<string>(estimateCount);

                // add standard
                this._log?.LogTrace("Adding league {LeagueName}", "Standard");
                if (addSSFHC)
                    leagues.Add("SSF Hardcore");
                if (addHC)
                    leagues.Add("Hardcore");
                if (addSSF)
                    leagues.Add("SSF Standard");
                leagues.Add("Standard");

                // add each league
                foreach (string leagueName in this._leagues.ListedLeagues)
                {
                    this._log?.LogTrace("Adding league {LeagueName}", leagueName);
                    if (addSSFHC)
                        leagues.Add($"SSF {leagueName} HC");
                    if (addHC)
                        leagues.Add($"Hardcore {leagueName}");
                    if (addSSF)
                        leagues.Add($"SSF {leagueName}");
                    leagues.Add(leagueName);
                }

                // revert it to make latest league appear on top
                leagues.Reverse();

                // cache results
                this._builtList = leagues;
                this._log?.LogDebug("Leagues list built. {LeagueCount} leagues in {LeagueVariant} variants cached", this._leagues.ListedLeagues.Count(), leagues.Count);
            }

            return this._builtList;
        }
    }
}
