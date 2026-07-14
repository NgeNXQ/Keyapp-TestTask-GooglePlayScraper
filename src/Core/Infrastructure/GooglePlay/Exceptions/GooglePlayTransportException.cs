using System;
using System.Net.Http;

namespace Keyapp.TestTask.GooglePlayScraper.Core.Infrastructure.GooglePlay.Exceptions;

internal sealed class GooglePlayTransportException : GooglePlayScraperException {
    public GooglePlayTransportException(HttpRequestMessage request, Exception innerException)
        : base($"Request to Google Play Services failed ({request}).", innerException) { }
}
