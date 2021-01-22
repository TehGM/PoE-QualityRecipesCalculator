using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Serilog;
using TehGM.PoeQualityPermutations.Serialization;
using TehGM.ConsoleProgressBar;

namespace TehGM.PoeQualityPermutations
{
    class PoeHttpClient : HttpClient
    {
        public string AccountName { get; set; }
        public string Realm { get; set; } = "pc";

        public PoeHttpClient(string sessionID, string accountName, string userAgent = "TehGM's Vendor Recipe Helper")
            : base(new HttpClientHandler() { UseCookies = false } )
        {
            DefaultRequestHeaders.Add("Cookie", $"POESESSID={sessionID}");
            DefaultRequestHeaders.Add("User-Agent", userAgent);
            AccountName = accountName;
        }

        public async Task<IEnumerable<StashTab>> GetStashTabsAsync(string league, CancellationToken cancellationToken = default)
        {
            ProgressBar progress = new ProgressBar("Downloading stash data...");
            progress.Start();
            progress.Update(0);

            // prepare request
            Log.Debug("Requesting stash tabs info for account {Account} in league {League}", AccountName, league);
            Dictionary<string, object> query = new Dictionary<string, object>(5)
            {
                { "accountName", this.AccountName },
                { "league", league },
                { "realm", this.Realm },
                { "tabs", 1 }
            };
            string url = $"https://www.pathofexile.com/character-window/get-stash-items?" +
                string.Join('&', query.Select(pair => $"{pair.Key}={pair.Value}"));
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            // send
            using HttpResponseMessage response = await SendAsync(request, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            // read content
            Log.Verbose("Parsing stash info");
            JObject data = JObject.Parse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            StashTab[] tabs = data["tabs"].ToObject<StashTab[]>(SerializationHelper.DefaultSerializer);
            for (int i = 0; i < tabs.Length; i++)
            {
                progress.Update(i, tabs.Length);
                StashTab tab = tabs[i];
                JToken items = await GetStashTabContentsInternalAsync(league, tab.Index, cancellationToken).ConfigureAwait(false);
                items.PopulateObject(tab);
            }
            progress.Update(tabs.Length, tabs.Length, "Stash data download complete.");
            Log.Verbose("Done parsing stash info");
            return tabs;
        }

        public async Task<IEnumerable<Item>> GetStashTabContentsAsync(string league, int index, CancellationToken cancellationToken = default)
            => (await GetStashTabContentsInternalAsync(league, index, cancellationToken).ConfigureAwait(false))["data"].ToObject<Item[]>();

        private async Task<JToken> GetStashTabContentsInternalAsync(string league, int index, CancellationToken cancellationToken = default)
        {
            // prepare request
            Log.Debug("Requesting stash tab {Index} for account {Account} in league {League}", index, AccountName, league);
            Dictionary<string, object> query = new Dictionary<string, object>(5)
            {
                { "accountName", this.AccountName },
                { "league", league },
                { "realm", this.Realm },
                { "tabs", 0 },
                { "tabIndex", index }
            };
            string url = $"https://www.pathofexile.com/character-window/get-stash-items?" +
                string.Join('&', query.Select(pair => $"{pair.Key}={pair.Value}"));
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            // send
            using HttpResponseMessage response = await SendAsync(request, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            // read content
            Log.Verbose("Parsing stash tab items");
            JObject data = JObject.Parse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            Log.Verbose("Done parsing stash tab items");
            return data;
        }
    }
}
