# Keyapp.TestTask.GooglePlayScrapper

Console app that scrapes Google Play app package names for a given keyword and
country, by calling Play Store's internal batchexecute RPC endpoint directly.

## Usage

```bash
dotnet run -- -k poker -c us -l en-US
```

| Flag | Description |
|---|---|
| `-k`, `--keyword` | search keyword |
| `-c`, `--country` | country code (`gl`) |
| `-l`, `--language` | language code (`hl`) |

Package names are printed to stdout, one per line, in server order.

## What it does

- Reproduces the real batchexecute request (headers, query params, form-encoded
  `f.req` body) using pre-captured JSON body templates with placeholders for
  keyword, RPC id and pagination token.
- Strips the anti-XSSI prefix and unwraps the nested JSON response format.
- Extracts package names via regex and dedupes them while preserving order.
- Follows the continuation token returned by the server and keeps requesting
  subsequent pages (via a separate RPC id) until no token is left, streaming
  results page by page instead of loading everything at once.

## Architecture

```
Presentation
  CliController                        # entry point logic, reads parsed CLI args, kicks off the scrape
  ScraperRequest                       # CLI argument schema (keyword, country, language)

Orchestration
  GooglePlayPackagesAggregatorService  # drives the scrape loop, prints results as they arrive
  ScrapingParams                       # holds keyword, country, language for a single run

Infrastructure
  GooglePlayClient                     # builds and sends the batchexecute request, loads body templates
  GooglePlayPackagesScraperGateway     # pages through results, calling the client until no token is left
  GeneralRpcProtobufPayloadParser      # strips anti-XSSI prefix and unwraps the response envelope
  PaginatedRpcProtobufPayloadParser    # finds the pagination token inside the response
  RegexHelpers                         # extracts package names from the raw payload
  GooglePlayScraperOptions             # config values (rpc ids, urls, timeouts)
  PackagesScraperResponse              # result of one request (packages and pagination token)
  Exceptions                           # GooglePlayTransportException, GooglePlayParsingException, GooglePlayBlueprintMissingException

Config
  etc/blueprints                       # request body templates
  etc/config                           # rpc ids, urls, timeouts
```