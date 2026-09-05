using Sylvercode.SiteExtractor.Resources;

namespace Sylvercode.SiteExtractor.Tests;

/// <summary>Tests for <see cref="ResourceState"/> initial state after construction.</summary>
public class ResourceStateTests_IsPullable
{
    /// <summary>Verifies that a pullable resource starts in the pending state awaiting a pull.</summary>
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
    /// <summary>Verifies that a non-pullable resource is already considered pulled from the start.</summary>
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

/// <summary>Tests for <see cref="ResourceState.AsPulled"/> state-machine transitions.</summary>
public class ResourceStateTests_AsPulled
{
    /// <summary>Verifies that transitioning a non-pullable resource to pulled throws an <see cref="InvalidOperationException"/>.</summary>
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

    /// <summary>Verifies that transitioning an already-pulled resource to pulled again throws an <see cref="InvalidOperationException"/>.</summary>
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

    /// <summary>Verifies that a pending pullable resource transitions correctly to the pulled state.</summary>
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

    /// <summary>Verifies that a currently-pulling resource transitions correctly to the pulled state.</summary>
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

/// <summary>Tests for <see cref="ResourceState.AsPulling"/> state-machine transitions.</summary>
public class ResourceStateTests_AsPulling
{
    /// <summary>Verifies that transitioning a non-pullable resource to pulling throws an <see cref="InvalidOperationException"/>.</summary>
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

    /// <summary>Verifies that transitioning an already-pulled resource to pulling throws an <see cref="InvalidOperationException"/>.</summary>
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

    /// <summary>Verifies that a pending pullable resource transitions correctly to the pulling state.</summary>
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

    /// <summary>Verifies that a resource already in the pulling state can remain in the pulling state.</summary>
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
