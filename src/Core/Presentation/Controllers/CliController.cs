using System.Threading;
using System.Threading.Tasks;
using Keyapp.TestTask.GooglePlayScraper.Core.Presentation.Schemas;
using Keyapp.TestTask.GooglePlayScraper.Core.Orchestration.Models;
using Keyapp.TestTask.GooglePlayScraper.Core.Orchestration.Interfaces;

namespace Keyapp.TestTask.GooglePlayScraper.Core.Presentation.Controllers;

internal sealed class CliController {

    private readonly ScraperRequest _request;
    private readonly IGooglePlayPackagesAggregatorService _googlePlayPackagesAggregatorService;

    public CliController(
        ScraperRequest request,
        IGooglePlayPackagesAggregatorService googlePlayPackagesAggregatorService
    ) {
        _request = request;
        _googlePlayPackagesAggregatorService = googlePlayPackagesAggregatorService;
    }

    internal async Task WorkAsync(CancellationToken token) {
        await _googlePlayPackagesAggregatorService.ScrapePackagesAsync(
            new ScrapingParams(_request.Keyword, _request.Country, _request.Language),
            token
        );
    }
}
