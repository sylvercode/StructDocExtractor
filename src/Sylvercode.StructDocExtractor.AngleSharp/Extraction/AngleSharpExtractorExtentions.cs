
#pragma warning disable IDE0130 // Namespace does not match folder structure
using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to register AngleSharp-backed
/// extractors in a DI container.
/// </summary>
public static class AngleSharpExtractorExtentions
{
    /// <summary>
    /// Registers a router extractor for the given <typeparamref name="TNodeFactoryProvider"/>
    /// using an explicit <see cref="IRouterExtractorSelector{INode}"/>.
    /// </summary>
    /// <typeparam name="TNodeFactoryProvider">
    /// The <see cref="IStructDocNodeFactoryProvider{TData,TDiscriminator}"/> implementation to register.
    /// </typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="selector">The router selector that determines which source nodes are handled.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddAngleExtractorFor<TNodeFactoryProvider>(
        this IServiceCollection services,
        IRouterExtractorSelector<INode> selector)
        where TNodeFactoryProvider : class, IStructDocNodeFactoryProvider<INode, HtmlNodeDiscriminator>
    {
        services.ConfigureRouterExtractors<INode>()
            .AddDefaultExtractorFor<HtmlNodeDiscriminator, TNodeFactoryProvider>(selector);

        return services;
    }

    /// <summary>
    /// Registers a router extractor for the given <typeparamref name="TNodeFactoryProvider"/>
    /// using the provider's own <see cref="IRouterExtractorSelectorProvider{TData}"/> implementation
    /// to supply the selector.
    /// </summary>
    /// <typeparam name="TNodeFactoryProvider">
    /// The factory provider that also implements <see cref="IRouterExtractorSelectorProvider{INode}"/>.
    /// </typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddAngleExtractorFor<TNodeFactoryProvider>(
        this IServiceCollection services)
        where TNodeFactoryProvider :
            class,
            IStructDocNodeFactoryProvider<INode, HtmlNodeDiscriminator>,
            IRouterExtractorSelectorProvider<INode>
    {
        services.ConfigureRouterExtractors<INode>()
            .AddDefaultExtractorFor<HtmlNodeDiscriminator, TNodeFactoryProvider>();

        return services;
    }
}
