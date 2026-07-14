using System.Threading;
using System.Collections.Generic;
using Keyapp.TestTask.GooglePlayScraper.Core.Orchestration.Models;

namespace Keyapp.TestTask.GooglePlayScraper.Core.Orchestration.Interfaces;

internal interface IGooglePlayPackagesScraperGateway {
    IAsyncEnumerable<string> FetchAllPackagesAsync(
        ScrapingParams parameters,
        CancellationToken token
    );
}
