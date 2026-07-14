using System;
using System.Linq;
using System.Text.Json;

namespace Keyapp.TestTask.GooglePlayScraper.Core.Infrastructure.GooglePlay.Parsers;

internal sealed class GeneralRpcProtobufPayloadParser : IGeneralRpcProtobufPayloadParser {

    public string? Parse(string input) {
        var stripped = input[4..].TrimStart(); // anti-XSSI prefix )]}'

        var lines = stripped.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        var json = lines.First(line => line.StartsWith('['));

        using var doc = JsonDocument.Parse(json);

        var array = doc.RootElement[0]; // wrb.fr (the first one cause only 1 rpc call is performed)
        var output = array[2].GetString();

        return output;
    }
}
