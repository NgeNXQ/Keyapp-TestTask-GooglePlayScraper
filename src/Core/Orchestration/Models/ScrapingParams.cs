namespace Keyapp.TestTask.GooglePlayScraper.Core.Orchestration.Models;

internal record class ScrapingParams(
    string Keyword,
    string Country,
    string Language
);
