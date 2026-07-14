using System.Text.Json;
using System.Text.Json.Nodes;

namespace Keyapp.TestTask.GooglePlayScraper.Core.Infrastructure.GooglePlay.Parsers;

internal sealed class PaginatedRpcProtobufPayloadParser : IPaginatedRpcProtobufPayloadParser {

    public string? Parse(string input) {
        JsonNode? root;

        try {
            root = JsonNode.Parse(input);
        }
        catch (JsonException) {
            return null;
        }

        return _FindPaginationToken(root);
    }

    private static string? _FindPaginationToken(JsonNode? node) {
        if (node is JsonArray array) {
            if (_IsTokenShape(array, out var token))
                return token;

            foreach (var child in array) {
                var found = _FindPaginationToken(child);

                if (found is not null)
                    return found;
            }
        }
        else if (node is JsonObject obj) {
            foreach (var (_, child) in obj) {
                var found = _FindPaginationToken(child);

                if (found is not null)
                    return found;
            }
        }

        return null;
    }

    private static bool _IsTokenShape(JsonArray array, out string? token) {
        token = null;

        if (array.Count != 5)
            return false;

        if (array[0] is not JsonValue v0 || v0.GetValueKind() != JsonValueKind.String)
            return false;

        if (array[1] is not null)
            return false;

        if (array[2] is not null)
            return false;

        if (array[3] is not JsonArray innerArray || innerArray.Count != 2)
            return false;

        if (innerArray[0] is not null)
            return false;

        if (innerArray[1] is not JsonValue tokenValue || tokenValue.GetValueKind() != JsonValueKind.String)
            return false;

        if (array[4] is not JsonValue v4 ||
            (v4.GetValueKind() != JsonValueKind.True && v4.GetValueKind() != JsonValueKind.False))
                return false;

        token = tokenValue.GetValue<string>();

        return true;
    }
}
