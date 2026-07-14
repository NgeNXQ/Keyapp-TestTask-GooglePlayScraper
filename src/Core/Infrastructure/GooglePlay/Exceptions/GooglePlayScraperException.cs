using System;

namespace Keyapp.TestTask.GooglePlayScraper.Core.Infrastructure.GooglePlay.Exceptions;

internal abstract class GooglePlayScraperException : Exception {
    protected GooglePlayScraperException(string message, Exception? innerException = null)
        : base(message, innerException) { }
}
