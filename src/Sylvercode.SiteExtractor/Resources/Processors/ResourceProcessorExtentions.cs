using System.Collections;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor;
using Sylvercode.SiteExtractor.Resources.Processors;
using Sylvercode.SiteExtractor.UriUtils;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ResourceProcessorProviderExtentions
{
    public static IServiceCollection AddImageCopierProcessor(this IServiceCollection services) =>
        services.AddResourceProcessor<ResourceCopier>(ImageUriMatcher.Default);

    public static IServiceCollection AddResourceDataExtractorProcessor<TExtractionData>(this IServiceCollection services) =>
        services.AddResourceProcessor<ResourceDataExtractor<TExtractionData>, IOptions<SiteExtractorOptions>>((options)
            => new UriMatcherByBase(options.Value.GetSourceBaseUri()));

    public static IServiceCollection AddResourceProcessorProvider(this IServiceCollection services)
    {
        services.AddOptions<ResourceProcessorCollection>();
        services.TryAddSingleton<IResourceProcessorProvider>((serviceProvider) =>
        {
            var ResourceProcessorCollection = serviceProvider.GetRequiredService<IOptions<ResourceProcessorCollection>>();
            ResourceProcessorProvider provider = new();
            foreach (var entry in ResourceProcessorCollection.Value)
                provider.RegisterProcessor(entry.UriMatcher, entry.Processor);
            return provider;
        });

        return services;
    }

    public static IServiceCollection AddResourceProcessor<TProcessor>(this IServiceCollection services, IUriMatcher uriMatcher)
        where TProcessor : class, IResourceProcessor
        => services.AddResourceProcessor<TProcessor>(() => uriMatcher);

    public static IServiceCollection AddResourceProcessor<TProcessor>(this IServiceCollection services, Func<IUriMatcher> uriMatcherProvider)
        where TProcessor : class, IResourceProcessor
    {
        services.TryAddSingleton<TProcessor>();
        services.AddOptions<ResourceProcessorCollection>().Configure<TProcessor>((col, p) => col.Add(uriMatcherProvider(), p));
        return services;
    }

    public static IServiceCollection AddResourceProcessor<TProcessor, TDep>(this IServiceCollection services, Func<TDep, IUriMatcher> uriMatcherProvider)
        where TProcessor : class, IResourceProcessor
        where TDep : class
    {
        services.TryAddSingleton<TProcessor>();
        services.AddOptions<ResourceProcessorCollection>().Configure<TProcessor, TDep>((col, p, dep) => col.Add(uriMatcherProvider(dep), p));
        return services;
    }

    public static IServiceCollection AddResourceProcessor(this IServiceCollection services, IResourceProcessor resourceProcessor, IUriMatcher uriMatcherProvider)
    {
        services.AddOptions<ResourceProcessorCollection>().Configure(col => col.Add(uriMatcherProvider, resourceProcessor));
        return services;
    }

    public class ResourceProcessorCollection : ICollection<ResourceProcessorCollection.Entry>
    {
        private readonly List<Entry> _entries = [];

        public class Entry(IUriMatcher uriMatcher, IResourceProcessor processor)
        {
            public IUriMatcher UriMatcher { get; } = uriMatcher;
            public IResourceProcessor Processor { get; } = processor;
        }

        public void Add(IUriMatcher uriMatcher, IResourceProcessor processor)
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
