namespace Sylvercode.SiteSource.Web;

public class ImageSelector : ISiteSourceProviderSelector
{
    private readonly string[] imageExtensions = [".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".svg"];
    public bool IsValid(Uri uri) => imageExtensions.Any(uri.AbsolutePath.EndsWith);
}
