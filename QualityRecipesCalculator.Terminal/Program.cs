using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using TehGM.ConsoleProgressBar;
using TehGM.PoE.QualityRecipesCalculator.Network;

namespace TehGM.PoE.QualityRecipesCalculator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            await Parser.Default.ParseArguments<TerminalOptions>(args).WithParsedAsync(async (options) =>
            {
                Console.Title = $"{HeadingInfo.Default} - {options.AccountName}, {options.League} league";

                // initialize log
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .MinimumLevel.Is(options.Debug ? 
                        (Debugger.IsAttached ? LogEventLevel.Verbose : LogEventLevel.Debug) 
                        : LogEventLevel.Information)
                    .CreateLogger();
                AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
                ILoggerFactory logFactory = new LoggerFactory().AddSerilog(Log.Logger);

                Log.Information(HeadingInfo.Default);
                Log.Information(CopyrightInfo.Default);

                // output args info
                Log.Information("Account name: {AccountName}", options.AccountName);
                Log.Information("League: {League}", options.League);
                Log.Information("Realm: {Realm}", options.Realm);

                IEnumerable<StashTab> tabs;
                try
                {
                    // download all stash data
                    PoeHttpClientOptions clientOptions = new PoeHttpClientOptions()
                    {
                        AccountName = options.AccountName,
                        UserAgent = $"{PoeHttpClientOptions.DefaultUserAgent} - Terminal Version, v{GetVersion()}",
                        Realm = options.Realm,
                        SessionID = options.SessionID
                    };
                    using PoeHttpClient client = new PoeHttpClient(clientOptions, logFactory.CreateLogger<PoeHttpClient>());

                    stopwatch.Restart();
                    ProgressBar progressBar = new ProgressBar();
                    progressBar.Start();
                    EventHandler<ProcessStatus> progressCallback = (sender, args) => progressBar.Update(args.CurrentProgress, args.MaxProgress, args.MainText);
                    client.StatusUpdated += progressCallback;
                    tabs = await client.GetStashTabsAsync(options.League).ConfigureAwait(false);
                    client.StatusUpdated -= progressCallback;
                    Log.Debug("Done downloading ({Time} ms)", stopwatch.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    if (options.Debug)
                        Log.Fatal(ex, "Failed downloading stash data");
                    else
                        Log.Fatal("Failed downloading stash data: {Message}", ex.Message);
                    return;
                }

                // calculate and output results
                try
                {
                    TerminalRecipesCalculator calculator = new TerminalRecipesCalculator(tabs, options, logFactory);

                    stopwatch.Restart();
                    calculator.CheckGlassblowersBaubleRecipe();
                    Log.Debug("Done checking Glassblower's Bauble Recipe ({Time} ms)", stopwatch.ElapsedMilliseconds);

                    stopwatch.Restart();
                    calculator.CheckGemcuttersPrismRecipe();
                    Log.Debug("Done checking Gemcutter's Prism Recipe ({Time} ms)", stopwatch.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    if (!options.Debug)
                        Log.Fatal("Failed calculating vendor recipes: {Message}", ex.Message);
                    else
                        Log.Fatal(ex, "Failed calculating vendor recipes");
                    return;
                }
            });
            if (Debugger.IsAttached)
            {
                Log.Information("Done. Press enter to exit...");
                Console.ReadLine();
            }
            Log.CloseAndFlush();
        }

        private static string GetVersion()
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(typeof(Program).Assembly.Location);
            if (!string.IsNullOrWhiteSpace(versionInfo.ProductVersion))
                return versionInfo.ProductVersion;
            string result = $"{versionInfo.FileMajorPart}.{versionInfo.FileMinorPart}.{versionInfo.FileBuildPart}";
            if (versionInfo.FilePrivatePart != 0)
                result += $".{versionInfo.FilePrivatePart}";
            return result;
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Log.Logger.Fatal((Exception)e.ExceptionObject, "An exception was unhandled");
                Log.CloseAndFlush();
            }
            catch { }
        }
    }
}
