using CommandLine;

namespace Keyapp.TestTask.GooglePlayScraper.Core.Presentation.Schemas;

internal sealed class ScraperRequest {

    [Option('k', "keyword", Required = true, HelpText = "Search keyword.")]
    public string Keyword { get; set; } = string.Empty;

    [Option('c', "country", Required = true, HelpText = "2-letter country code, e.g. us.")]
    public string Country { get; set; } = string.Empty;

    [Option('l', "language", Required = true, HelpText = "Language code, e.g. en-US.")]
    public string Language { get; set; } = string.Empty;
}
