namespace Sylvercode.SiteExtractor;

/// <summary>Defines the contract for running a complete site extraction pass.</summary>
public interface ISiteExtractor
{
    /// <summary>Extracts all reachable resources starting from <paramref name="uri"/>.</summary>
    /// <param name="uri">The seed URI from which site extraction begins.</param>
    void Extract(Uri uri);
}
