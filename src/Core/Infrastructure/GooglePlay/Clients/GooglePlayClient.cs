using System;
using System.IO;
using System.Web;
using System.Net.Http;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.FileProviders;
using Keyapp.TestTask.GooglePlayScraper.Core.Infrastructure.GooglePlay.Helpers;
using Keyapp.TestTask.GooglePlayScraper.Core.Infrastructure.GooglePlay.Parsers;
using Keyapp.TestTask.GooglePlayScraper.Core.Infrastructure.GooglePlay.Options;
using Keyapp.TestTask.GooglePlayScraper.Core.Infrastructure.GooglePlay.Payloads;
using Keyapp.TestTask.GooglePlayScraper.Core.Infrastructure.GooglePlay.Exceptions;

namespace Keyapp.TestTask.GooglePlayScraper.Core.Infrastructure.GooglePlay.Clients;

internal sealed class GooglePlayClient : IGooglePlayClient {

    private readonly HttpClient _httpClient;
    private readonly IFileProvider _blueprintsProvider;
    private readonly GooglePlayScraperOptions _scraperOptions;
    private readonly IGeneralRpcProtobufPayloadParser _generalPayloadParser;
    private readonly IPaginatedRpcProtobufPayloadParser _paginatedPayloadParser;

    private string _cachedPaginatedPayloadTemplate = null!;

    public GooglePlayClient(
        HttpClient httpClient,
        IFileProvider blueprintsProvider,
        IOptions<GooglePlayScraperOptions> scraperOptions,
        IGeneralRpcProtobufPayloadParser generalPayloadParser,
        IPaginatedRpcProtobufPayloadParser paginatedPayloadParser
    ) {
        _httpClient = httpClient;
        _scraperOptions = scraperOptions.Value;
        _blueprintsProvider = blueprintsProvider;
        _generalPayloadParser = generalPayloadParser;
        _paginatedPayloadParser = paginatedPayloadParser;

        _httpClient.Timeout = TimeSpan.FromSeconds(_scraperOptions.HttpTimeoutSeconds);
    }

    public async Task<PackagesScraperResponse> PostInitialPackagesAsync(
        string keyword,
        string country,
        string language,
        CancellationToken token
    ) {
        var requestBody = await _CreateInitialRequestBodyPayloadAsync(keyword);

        var request = _CreatePostPackagesRequest(
            _scraperOptions.AppsLookupInitialRpcId,
            country,
            language,
            requestBody
        );

        var response = await SendRequestAsync(request, token);
        var result = ParseResponse(response);

        return result;
    }

    public async Task<PackagesScraperResponse> PostPaginatedPackagesAsync(
        string metadata,
        string country,
        string language,
        CancellationToken token
    ) {
        var requestBody = await _CreatePaginatedRequestPayloadAsync(metadata);

        var request = _CreatePostPackagesRequest(
            _scraperOptions.AppsLookupPaginatedRpcId,
            country,
            language,
            requestBody
        );

        var response = await SendRequestAsync(request, token);
        var result = ParseResponse(response);

        return result;
    }

    private async Task<string> _CreateInitialRequestBodyPayloadAsync(string keyword) {
        var initialPayloadTemplate = await _LoadTemplatePayloadAsync(
            _scraperOptions.BlueprintBatchexecuteInitialPayload
        );

        var payload = initialPayloadTemplate
            .Replace("<rpc_id>", _scraperOptions.AppsLookupInitialRpcId)
            .Replace("<keyword>", JsonEncodedText.Encode(keyword).ToString());

        return payload;
    }

    private async ValueTask<string> _CreatePaginatedRequestPayloadAsync(string metadata) {
        if (_cachedPaginatedPayloadTemplate is null) {
            _cachedPaginatedPayloadTemplate = await _LoadTemplatePayloadAsync(
                _scraperOptions.BlueprintBatchexecutePaginatedPayload
            );
        }

        var payload = _cachedPaginatedPayloadTemplate
            .Replace("<rpc_id>", _scraperOptions.AppsLookupPaginatedRpcId)
            .Replace("<metadata>", JsonEncodedText.Encode(metadata).ToString());

        return payload;
    }

    private async Task<string> _LoadTemplatePayloadAsync(string blueprintPayload) {
        var payloadFile = _blueprintsProvider.GetFileInfo(blueprintPayload);

        if (!payloadFile.Exists)
            throw new GooglePlayBlueprintMissingException(blueprintPayload);

        using var reader = new StreamReader(payloadFile.CreateReadStream());
        var payloadTemplate = await reader.ReadToEndAsync();

        return payloadTemplate;
    }

    private HttpRequestMessage _CreatePostPackagesRequest(
        string rpcId,
        string country,
        string language,
        string bodyPayload
    ) {
        var uri = new Uri(
            new Uri(_scraperOptions.PlayStoreServiceUrl),
            _scraperOptions.BatchExecuteEndpoint
        );

        var queryParams = new Dictionary<string, string> {
            ["rpcids"] = rpcId,
            ["bl"] = _scraperOptions.BuildLabel,
            ["hl"] = language,
            ["gl"] = country,
            ["rt"] = "c"
        };

        var query = HttpUtility.ParseQueryString(string.Empty);

        foreach (var (key, value) in queryParams)
            query[key] = value;

        var url = new UriBuilder(uri) { Query = query.ToString() }.Uri;

        var request = new HttpRequestMessage(HttpMethod.Post, url) {
            Content = new FormUrlEncodedContent(new Dictionary<string, string> {
                ["f.req"] = bodyPayload
            })
        };

        request.Headers.Add("Host", "play.google.com");
        request.Headers.Add("Origin", "https://play.google.com");
        request.Headers.Add("Referer", "https://play.google.com/");
        request.Headers.Add("X-Same-Domain", "1");
        request.Headers.Add("User-Agent", _scraperOptions.SpoofedUserAgent);

        return request;
    }

    private async Task<string> SendRequestAsync(
        HttpRequestMessage request,
        CancellationToken token
    ) {
        try {
            var response = await _httpClient.SendAsync(request, token);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(token);

            return content;
        }
        catch (HttpRequestException ex) {
            throw new GooglePlayTransportException(request, ex);
        }
        catch (TaskCanceledException ex) when (!token.IsCancellationRequested) {
            throw new GooglePlayTransportException(request, ex);
        }
    }

    private PackagesScraperResponse ParseResponse(string payload) {
        try {
            var json = _generalPayloadParser.Parse(payload);

            var matches = RegexHelpers.PackageIdentifierPattern().Matches(json!);
            var results = matches.Select(match => match.Groups[1].Value).Distinct();

            var paginationMetadata = _paginatedPayloadParser.Parse(json!);

            return new PackagesScraperResponse(results, paginationMetadata);
        }
        catch (Exception ex) {
            throw new GooglePlayParsingException(payload, ex);
        }
    }
}
