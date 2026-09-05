using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.SiteExtractor.Resources.Processors;

/// <summary>Contract for filtering which dependency links discovered inside a resource should be followed during site extraction.</summary>
public interface IResourceDependancyFilter
{
    /// <summary>Determines whether the given reference link should be tracked as a resource dependency.</summary>
    /// <param name="referencer">The structural node carrying the URI reference to evaluate.</param>
    /// <returns><see langword="true"/> if the reference should be followed; <see langword="false"/> if it should be ignored.</returns>
    bool IsAccepted(IStructDocReferencer referencer);
}
