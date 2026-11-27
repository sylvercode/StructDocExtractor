namespace Sylvercode.SiteExtractor.Store;

/// <summary>Represents a resolved datastore destination and its portable relative path.</summary>
/// <param name="Uri">The normalized absolute URI resolved below the datastore root.</param>
/// <param name="PhysicalPath">The full platform-specific filesystem path.</param>
/// <param name="RelativePath">The normalized portable path relative to the datastore root.</param>
public sealed record DataStorePath(
    Uri Uri,
    string PhysicalPath,
    string RelativePath);
