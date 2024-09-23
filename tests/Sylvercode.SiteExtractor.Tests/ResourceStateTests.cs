using Sylvercode.SiteExtractor.Resources;

namespace Sylvercode.SiteExtractor.Tests;

public class ResourceStateTests_IsPullable
{
    [Fact]
    public void IsPullable()
    {
        // Given

        // When
        ResourceState resourceState = new(true);

        // Then
        Assert.True(resourceState.IsPullable);
        Assert.True(resourceState.IsPullNeeded);
        Assert.False(resourceState.IsPulling);
        Assert.False(resourceState.IsPulled);
    }
    [Fact]
    public void IsNotPullable()
    {
        // Given

        // When
        ResourceState resourceState = new(false);

        // Then
        Assert.False(resourceState.IsPullable);
        Assert.False(resourceState.IsPullNeeded);
        Assert.False(resourceState.IsPulling);
        Assert.True(resourceState.IsPulled);
    }
}

public class ResourceStateTests_AsPulled
{
    [Fact]
    public void NotPullable_Throws()
    {
        // Given
        ResourceState resourceState = new(false);

        // When
        void action() => resourceState.AsPulled();

        // Then
        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void Pulled_Throws()
    {
        // Given
        ResourceState resourceState = new ResourceState(true).AsPulled();

        // When
        void action() => resourceState.AsPulled();

        // Then
        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void Pending_ReturnsPulled()
    {
        // Given
        ResourceState resourceState = new(true);

        // When
        ResourceState result = resourceState.AsPulled();

        // Then
        Assert.True(result.IsPulled);
        Assert.True(result.IsPullable);
        Assert.False(result.IsPullNeeded);
        Assert.False(result.IsPulling);
    }

    [Fact]
    public void Pulling_ReturnsPulled()
    {
        // Given
        ResourceState resourceState = new ResourceState(true).AsPulling();

        // When
        ResourceState result = resourceState.AsPulled();

        // Then
        Assert.True(result.IsPulled);
        Assert.True(result.IsPullable);
        Assert.False(result.IsPullNeeded);
        Assert.False(result.IsPulling);
    }
}

public class ResourceStateTests_AsPulling
{
    [Fact]
    public void NotPullable_Throws()
    {
        // Given
        ResourceState resourceState = new(false);

        // When
        void action() => resourceState.AsPulling();

        // Then
        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void Pulled_Throws()
    {
        // Given
        ResourceState resourceState = new ResourceState(true).AsPulled();

        // When
        void action() => resourceState.AsPulling();

        // Then
        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void Pending_ReturnsPulling()
    {
        // Given
        ResourceState resourceState = new(true); ;

        // When
        ResourceState result = resourceState.AsPulling();

        // Then
        Assert.True(result.IsPulling);
        Assert.True(result.IsPullable);
        Assert.True(result.IsPullNeeded);
        Assert.False(result.IsPulled);
    }

    [Fact]
    public void Pulling_ReturnsPulling()
    {
        // Given
        ResourceState resourceState = new ResourceState(true).AsPulling();

        // When
        ResourceState result = resourceState.AsPulling();

        // Then
        Assert.True(result.IsPulling);
        Assert.True(result.IsPullable);
        Assert.True(result.IsPullNeeded);
        Assert.False(result.IsPulled);
    }
}
