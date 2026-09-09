using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Resources.Processors;

/// <summary>Default <see cref="IResourceProcessorProvider"/> that selects a processor by matching resource URIs against a priority-ordered list of <see cref="IUriMatcher"/> entries, falling back to an optional default processor.</summary>
/// <remarks>
/// Processors are registered via <see cref="RegisterProcessor"/> (with a matcher) or
/// <see cref="RegisterDefaultProcessor"/> (fallback with no matcher).  Only one default processor may be registered;
/// a second call throws <see cref="InvalidOperationException"/>.  At resolution time the first registered entry
/// whose matcher accepts the URI wins; if none match, the default processor is returned or <see langword="null"/>.
/// </remarks>
public class ResourceProcessorProvider : IResourceProcessorProvider
{
    private sealed class Entry(IUriMatcher matcher, IResourceProcessor processor)
    {
        public IUriMatcher Matcher { get; } = matcher;
        public IResourceProcessor Processor { get; } = processor;
    }

    private readonly List<Entry> _entries = [];

    private IResourceProcessor? _defaultProcessor;

    /// <summary>Registers a fallback processor used when no matcher-based entry accepts a given URI.</summary>
    /// <param name="processor">The default processor to register.</param>
    /// <exception cref="InvalidOperationException">Thrown if a default processor has already been registered.</exception>
    public void RegisterDefaultProcessor(IResourceProcessor processor)
    {
        if (_defaultProcessor is not null)
            throw new InvalidOperationException("Default processor is already registered.");

        _defaultProcessor = processor;
    }

    /// <summary>Registers a processor paired with a URI matcher, or as the default when <paramref name="matcher"/> is <see langword="null"/>.</summary>
    /// <param name="matcher">The matcher that selects resources for this processor, or <see langword="null"/> to register as the default.</param>
    /// <param name="processor">The processor to register.</param>
    public void RegisterProcessor(IUriMatcher? matcher, IResourceProcessor processor)
    {
        if (matcher is null)
            RegisterDefaultProcessor(processor);
        else
            _entries.Add(new Entry(matcher, processor));
    }

    /// <inheritdoc/>
    public IResourceProcessor? GetProcessor(Uri uri)
        => _entries
            .FirstOrDefault(entry => entry.Matcher.IsMatching(uri))
            ?.Processor ?? _defaultProcessor;
}
