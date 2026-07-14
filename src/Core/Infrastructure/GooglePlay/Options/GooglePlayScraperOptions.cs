namespace Keyapp.TestTask.GooglePlayScraper.Core.Infrastructure.GooglePlay.Options;

public sealed class GooglePlayScraperOptions {
    public string BuildLabel { get; init; } = null!;
    public int HttpTimeoutSeconds { get; init; }
    public string SpoofedUserAgent { get; init; } = null!;
    public string PlayStoreServiceUrl { get; init; } = null!;
    public string BatchExecuteEndpoint { get; init; } = null!;
    public string AppsLookupInitialRpcId { get; init; } = null!;
    public string AppsLookupPaginatedRpcId { get; init; } = null!;
    public string BlueprintBatchexecuteInitialPayload { get; init; } = null!;
    public string BlueprintBatchexecutePaginatedPayload { get; init; } = null!;
}
