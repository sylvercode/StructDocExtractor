using System.Diagnostics.CodeAnalysis;

namespace Sylvercode.StructDocExtractor.Extraction;

/// <summary>Defines the contract for running a full extraction pass over typed source data, producing a structured document result.</summary>
/// <typeparam name="TExtractionData">The type of source data to extract from.</typeparam>
public interface IExtractor<TExtractionData>
{
    /// <summary>Extracts a structured document from <paramref name="data"/>, optionally notifying an observer about each task as it is processed.</summary>
    /// <param name="data">The root source data to extract from; must not be <see langword="null"/>.</param>
    /// <param name="observer">An optional observer that receives each <see cref="ExtractionTask"/> as it is processed; may be <see langword="null"/>.</param>
    /// <returns>The <see cref="ExtractionResult"/> produced by the extraction pass.</returns>
    ExtractionResult Extract([DisallowNull] TExtractionData data, IObserver<ExtractionTask>? observer = null);
}
