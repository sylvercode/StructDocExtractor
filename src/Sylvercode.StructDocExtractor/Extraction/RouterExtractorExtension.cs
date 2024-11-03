
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Extraction.PreviewProvider;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class RouterExtractorExtension
{
    public class RouterExtractorBuilder<TExtractionData>(IServiceCollection services)
    {
        public void AddExtractor<TExtractor>(IRouterExtractorSelector<TExtractionData> selector)
            where TExtractor : class, IExtractor<TExtractionData>
        {
            services.TryAddSingleton<TExtractor>();
            services.AddOptions<RouterExtractorList<TExtractionData>>()
                .Configure<TExtractor>((option, extractor) =>
                    option.Add(selector, extractor));
        }

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

        public void NoExtractorAsError(bool value = true)
        {
            services.Configure<RouterExtractor<TExtractionData>.RouterExtractorOptions>(options => options.NoExtractorAsError = value);
        }
    }

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

    public static RouterExtractorBuilder<TExtractionData> ConfigureRouterExtractors<TExtractionData>(this IServiceCollection services)
        => new(services);
}
