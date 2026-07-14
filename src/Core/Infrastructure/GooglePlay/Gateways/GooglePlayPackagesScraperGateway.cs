using System.Runtime.CompilerServices;
using System.Threading;
using System.Collections.Generic;
using Keyapp.TestTask.GooglePlayScraper.Core.Orchestration.Models;
using Keyapp.TestTask.GooglePlayScraper.Core.Orchestration.Interfaces;
using Keyapp.TestTask.GooglePlayScraper.Core.Infrastructure.GooglePlay.Clients;

namespace Keyapp.TestTask.GooglePlayScraper.Core.Infrastructure.GooglePlay.Gateways;

internal sealed class GooglePlayPackagesScraperGateway : IGooglePlayPackagesScraperGateway {

    private readonly IGooglePlayClient _googlePlayClient;

    public GooglePlayPackagesScraperGateway(
        IGooglePlayClient googlePlayClient
    ) {
        _googlePlayClient = googlePlayClient;
    }

    public async IAsyncEnumerable<string> FetchAllPackagesAsync(
        ScrapingParams parameters,
        [EnumeratorCancellation] CancellationToken token
    ) {
        var current = await _googlePlayClient.PostInitialPackagesAsync(
            parameters.Keyword,
            parameters.Country,
            parameters.Language,
            token
        );

        foreach (var pkg in current.Packages)
            yield return pkg;

        while (!string.IsNullOrEmpty(current.PaginationMetadata)) {
            token.ThrowIfCancellationRequested();

            current = await _googlePlayClient.PostPaginatedPackagesAsync(
                current.PaginationMetadata,
                parameters.Country,
                parameters.Language,
                token
            );

            foreach (var pkg in current.Packages)
                yield return pkg;
        }
    }
}
