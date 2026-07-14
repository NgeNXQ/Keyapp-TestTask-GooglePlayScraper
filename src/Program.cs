using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CommandLine;
using Keyapp.TestTask.GooglePlayScraper.Core.Presentation.Schemas;
using Keyapp.TestTask.GooglePlayScraper.Core.Presentation.Controllers;
using Keyapp.TestTask.GooglePlayScraper.Core.Orchestration.Services;
using Keyapp.TestTask.GooglePlayScraper.Core.Orchestration.Interfaces;
using Keyapp.TestTask.GooglePlayScraper.Core.Infrastructure.GooglePlay.Clients;
using Keyapp.TestTask.GooglePlayScraper.Core.Infrastructure.GooglePlay.Parsers;
using Keyapp.TestTask.GooglePlayScraper.Core.Infrastructure.GooglePlay.Options;
using Keyapp.TestTask.GooglePlayScraper.Core.Infrastructure.GooglePlay.Gateways;
using Keyapp.TestTask.GooglePlayScraper.Shared;

namespace Keyapp.TestTask.GooglePlayScraper;

file sealed class Program {

    private static async Task<int> Main(string[] args) {
        var builder = CliApplication.CreateBuilder(new() {
            ContentRootPath = Path.Combine(AppContext.BaseDirectory, "etc")
        });

        builder.Configuration.AddJsonFile("config/settings.json");

        builder.Services.Configure<GooglePlayScraperOptions>(
            builder.Configuration.GetSection("GooglePlay:Scraper")
        );

        builder.Services.AddSingleton<CliController>();

        builder.Services.AddSingleton<
            IGooglePlayPackagesAggregatorService,
            GooglePlayPackagesAggregatorService
        >();

        builder.Services.AddSingleton<
            IGooglePlayPackagesScraperGateway,
            GooglePlayPackagesScraperGateway
        >();

        builder.Services.AddSingleton<
            IPaginatedRpcProtobufPayloadParser,
            PaginatedRpcProtobufPayloadParser
        >();

        builder.Services.AddSingleton<
            IGeneralRpcProtobufPayloadParser,
            GeneralRpcProtobufPayloadParser
        >();

        builder.Services.AddSingleton<
            IGooglePlayClient,
            GooglePlayClient
        >();

        builder.Services.AddHttpClient();

        builder.Services.AddSingleton<IFileProvider>(service => {
            var env = service.GetRequiredService<IHostEnvironment>();
            return new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "blueprints"));
        });

        var parsedOptions = Parser.Default.ParseArguments<ScraperRequest>(args);

        if (parsedOptions.Errors.Any())
            return 1;

        builder.Services.AddSingleton(parsedOptions.Value);

        var app = builder.Build();

        await app.RunAsync();

        return 0;
    }
}
