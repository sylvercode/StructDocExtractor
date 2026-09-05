using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.SiteExtractor.Tests.Stubs;

/// <summary>Stub <see cref="Sylvercode.StructDocExtractor.Model.IStructDocNode"/> carrying a <see cref="System.Uri"/> used in URI-translation tests.</summary>
public class UriNode(string uri) : BaseStructDocNode(uri)
{
    /// <summary>Gets the <see cref="System.Uri"/> associated with this stub node.</summary>
    public Uri Uri { get; } = new(uri);
}
