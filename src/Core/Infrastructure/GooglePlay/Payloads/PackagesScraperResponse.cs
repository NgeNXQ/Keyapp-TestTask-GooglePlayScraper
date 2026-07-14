using System.Collections.Generic;

namespace Keyapp.TestTask.GooglePlayScraper.Core.Infrastructure.GooglePlay.Payloads;

internal record class PackagesScraperResponse(
    IEnumerable<string> Packages,
    string? PaginationMetadata
);
