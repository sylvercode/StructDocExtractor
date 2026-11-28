namespace Sylvercode.SiteExtractor.Resources;

/// <summary>Identifies the kind of output produced for a resource.</summary>
public enum ResourceOutputKind
{
    /// <summary>No output is expected or produced.</summary>
    None,

    /// <summary>A document was extracted from the source.</summary>
    Document,

    /// <summary>A source asset was copied without document extraction.</summary>
    Asset
}
