using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using Serilog;
using Serilog.Events;

namespace TehGM.PoE.QualityRecipesCalculator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<Options>(args).WithParsedAsync(async (options) =>
            {
                // initialize log
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .MinimumLevel.Is(options.Debug ? LogEventLevel.Debug : LogEventLevel.Information)
                    .CreateLogger();

                Log.Information(HeadingInfo.Default);
                Log.Information(CopyrightInfo.Default);

                // output args info
                Log.Information("Account name: {AccountName}", options.AccountName);
                Log.Information("League: {League}", options.League);
                Log.Information("Realm: {Realm}", options.Realm);

                // download all stash data
                using PoeHttpClient client = new PoeHttpClient(options.SessionID, options.AccountName);
                client.Realm = options.Realm;
                IEnumerable<StashTab> tabs = await client.GetStashTabsAsync(options.League).ConfigureAwait(false);

                // calculate and output results
                RecipesCalculator calculator = new RecipesCalculator(tabs, options);
                calculator.CheckGlassblowersBaubleRecipe();
                calculator.CheckGemcuttersPrismRecipe();
            });
            Log.Information("Done");
            Console.ReadLine();
        }

    }
}
