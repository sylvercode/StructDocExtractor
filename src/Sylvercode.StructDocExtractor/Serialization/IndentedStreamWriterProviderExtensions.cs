using Microsoft.Extensions.DependencyInjection.Extensions;
using Sylvercode.StructDocExtractor.Serialization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure


public static class IndentedStreamWriterProviderExtensions
{
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
