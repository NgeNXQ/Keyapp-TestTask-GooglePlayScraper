using System;

namespace Keyapp.TestTask.GooglePlayScraper.Core.Infrastructure.GooglePlay.Exceptions;

internal sealed class GooglePlayParsingException : GooglePlayScraperException {
    public GooglePlayParsingException(string context, Exception innerException)
        : base($"Failed to parse Google Play Services response: {context}", innerException) { }
}
