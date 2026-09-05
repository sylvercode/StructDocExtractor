using Microsoft.Extensions.DependencyInjection.Extensions;
using Sylvercode.StructDocExtractor.Serialization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>Extension helpers for registering <see cref="IndentedStreamWriterProvider"/> in a DI container.</summary>
public static class IndentedStreamWriterProviderExtensions
{
    /// <summary>Registers <see cref="IndentedStreamWriterProvider"/> as both <see cref="ITextWriterProvider"/> and <see cref="ITextWriterProvider{IndentedStreamWriter}"/> in <paramref name="services"/>.</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="copnfig">An optional action to configure <see cref="IndentedStreamWriterProvider.Options"/>.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddIndentedStreamWriterProvider(this IServiceCollection services, Action<IndentedStreamWriterProvider.Options>? copnfig = null)
    {
        services.TryAddSingleton<ITextWriterProvider>(sp => sp.GetRequiredService<ITextWriterProvider<IndentedStreamWriter>>());
        services.TryAddSingleton<ITextWriterProvider<IndentedStreamWriter>, IndentedStreamWriterProvider>();
        Options.OptionsBuilder<IndentedStreamWriterProvider.Options> optionsBuilder = services.AddOptions<IndentedStreamWriterProvider.Options>();
        if (copnfig is not null)
            optionsBuilder.Configure(copnfig);

        return services;
    }
}
