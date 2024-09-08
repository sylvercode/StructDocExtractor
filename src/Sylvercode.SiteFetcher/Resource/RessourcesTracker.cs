namespace Sylvercode.SiteFetcher.Resource;

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
