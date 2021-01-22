using System.Collections.Generic;
using System.Threading.Tasks;
using CommandLine;
using Serilog;

namespace TehGM.PoeQualityPermutations
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Debug()
                .CreateLogger();

            await Parser.Default.ParseArguments<Options>(args).WithParsedAsync(async (options) =>
            {
                using PoeHttpClient client = new PoeHttpClient(options.SessionID, options.AccountName);
                client.Realm = options.Realm;
                IEnumerable<StashTab> tabs = await client.GetStashTabsAsync(options.League).ConfigureAwait(false);
            });
        }
    }
}
