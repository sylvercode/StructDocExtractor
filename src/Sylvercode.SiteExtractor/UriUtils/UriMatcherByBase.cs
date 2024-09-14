using System;

namespace Sylvercode.SiteExtractor.UriUtils;

public class UriMatcherByBase(Uri baseUri) : IUriMatcher
{
    public bool IsMatching(Uri uri) => baseUri.IsBaseOf(uri);
}
