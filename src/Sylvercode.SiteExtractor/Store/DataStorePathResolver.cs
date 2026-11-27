namespace Sylvercode.SiteExtractor.Store;

/// <summary>Resolves output URIs consistently for datastore writes and manifest paths.</summary>
public static class DataStorePathResolver
{
    /// <summary>Resolves an output URI exactly once below a datastore base URI.</summary>
    /// <param name="baseUri">The datastore root URI.</param>
    /// <param name="uri">The output URI to resolve.</param>
    /// <returns>The resolved destination.</returns>
    /// <exception cref="ArgumentException">Thrown when the destination escapes the datastore root.</exception>
    public static DataStorePath Resolve(Uri baseUri, Uri uri)
    {
        Uri completeUri;
        if (uri.IsAbsoluteUri)
        {
            completeUri = new UriBuilder(uri)
            {
                Query = string.Empty,
                Fragment = string.Empty,
            }.Uri;
        }
        else
        {
            string relative = uri.OriginalString;
            int suffixIndex = relative.IndexOfAny(['?', '#']);
            if (suffixIndex >= 0)
                relative = relative[..suffixIndex];
            completeUri = new Uri(
                baseUri,
                new Uri(relative, UriKind.Relative));
        }

        if (!baseUri.IsBaseOf(completeUri))
            throw new ArgumentException("Uri is not a child of the base uri", nameof(uri));

        string physicalPath = Path.GetFullPath(
            Uri.UnescapeDataString(completeUri.LocalPath));
        string basePath = Path.GetFullPath(
            Uri.UnescapeDataString(baseUri.LocalPath));
        string relativePath = Path.GetRelativePath(basePath, physicalPath);
        if (relativePath == "."
            || Path.IsPathRooted(relativePath)
            || relativePath == ".."
            || relativePath.StartsWith(
                ".." + Path.DirectorySeparatorChar,
                StringComparison.Ordinal))
        {
            throw new ArgumentException("Uri is not a child of the base uri", nameof(uri));
        }

        return new(
            completeUri,
            physicalPath,
            relativePath.Replace(Path.DirectorySeparatorChar, '/'));
    }
}
