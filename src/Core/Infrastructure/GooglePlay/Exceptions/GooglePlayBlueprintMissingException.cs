namespace Keyapp.TestTask.GooglePlayScraper.Core.Infrastructure.GooglePlay.Exceptions;

internal sealed class GooglePlayBlueprintMissingException  : GooglePlayScraperException {
    public GooglePlayBlueprintMissingException(string blueprintPath)
        : base($"Blueprint payload template not found: {blueprintPath}") { }
}
