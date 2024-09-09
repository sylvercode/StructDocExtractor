namespace Sylvercode.SiteSource;

public class SiteSourceProvider
{
    private class Entry(ISiteSourceProviderSelector selector, ISiteSource source)
    {
        public ISiteSourceProviderSelector Selector { get; } = selector;
        public ISiteSource Source { get; } = source;
    }

    private readonly List<Entry> _sourceProviders = [];

    public void Add(ISiteSourceProviderSelector selector, ISiteSource source)
        => _sourceProviders.Add(new Entry(selector, source));

    public ISiteSource GetSource(Uri uri, ISiteSource? defaultSource)
        => _sourceProviders.FirstOrDefault(entry => entry.Selector.IsValid(uri))?.Source
           ?? defaultSource
           ?? throw new InvalidOperationException("No source found");
}
