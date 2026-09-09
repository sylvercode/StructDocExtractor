using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor.Sources;

namespace Sylvercode.SiteExtractor.Tests;

/// <summary>Tests for <see cref="MemorySiteSource{TData}"/> document retrieval and missing-key behaviour.</summary>
public class MemorySiteSourceTests
{
    /// <summary>Verifies that an existing entry is reachable and returns its stored value.</summary>
    [Fact]
    public void ExistingData_IsPresent()
    {
        // Given
        Uri uri = new("https://example.com");
        string data = nameof(data);

        // When
        MemorySiteSource<string> source = new(Options.Create(new MemorySiteSource<string>.Options()))
        {
            [uri] = data
        };

        // Then
        Assert.True(source.CanGetFrom(uri));
        Assert.True(source.DataExists(uri));
        Assert.Equal(data, source.GetData(uri));
    }

    /// <summary>Verifies that a URI not present in the source is correctly reported as absent.</summary>
    [Fact]
    public void NonExistingData_IsNotPresent()
    {
        // Given
        Uri uri = new("https://example.com");

        // When
        MemorySiteSource<string> source = new(Options.Create(new MemorySiteSource<string>.Options()))
        {
            [new Uri("https://other.example.org")] = "data"
        };

        // Then
        Assert.False(source.CanGetFrom(uri));
        Assert.False(source.DataExists(uri));
        Assert.Throws<InvalidOperationException>(() => source.GetData(uri));
    }

    /// <summary>Verifies that the configured default data is returned when a URI is not explicitly stored.</summary>
    [Fact]
    public void DefaultData_IsReturned()
    {
        // Given
        Uri uri = new("https://example.com");
        string defaultData = nameof(defaultData);

        var options = Options.Create(new MemorySiteSource<string>.Options()
        {
            DefaultData = defaultData
        });

        // When
        MemorySiteSource<string> source = new(options)
        {
            [new Uri("https://other.example.org")] = "data"
        };

        // Then
        Assert.True(source.CanGetFrom(uri));
        Assert.True(source.DataExists(uri));
        Assert.Equal(defaultData, source.GetData(uri));
    }
}
