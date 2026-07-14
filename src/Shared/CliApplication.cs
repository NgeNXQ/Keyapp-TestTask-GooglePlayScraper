using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Keyapp.TestTask.GooglePlayScraper.Shared;

public static class CliApplication {

    public static HostApplicationBuilder CreateBuilder(HostApplicationBuilderSettings settings) {
        var builder = Host.CreateEmptyApplicationBuilder(settings);
        builder.Services.AddHostedService<CliBackgroundService>();
        return builder;
    }
}
