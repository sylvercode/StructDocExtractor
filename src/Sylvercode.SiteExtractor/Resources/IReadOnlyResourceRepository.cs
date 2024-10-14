using System.Diagnostics.CodeAnalysis;

namespace Sylvercode.SiteExtractor.Resources;

public interface IReadOnlyResourceRepository : IEnumerable<Resource>
{
    Resource this[Uri key] { get; }
    IEnumerable<Uri> UriResources { get; }
    int Count { get; }
    bool ContainsResourceForUri(Uri key);
    bool TryGetResourceForUri(Uri key, [MaybeNullWhen(false)] out Resource value);
    IReadOnlyDictionary<Uri, Resource> AsDictionary();
}
