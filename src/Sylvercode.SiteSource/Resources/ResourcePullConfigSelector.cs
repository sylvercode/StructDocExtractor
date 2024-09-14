using System;

namespace Sylvercode.SiteSource.Resources;

public class ResourcePullConfigSelector(IResourcePullConfig defaultPullConfig) : IResourcePullConfig
{
    private class Entry(IUriMatcher matcher, IResourcePullConfig config)
    {
        public IUriMatcher Matcher { get; } = matcher;
        public IResourcePullConfig Config { get; } = config;
    }
    public IResourcePullConfig DefaultPullConfig { get; } = defaultPullConfig ?? StaticResourcePullConfig.Default;

    private readonly List<Entry> pullConfigs = [];

    public ResourcePullConfigSelector(ResourcePullType staticDefault = ResourcePullType.NoPull)
        : this(new StaticResourcePullConfig(staticDefault)) { }

    public void AddPullConfig(IUriMatcher matcher, IResourcePullConfig pullConfig)
        => pullConfigs.Add(new(matcher, pullConfig));

    public ResourcePullType GetPullType(Uri uri)
        => (pullConfigs.First(e => e.Matcher.IsMatching(uri)).Config ?? DefaultPullConfig).GetPullType(uri);
}
