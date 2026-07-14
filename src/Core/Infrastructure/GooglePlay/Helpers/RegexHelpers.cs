using System.Text.RegularExpressions;

namespace Keyapp.TestTask.GooglePlayScraper.Core.Infrastructure.GooglePlay.Helpers;

internal static partial class RegexHelpers {
    [GeneratedRegex(
        @"/store/apps/details\?id\\u003[dD]([a-zA-Z][a-zA-Z0-9_]*(?:\.[a-zA-Z0-9_]+)+)"
    )]
    internal static partial Regex PackageIdentifierPattern();
}
