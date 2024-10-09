
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sylvercode.StructDocExtractor.Extraction;

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

        public void NoExtractorAsError(bool value = true)
        {
            services.Configure<RouterExtractor<TExtractionData>.RouterExtractorOptions>(options => options.NoExtractorAsError = value);
        }
    }

    public static IServiceCollection AddRouterExtractor<TExtractionData>(this IServiceCollection services, Action<RouterExtractorBuilder<TExtractionData>> configure)
    {
        services.AddOptions<RouterExtractorList<TExtractionData>>();
        services.AddOptions<RouterExtractor<TExtractionData>.RouterExtractorOptions>();
        services.TryAddSingleton<IExtractor<TExtractionData>>(sp =>
        {
            var extractors = sp.GetRequiredService<IOptions<RouterExtractorList<TExtractionData>>>();
            var options = sp.GetRequiredService<IOptions<RouterExtractor<TExtractionData>.RouterExtractorOptions>>();
            return new RouterExtractor<TExtractionData>(extractors.Value, options);
        });
        configure.Invoke(new RouterExtractorBuilder<TExtractionData>(services));
        return services;
    }
}
