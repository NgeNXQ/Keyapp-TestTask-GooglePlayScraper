using System.Threading;
using System.Threading.Tasks;
using Keyapp.TestTask.GooglePlayScraper.Core.Orchestration.Models;

namespace Keyapp.TestTask.GooglePlayScraper.Core.Orchestration.Interfaces;

internal interface IGooglePlayPackagesAggregatorService {
    Task ScrapePackagesAsync(ScrapingParams parameters, CancellationToken token);
}
