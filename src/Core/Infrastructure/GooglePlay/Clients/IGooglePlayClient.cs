using System.Threading;
using System.Threading.Tasks;
using Keyapp.TestTask.GooglePlayScraper.Core.Infrastructure.GooglePlay.Payloads;

namespace Keyapp.TestTask.GooglePlayScraper.Core.Infrastructure.GooglePlay.Clients;

internal interface IGooglePlayClient {
    Task<PackagesScraperResponse> PostInitialPackagesAsync(
        string keyword,
        string country,
        string language,
        CancellationToken token
    );

    Task<PackagesScraperResponse> PostPaginatedPackagesAsync(
        string metadata,
        string country,
        string language,
        CancellationToken token
    );
}
