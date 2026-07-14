namespace Keyapp.TestTask.GooglePlayScraper.Core.Infrastructure.GooglePlay.Parsers;

internal interface IRpcProtobufPayloadParser {
    string? Parse(string input);
}
