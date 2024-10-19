using Microsoft.Extensions.DependencyInjection.Extensions;
using Sylvercode.StructDocExtractor.Serialization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure


public static class IndentedStreamWriterProviderExtensions
{
    public static IServiceCollection AddIndentedStreamWriterProvider(this IServiceCollection services)
    {
        services.TryAddSingleton<ITextWriterProvider, IndentedStreamWriterProvider>();
        services.AddOptions<IndentedStreamWriterProvider.Options>();

        return services;
    }
}
