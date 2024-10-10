
#pragma warning disable IDE0130 // Namespace does not match folder structure
using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class AngleSharpExtractorExtentions
{
    public static IServiceCollection AddAngleExtractorFor<TNodeFactoryProvider>(
        this IServiceCollection services,
        IRouterExtractorSelector<IElement> selector)
        where TNodeFactoryProvider : class, IStructDocNodeFactoryProvider<IElement, HtmlNodeDiscriminator>
    {
        services.ConfigureRouterExtractors<IElement>()
            .AddDefaultExtractorFor<HtmlNodeDiscriminator, TNodeFactoryProvider>(selector);

        return services;
    }
}
