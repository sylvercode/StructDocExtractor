namespace Sylvercode.SiteExtractor.UriUtils;

/// <summary>Defines the contract for testing whether a <see cref="Uri"/> satisfies a given matching rule</summary>
public interface IUriMatcher
{
    /// <summary>Returns a value indicating whether <paramref name="uri"/> satisfies this matcher's rule</summary>
    /// <param name="uri">The URI to test against the matching rule</param>
    /// <returns><see langword="true"/> if the URI matches; otherwise <see langword="false"/></returns>
    bool IsMatching(Uri uri);
}
