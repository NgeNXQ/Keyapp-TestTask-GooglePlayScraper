using System;
using System.Threading;
using System.Threading.Tasks;
using Keyapp.TestTask.GooglePlayScraper.Core.Orchestration.Models;
using Keyapp.TestTask.GooglePlayScraper.Core.Orchestration.Interfaces;

namespace Keyapp.TestTask.GooglePlayScraper.Core.Orchestration.Services;

internal sealed class GooglePlayPackagesAggregatorService : IGooglePlayPackagesAggregatorService {

    private readonly IGooglePlayPackagesScraperGateway _googlePlayPackagesScraperGateway;

    public GooglePlayPackagesAggregatorService(
        IGooglePlayPackagesScraperGateway googlePlayPackagesScraperGateway
    ) {
        _googlePlayPackagesScraperGateway = googlePlayPackagesScraperGateway;
    }

    public async Task ScrapePackagesAsync(ScrapingParams parameters, CancellationToken token) {
        await foreach (var package in _googlePlayPackagesScraperGateway.FetchAllPackagesAsync(
            parameters,
            token
        )) {
            Console.WriteLine(package);
        }
    }
}
