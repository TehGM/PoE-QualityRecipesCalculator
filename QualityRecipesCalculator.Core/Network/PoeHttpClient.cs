using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TehGM.PoE.Serialization;
using System;
using Microsoft.Extensions.Logging;

namespace TehGM.PoE.QualityRecipesCalculator.Network
{
    public class PoeHttpClient : HttpClient, IPoeClient
    {
        public ProcessStatus Status { get; }
        public event EventHandler<ProcessStatus> StatusUpdated;

        public string AccountName { get; }
        public string Realm { get; }

        private readonly ILogger _log;

        public PoeHttpClient(PoeHttpClientOptions options)
            : this(options, null) { }

        public PoeHttpClient(PoeHttpClientOptions options, ILogger<PoeHttpClient> log)
            : base(new HttpClientHandler() { UseCookies = false })
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (options.AccountName == null)
                throw new ArgumentNullException(nameof(options.AccountName));
            if (options.Realm == null)
                throw new ArgumentNullException(nameof(options.Realm));
            if (options.SessionID == null)
                throw new ArgumentNullException(nameof(options.SessionID));
            if (options.UserAgent == null)
                throw new ArgumentNullException(nameof(options.UserAgent));

            base.DefaultRequestHeaders.Add("Cookie", $"POESESSID={options.SessionID}");
            base.DefaultRequestHeaders.Add("User-Agent", options.UserAgent);
            this.AccountName = options.AccountName;
            this.Realm = options.Realm;
            this.Status = new ProcessStatus(null);
            this._log = log;
        }

        public async Task<IEnumerable<StashTab>> GetStashTabsAsync(string league, CancellationToken cancellationToken = default)
        {
            this.Status.MainText = "Downloading stash data...";
            this.UpdateProgress(0, 100);

            // prepare request
            this._log?.LogDebug("Requesting stash tabs info for account {Account} in league {League}", AccountName, league);
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
            this._log?.LogTrace("Parsing stash info");
            JObject data = JObject.Parse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            StashTab[] tabs = data["tabs"].ToObject<StashTab[]>(SerializationHelper.DefaultSerializer);
            for (int i = 0; i < tabs.Length; i++)
            {
                this.UpdateProgress(i, tabs.Length);
                StashTab tab = tabs[i];
                JToken items = await GetStashTabContentsInternalAsync(league, tab.Index, cancellationToken).ConfigureAwait(false);
                items.PopulateObject(tab);
            }
            this.Status.MainText = "Stash data download complete.";
            this.UpdateProgress(tabs.Length, tabs.Length);
            this._log?.LogTrace("Done parsing stash info");
            return tabs;
        }

        public async Task<IEnumerable<Item>> GetStashTabContentsAsync(string league, int index, CancellationToken cancellationToken = default)
            => (await GetStashTabContentsInternalAsync(league, index, cancellationToken).ConfigureAwait(false))["data"].ToObject<Item[]>();

        private async Task<JToken> GetStashTabContentsInternalAsync(string league, int index, CancellationToken cancellationToken = default)
        {
            // prepare request
            this._log?.LogDebug("Requesting stash tab {Index} for account {Account} in league {League}", index, AccountName, league);
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
            this._log?.LogTrace("Parsing stash tab items");
            JObject data = JObject.Parse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            this._log?.LogTrace("Done parsing stash tab items");
            return data;
        }

        private void UpdateProgress(int current, int max)
        {
            this.Status.MaxProgress = max;
            this.Status.CurrentProgress = current;

            this.StatusUpdated?.Invoke(this, this.Status);
        }
    }
}
