namespace Sylvercode.SiteExtractor.UriUtils;

// TODO: Add tests
public class ImageUriMatcher : IUriMatcher
{
    public static readonly ImageUriMatcher Default = new();
    private readonly string[] imageExtensions = [".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".svg"];
    public bool IsMatching(Uri uri) => imageExtensions.Any(uri.AbsolutePath.EndsWith);
}
