namespace Sylvercode.SiteSource.Tests;

public class MemorySiteSourceTests
{
    [Fact]
    public void ExistingData_IsPresent()
    {
        // Given
        Uri uri = new("https://example.com");
        string data = nameof(data);

        // When
        MemorySiteSource<string> source = new()
        {
            [uri] = data
        };

        // Then
        Assert.True(source.CanGetFrom(uri));
        Assert.True(source.DataExists(uri));
        Assert.Equal(data, source.GetData(uri));
    }

    [Fact]
    public void NonExistingData_IsNotPresent()
    {
        // Given
        Uri uri = new("https://example.com");

        // When
        MemorySiteSource<string> source = new()
        {
            [new Uri("https://other.example.org")] = "data"
        };

        // Then
        Assert.False(source.CanGetFrom(uri));
        Assert.False(source.DataExists(uri));
        Assert.Throws<InvalidOperationException>(() => source.GetData(uri));
    }

    [Fact]
    public void DefaultData_IsReturned()
    {
        // Given
        Uri uri = new("https://example.com");
        string defaultData = nameof(defaultData);

        // When
        MemorySiteSource<string> source = new(defaultData)
        {
            [new Uri("https://other.example.org")] = "data"
        };

        // Then
        Assert.True(source.CanGetFrom(uri));
        Assert.True(source.DataExists(uri));
        Assert.Equal(defaultData, source.GetData(uri));
    }
}
