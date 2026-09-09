using Microsoft.Extensions.DependencyInjection.Extensions;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Serialization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure


/// <summary>Provides extension methods for registering <see cref="MarkdownStreamWriterProvider"/> in a DI container.</summary>
public static class MarkdownStreamWriterProviderExtensions
{
    /// <summary>Registers <see cref="MarkdownStreamWriterProvider"/> and its <see cref="ITextWriterProvider"/> adapter in <paramref name="services"/>, optionally applying <paramref name="copnfig"/>.</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="copnfig">An optional action to configure <see cref="MarkdownStreamWriterProvider.Options"/>.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddMarkdownStreamWriterProvider(this IServiceCollection services, Action<MarkdownStreamWriterProvider.Options>? copnfig = null)
    {
        services.TryAddSingleton<ITextWriterProvider>(sp => sp.GetRequiredService<ITextWriterProvider<MarkdownStreamWriter>>());
        services.TryAddSingleton<ITextWriterProvider<MarkdownStreamWriter>, MarkdownStreamWriterProvider>();
        Options.OptionsBuilder<MarkdownStreamWriterProvider.Options> optionsBuilder = services.AddOptions<MarkdownStreamWriterProvider.Options>();
        if (copnfig is not null)
            optionsBuilder.Configure(copnfig);

        return services;
    }
}
