using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using TehGM.PoE.QualityRecipesCalculator.Settings;
using TehGM.PoE.QualityRecipesCalculator.Settings.Services;

namespace TehGM.PoE.QualityRecipesCalculator
{
    public class Program
    {
        public const string ProgramName = "TehGM's PoE Tools";

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<UI.App>("#app");
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration, "Logging")
                .CreateLogger();
            Serilog.Debugging.SelfLog.Enable(m => Console.Error.WriteLine(m));
            builder.Logging.AddSerilog(Log.Logger, true);

            builder.Services.Configure<LeaguesOptions>(builder.Configuration.GetSection("Leagues"));

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<IUserSettingsProvider, UserSettingsProvider>();
            builder.Services.AddSingleton<ILeaguesProvider, LeaguesProvider>();

            await builder.Build().RunAsync();
        }
    }
}
