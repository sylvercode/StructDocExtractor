using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor.Store;

namespace Sylvercode.SiteExtractor.Tests;

public class MemoryDataStoreTests
{
    [Theory]
    [InlineData("report%23a.md", "report%23b.md", "report#a.md", "report#b.md")]
    [InlineData("report%3Fa.md", "report%3Fb.md", "report?a.md", "report?b.md")]
    public void GetStream_EncodedDelimiters_KeepDestinationsDistinct(
        string firstUri,
        string secondUri,
        string firstPath,
        string secondPath)
    {
        MemoryDataStore store = new(Options.Create(
            new SiteExtractorOptions { OutputDirectory = "memory://output/" }));
        Uri first = new(firstUri, UriKind.Relative);
        Uri second = new(secondUri, UriKind.Relative);

        store.GetStream(first).WriteByte(1);
        store.GetStream(second).WriteByte(2);

        DataStorePath firstDestination = DataStorePathResolver.Resolve(store.BaseUri, first);
        DataStorePath secondDestination = DataStorePathResolver.Resolve(store.BaseUri, second);
        Assert.Equal(firstPath, firstDestination.RelativePath);
        Assert.Equal(secondPath, secondDestination.RelativePath);
        Assert.Equal(2, store.Count);
        Assert.Equal(new byte[] { 1 }, store[firstDestination.Uri].ToArray());
        Assert.Equal(new byte[] { 2 }, store[secondDestination.Uri].ToArray());
    }

    [Theory]
    [InlineData("%2e%2e%2foutside.md")]
    [InlineData("nested%2f%2e%2e%2foutside.md")]
    public void GetStream_EncodedDotSegments_AreRejected(string outputUri)
    {
        MemoryDataStore store = new(Options.Create(
            new SiteExtractorOptions { OutputDirectory = "memory://output/" }));

        Assert.Throws<ArgumentException>(() => store.GetStream(new Uri(outputUri, UriKind.Relative)));
        Assert.Empty(store);
    }
}
