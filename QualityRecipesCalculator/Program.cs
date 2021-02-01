﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                Console.Title = $"{HeadingInfo.Default} - {options.AccountName}, {options.League} league";

                // initialize log
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .MinimumLevel.Is(options.Debug ? LogEventLevel.Debug : LogEventLevel.Information)
                    .CreateLogger();
                AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

                Log.Information(HeadingInfo.Default);
                Log.Information(CopyrightInfo.Default);

                // output args info
                Log.Information("Account name: {AccountName}", options.AccountName);
                Log.Information("League: {League}", options.League);
                Log.Information("Realm: {Realm}", options.Realm);

                // download all stash data
                using PoeHttpClient client = new PoeHttpClient(options.SessionID, options.AccountName);
                client.Realm = options.Realm;
                IEnumerable<StashTab> tabs;
                try
                {
                    tabs = await client.GetStashTabsAsync(options.League).ConfigureAwait(false);
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
                    RecipesCalculator calculator = new RecipesCalculator(tabs, options);
                    calculator.CheckGlassblowersBaubleRecipe();
                    calculator.CheckGemcuttersPrismRecipe();
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
