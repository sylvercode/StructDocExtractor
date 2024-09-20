using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Resources.Processors;

public class ResourceProcessorProvider() : IResourceProcessorProvider
{
    private class Entry(IUriMatcher matcher, IResourceProcessor processor)
    {
        public IUriMatcher Matcher { get; } = matcher;
        public IResourceProcessor Processor { get; } = processor;
    }

    private readonly List<Entry> _entries = [];

    public void RegisterProcessor(IUriMatcher matcher, IResourceProcessor processor)
        => _entries.Add(new Entry(matcher, processor));

    public IResourceProcessor GetProcessor(Uri uri)
        => _entries
            .FirstOrDefault(entry => entry.Matcher.IsMatching(uri))
            ?.Processor ?? NoPullProcessor.Default;
}
