namespace Sylvercode.SiteExtractor.Resources.Processors;

public interface IResourceProcessorProvider
{
    public IResourceProcessor? GetProcessor(Uri uri);
}
