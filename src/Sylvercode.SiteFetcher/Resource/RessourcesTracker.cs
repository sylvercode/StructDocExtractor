namespace Sylvercode.SiteFetcher.Resource;

public interface IResourcesTracker
{
    void OnTaskResult(object sender, EventArgs e);
}

public interface IResourcePullConfig
{
    ResourcePullType GetPullType(Uri uri);
}

public class StaticResourcePullConfig(ResourcePullType pullType) : IResourcePullConfig
{
    public static StaticResourcePullConfig Default { get; } = new StaticResourcePullConfig(ResourcePullType.NoPull);
    public ResourcePullType GetPullType(Uri uri) => pullType;
}

public abstract class BaseResourcesTracker(ResourceDictionary resourceDictionary,
                                           IResourcePullConfig config) : IResourcesTracker
{
    public void OnTaskResult(object sender, EventArgs e)
    {
        Uri? uri = GetUri(sender);
        if (uri is null
            || resourceDictionary.ContainsKey(uri))
            return;

        ResourcePullType pullType = config.GetPullType(uri);
        resourceDictionary.Add(uri, pullType);
    }

    protected abstract Uri? GetUri(object sender);
}
