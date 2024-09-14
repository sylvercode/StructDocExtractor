using System;

namespace Sylvercode.SiteSource;

public class UriMatcherByBase(Uri baseUri) : IUriMatcher
{
    public bool IsMatching(Uri uri) => baseUri.IsBaseOf(uri);
}
