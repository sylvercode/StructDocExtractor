using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Resources.Processors;

public class ResourceProcessorProvider : IResourceProcessorProvider
{
    private sealed class Entry(IUriMatcher matcher, IResourceProcessor processor)
    {
        public IUriMatcher Matcher { get; } = matcher;
        public IResourceProcessor Processor { get; } = processor;
    }

    private readonly List<Entry> _entries = [];

    private IResourceProcessor? _defaultProcessor;

    public void RegisterDefaultProcessor(IResourceProcessor processor)
    {
        if (_defaultProcessor is not null)
            throw new InvalidOperationException("Default processor is already registered.");

        _defaultProcessor = processor;
    }

    public void RegisterProcessor(IUriMatcher? matcher, IResourceProcessor processor)
    {
        if (matcher is null)
            RegisterDefaultProcessor(processor);
        else
            _entries.Add(new Entry(matcher, processor));
    }

    public IResourceProcessor? GetProcessor(Uri uri)
        => _entries
            .FirstOrDefault(entry => entry.Matcher.IsMatching(uri))
            ?.Processor ?? _defaultProcessor;
}
