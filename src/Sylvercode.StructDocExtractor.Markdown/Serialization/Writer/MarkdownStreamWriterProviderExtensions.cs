using Microsoft.Extensions.DependencyInjection.Extensions;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Serialization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure


public static class MarkdownStreamWriterProviderExtensions
{
    public static IServiceCollection AddMarkdownStreamWriterProvider(this IServiceCollection services, Action<MarkdownStreamWriterProvider.Options>? copnfig = null)
    {
        services.TryAddSingleton<ITextWriterProvider, MarkdownStreamWriterProvider>();
        Options.OptionsBuilder<MarkdownStreamWriterProvider.Options> optionsBuilder = services.AddOptions<MarkdownStreamWriterProvider.Options>();
        if (copnfig is not null)
            optionsBuilder.Configure(copnfig);

        return services;
    }
}
