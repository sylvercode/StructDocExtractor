namespace Sylvercode.SiteSource;

public class SiteSourceProvider(ISiteSource? defaultSource) : ISiteSourceProvider
{
    private class Entry(IUriMatcher matcher, ISiteSource source)
    {
        public IUriMatcher Matcher { get; } = matcher;
        public ISiteSource Source { get; } = source;
    }

    private readonly List<Entry> _sourceProviders = [];

    public void Add(IUriMatcher matcher, ISiteSource source)
        => _sourceProviders.Add(new Entry(matcher, source));

    public ISiteSource GetSource(Uri uri)
        => _sourceProviders.FirstOrDefault(entry => entry.Matcher.IsMatching(uri))?.Source
           ?? defaultSource
           ?? throw new InvalidOperationException("No source found");
}
