namespace Sylvercode.SiteExtractor.UriUtils;

/// <summary>Implementation of <see cref="IUriMatcher"/> that identifies URIs pointing to common image file extensions</summary>
public class ImageUriMatcher : IUriMatcher
{
    /// <summary>Gets a shared default instance of <see cref="ImageUriMatcher"/></summary>
    public static readonly ImageUriMatcher Default = new();
    private readonly string[] imageExtensions = [".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".svg"];

    /// <inheritdoc/>
    public bool IsMatching(Uri uri) => imageExtensions.Any(uri.AbsolutePath.EndsWith);
}
