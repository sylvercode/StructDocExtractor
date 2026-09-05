using System.Collections;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor;
using Sylvercode.SiteExtractor.Resources.Processors;
using Sylvercode.SiteExtractor.UriUtils;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>Provides extension methods for <see cref="IServiceCollection"/> to compose and register resource processors in a DI container.</summary>
public static class ResourceProcessorProviderExtentions
{
    /// <summary>Registers a <see cref="ResourceCopier"/> processor that copies image URIs to the data store.</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddImageCopierProcessor(this IServiceCollection services) =>
        services.AddResourceProcessor<ResourceCopier>(ImageUriMatcher.Default);

    /// <summary>Registers a <see cref="ResourceDataExtractor{TExtractionData}"/> processor scoped to the configured source base URI.</summary>
    /// <typeparam name="TExtractionData">The raw data type provided by the site source.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddResourceDataExtractorProcessor<TExtractionData>(
        this IServiceCollection services) =>
        services.AddResourceProcessor<ResourceDataExtractor<TExtractionData>, IOptions<SiteExtractorOptions>>(
            (options) => new UriMatcherByBase(options.Value.GetSourceBaseUri()));

    /// <summary>Registers <see cref="IResourceProcessorProvider"/> as a singleton, wiring all previously registered processors from <c>ResourceProcessorCollection</c> options.</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddResourceProcessorProvider(this IServiceCollection services)
    {
        services.AddOptions<ResourceProcessorCollection>();
        services.TryAddSingleton<IResourceProcessorProvider>((serviceProvider) =>
        {
            var ResourceProcessorCollection =
                serviceProvider.GetRequiredService<IOptions<ResourceProcessorCollection>>();
            ResourceProcessorProvider provider = new();
            foreach (var entry in ResourceProcessorCollection.Value)
                provider.RegisterProcessor(entry.UriMatcher, entry.Processor);
            return provider;
        });

        return services;
    }

    /// <summary>Registers a default <typeparamref name="TProcessor"/> processor with no URI matcher, used when no other processor matches.</summary>
    /// <typeparam name="TProcessor">The processor type to register as the default.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddDefaultResourceProcessor<TProcessor>(
        this IServiceCollection services)
            where TProcessor : class, IResourceProcessor
    {
        services.TryAddSingleton<TProcessor>();
        services.AddOptions<ResourceProcessorCollection>().Configure<TProcessor>(
            (col, p) => col.Add(p));
        return services;
    }

    /// <summary>Registers <typeparamref name="TProcessor"/> as a singleton processor matched against <paramref name="uriMatcher"/>.</summary>
    /// <typeparam name="TProcessor">The processor type to register.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="uriMatcher">The matcher used to select this processor for incoming resource URIs.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddResourceProcessor<TProcessor>(
        this IServiceCollection services,
        IUriMatcher uriMatcher)
        where TProcessor : class, IResourceProcessor
        => services.AddResourceProcessor<TProcessor>(() => uriMatcher);

    /// <summary>Registers <typeparamref name="TProcessor"/> as a singleton processor, with the URI matcher supplied by a factory delegate.</summary>
    /// <typeparam name="TProcessor">The processor type to register.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="uriMatcherProvider">A delegate that produces the <see cref="IUriMatcher"/> at registration time.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddResourceProcessor<TProcessor>(
        this IServiceCollection services,
        Func<IUriMatcher> uriMatcherProvider)
        where TProcessor : class, IResourceProcessor
    {
        services.TryAddSingleton<TProcessor>();
        services.AddOptions<ResourceProcessorCollection>().Configure<TProcessor>(
            (col, p) => col.Add(uriMatcherProvider(), p));
        return services;
    }

    /// <summary>Registers <typeparamref name="TProcessor"/> as a singleton processor, resolving the URI matcher from a DI dependency <typeparamref name="TDep"/>.</summary>
    /// <typeparam name="TProcessor">The processor type to register.</typeparam>
    /// <typeparam name="TDep">A DI-resolved dependency used to build the <see cref="IUriMatcher"/>.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="uriMatcherProvider">A delegate that builds the matcher from the resolved <typeparamref name="TDep"/>.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddResourceProcessor<TProcessor, TDep>(
        this IServiceCollection services,
        Func<TDep, IUriMatcher> uriMatcherProvider)
        where TProcessor : class, IResourceProcessor
        where TDep : class
    {
        services.TryAddSingleton<TProcessor>();
        services.AddOptions<ResourceProcessorCollection>().Configure<TProcessor, TDep>(
            (col, p, dep) => col.Add(uriMatcherProvider(dep), p));
        return services;
    }

    /// <summary>Registers a pre-constructed <paramref name="resourceProcessor"/> instance matched by <paramref name="uriMatcherProvider"/>.</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="resourceProcessor">The processor instance to register.</param>
    /// <param name="uriMatcherProvider">The matcher that determines which URIs this processor handles.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddResourceProcessor(
        this IServiceCollection services,
        IResourceProcessor resourceProcessor,
        IUriMatcher uriMatcherProvider)
    {
        services.AddOptions<ResourceProcessorCollection>().Configure(
            col => col.Add(uriMatcherProvider, resourceProcessor));
        return services;
    }

    /// <summary>Options collection that accumulates <see cref="Entry"/> items pairing processors with their URI matchers for later registration into <see cref="ResourceProcessorProvider"/>.</summary>
    public class ResourceProcessorCollection : ICollection<ResourceProcessorCollection.Entry>
    {
        private readonly List<Entry> _entries = [];

        public class Entry(IUriMatcher? uriMatcher, IResourceProcessor processor)
        {
            /// <summary>Gets the URI matcher for this entry, or <see langword="null"/> if this is the default (fallback) processor.</summary>
            public IUriMatcher? UriMatcher { get; } = uriMatcher;

            /// <summary>Gets the processor associated with this entry.</summary>
            public IResourceProcessor Processor { get; } = processor;
        }

        /// <summary>Adds a default (no-matcher) processor entry.</summary>
        /// <param name="processor">The processor to register as the default fallback.</param>
        public void Add(IResourceProcessor processor)
            => Add(new Entry(null, processor));

        /// <summary>Adds a processor entry paired with the given URI matcher.</summary>
        /// <param name="uriMatcher">The matcher, or <see langword="null"/> for a default (fallback) entry.</param>
        /// <param name="processor">The processor to register.</param>
        public void Add(IUriMatcher? uriMatcher, IResourceProcessor processor)
            => _entries.Add(new Entry(uriMatcher, processor));

        #region ICollection
        public int Count => ((ICollection<Entry>)_entries).Count;

        public bool IsReadOnly => ((ICollection<Entry>)_entries).IsReadOnly;

        public void Add(Entry item)
        {
            ((ICollection<Entry>)_entries).Add(item);
        }

        public void Clear()
        {
            ((ICollection<Entry>)_entries).Clear();
        }

        public bool Contains(Entry item)
        {
            return ((ICollection<Entry>)_entries).Contains(item);
        }

        public void CopyTo(Entry[] array, int arrayIndex)
        {
            ((ICollection<Entry>)_entries).CopyTo(array, arrayIndex);
        }

        public bool Remove(Entry item)
        {
            return ((ICollection<Entry>)_entries).Remove(item);
        }

        public IEnumerator<Entry> GetEnumerator()
        {
            return ((IEnumerable<Entry>)_entries).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_entries).GetEnumerator();
        }
        #endregion
    }
}
