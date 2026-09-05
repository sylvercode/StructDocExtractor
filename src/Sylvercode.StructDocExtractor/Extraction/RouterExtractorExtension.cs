
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Extraction.PreviewProvider;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>Provides extension methods for <see cref="IServiceCollection"/> to register and compose <see cref="RouterExtractor{TExtractionData}"/> in a DI container.</summary>
public static class RouterExtractorExtension
{
    /// <summary>Fluent builder for registering extractors into a <see cref="RouterExtractorList{TExtractionData}"/> via <see cref="IServiceCollection"/>.</summary>
    /// <typeparam name="TExtractionData">The type of source data element the extractors handle.</typeparam>
    public class RouterExtractorBuilder<TExtractionData>(IServiceCollection services)
    {
        /// <summary>Registers a custom extractor type into the router list under the given selector.</summary>
        /// <typeparam name="TExtractor">The extractor type to register.</typeparam>
        /// <param name="selector">The selector that determines whether this extractor handles a given source element.</param>
        public void AddExtractor<TExtractor>(IRouterExtractorSelector<TExtractionData> selector)
            where TExtractor : class, IExtractor<TExtractionData>
        {
            services.TryAddSingleton<TExtractor>();
            services.AddOptions<RouterExtractorList<TExtractionData>>()
                .Configure<TExtractor>((option, extractor) =>
                    option.Add(selector, extractor));
        }

        /// <summary>Registers a standard <see cref="Extractor{TExtractionData, TDataDiscriminator}"/> backed by <typeparamref name="TNodeFactoryProvider"/>, using the provider's built-in <see cref="IRouterExtractorSelectorProvider{TExtractionData}.DefaultRouterSelector"/>.</summary>
        /// <typeparam name="TDataDiscriminator">The discriminator type for the extractor.</typeparam>
        /// <typeparam name="TNodeFactoryProvider">The factory provider type that also supplies the default router selector.</typeparam>
        /// <param name="extractorOption">Optional extractor options; uses defaults when <see langword="null"/>.</param>
        public void AddDefaultExtractorFor<TDataDiscriminator, TNodeFactoryProvider>(
            ExtractorOption? extractorOption = default)
            where TNodeFactoryProvider :
                class,
                IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator>,
                IRouterExtractorSelectorProvider<TExtractionData>
        {
            services.TryAddSingleton<TNodeFactoryProvider>();
            services.AddOptions<RouterExtractorList<TExtractionData>>()
                .Configure<IServiceProvider, TNodeFactoryProvider>((option, sp, nfp) =>
                {
                    var dataDiscriminatorFactory = sp.GetService<IDataDiscriminatorFactory<TExtractionData, TDataDiscriminator>>();
                    var dataPreviewProvider = sp.GetService<IDataPreviewProvider<TExtractionData>>();
                    var loggerFactory = sp.GetService<ILoggerFactory>();
                    Extractor<TExtractionData, TDataDiscriminator> extractor = new(
                        nfp,
                        extractorOption ?? default,
                        dataDiscriminatorFactory,
                        dataPreviewProvider,
                        loggerFactory);
                    option.Add(nfp.DefaultRouterSelector, extractor);
                });
        }

        /// <summary>Registers a standard <see cref="Extractor{TExtractionData, TDataDiscriminator}"/> backed by <typeparamref name="TNodeFactoryProvider"/>, under the given explicit selector.</summary>
        /// <typeparam name="TDataDiscriminator">The discriminator type for the extractor.</typeparam>
        /// <typeparam name="TNodeFactoryProvider">The factory provider type to register.</typeparam>
        /// <param name="selector">The selector that determines whether this extractor handles a given source element.</param>
        /// <param name="extractorOption">Optional extractor options; uses defaults when <see langword="null"/>.</param>
        public void AddDefaultExtractorFor<TDataDiscriminator, TNodeFactoryProvider>(
            IRouterExtractorSelector<TExtractionData> selector,
            ExtractorOption? extractorOption = default)
            where TNodeFactoryProvider : class, IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator>
        {
            services.TryAddSingleton<TNodeFactoryProvider>();
            services.AddOptions<RouterExtractorList<TExtractionData>>()
                .Configure<IServiceProvider, TNodeFactoryProvider>((option, sp, nfp) =>
                {
                    var dataDiscriminatorFactory = sp.GetService<IDataDiscriminatorFactory<TExtractionData, TDataDiscriminator>>();
                    var dataPreviewProvider = sp.GetService<IDataPreviewProvider<TExtractionData>>();
                    var loggerFactory = sp.GetService<ILoggerFactory>();
                    Extractor<TExtractionData, TDataDiscriminator> extractor = new(
                        nfp,
                        extractorOption ?? default,
                        dataDiscriminatorFactory,
                        dataPreviewProvider,
                        loggerFactory);
                    option.Add(selector, extractor);
                });
        }

        /// <summary>Configures the router to treat an unmatched extraction attempt as an error.</summary>
        /// <param name="value">When <see langword="true"/>, unmatched extractions produce an error result; otherwise they are skipped.</param>
        public void NoExtractorAsError(bool value = true)
        {
            services.Configure<RouterExtractor<TExtractionData>.RouterExtractorOptions>(options => options.NoExtractorAsError = value);
        }
    }

    /// <summary>Registers <see cref="RouterExtractor{TExtractionData}"/> and its supporting services, then invokes the optional configuration delegate.</summary>
    /// <typeparam name="TExtractionData">The type of source data element the router extracts.</typeparam>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configure">Optional delegate to configure extractors via <see cref="RouterExtractorBuilder{TExtractionData}"/>.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddRouterExtractor<TExtractionData>(this IServiceCollection services, Action<RouterExtractorBuilder<TExtractionData>>? configure = null)
    {
        services.AddOptions<RouterExtractorList<TExtractionData>>();
        services.AddOptions<RouterExtractor<TExtractionData>.RouterExtractorOptions>();
        services.TryAddSingleton<IExtractor<TExtractionData>>(sp =>
        {
            var extractors = sp.GetRequiredService<IOptions<RouterExtractorList<TExtractionData>>>();
            var options = sp.GetRequiredService<IOptions<RouterExtractor<TExtractionData>.RouterExtractorOptions>>();
            return new RouterExtractor<TExtractionData>(extractors.Value, options);
        });
        configure?.Invoke(ConfigureRouterExtractors<TExtractionData>(services));
        return services;
    }

    /// <summary>Returns a <see cref="RouterExtractorBuilder{TExtractionData}"/> for configuring extractors on an existing service collection.</summary>
    /// <typeparam name="TExtractionData">The type of source data element the router extracts.</typeparam>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>A builder instance for fluent extractor registration.</returns>
    public static RouterExtractorBuilder<TExtractionData> ConfigureRouterExtractors<TExtractionData>(this IServiceCollection services)
        => new(services);
}
