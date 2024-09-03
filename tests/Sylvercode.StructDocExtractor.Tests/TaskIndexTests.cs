using Sylvercode.StructDocExtractor.Extraction;

namespace Sylvercode.StructDocExtractor.Tests;

public class TaskIndexTests_ctor
{
    [Fact]
    public void Empty_ResultZeroIndex()
    {
        // Given

        // When
        TaskIndex result = new();

        // Then
        Assert.False(result.IsSubTaskIndex);
        Assert.Single(result, 0);
    }

    [Fact]
    public void SingleIndex_ResultIndex()
    {
        // Given
        int index = 2;

        // When
        TaskIndex result = new(index);

        // Then
        Assert.False(result.IsSubTaskIndex);
        Assert.Single(result, index);
    }

    [Fact]
    public void WithParentAndIndex_ResultParnentIndexAndIndex()
    {
        // Given
        int parentIndex = 2;
        TaskIndex parent = new(parentIndex);
        int index = 5;

        // When
        TaskIndex result = new(parent, index);

        // Then
        Assert.True(result.IsSubTaskIndex);
        Assert.Collection(result,
                          i => Assert.Equal(parentIndex, i),
                          i => Assert.Equal(index, i));
    }

    [Fact]
    public void WithParentAndNoIndex_ResultParentIndexAndZero()
    {
        // Given
        int parentIndex = 2;
        TaskIndex parent = new(parentIndex);

        // When
        TaskIndex result = new(parent);

        // Then
        Assert.True(result.IsSubTaskIndex);
        Assert.Collection(result,
                          i => Assert.Equal(parentIndex, i),
                          i => Assert.Equal(0, i));
    }

    [Fact]
    public void WithParentWithParentAndIndex_ResultGrandParentAndParentAndIndex()
    {
        // Given
        int grandParentIndex = 7;
        TaskIndex grandParent = grandParentIndex; // implicite conversion
        int parentIndex = 2;
        TaskIndex parent = new(grandParent, parentIndex);
        int index = 9;

        // When
        TaskIndex result = new(parent, index);

        // Then
        Assert.True(result.IsSubTaskIndex);
        Assert.Collection(result,
                          i => Assert.Equal(grandParentIndex, i),
                          i => Assert.Equal(parentIndex, i),
                          i => Assert.Equal(index, i));
    }
}

public class TaskIndexTests_NewSubTaskIndex
{
    [Fact]
    public void SingleIndexAndNoSubIndex_ReturnsSubZero()
    {
        // Given
        int parentIndex = 6;
        TaskIndex parent = parentIndex;

        // When
        TaskIndex result = parent.NewSubTaskIndex();

        // Then
        Assert.True(result.IsSubTaskIndex);
        Assert.Collection(result,
                          i => Assert.Equal(parentIndex, i),
                          i => Assert.Equal(0, i));
    }

    [Fact]
    public void SingleIndexAndSubIndex_ReturnsSubIndex()
    {
        // Given
        int parentIndex = 6;
        TaskIndex parent = parentIndex;
        int index = 8;

        // When
        TaskIndex result = parent.NewSubTaskIndex(index);

        // Then
        Assert.True(result.IsSubTaskIndex);
        Assert.Collection(result,
                          i => Assert.Equal(parentIndex, i),
                          i => Assert.Equal(index, i));
    }

    [Fact]
    public void DoubleIndexAndSubIndex_ReturnsSubIndex()
    {
        // Given
        int grandParentIndex = 4;
        TaskIndex grandParent = grandParentIndex;
        int parentIndex = 6;
        TaskIndex parent = new(grandParent, parentIndex);
        int index = 8;

        // When
        TaskIndex result = parent.NewSubTaskIndex(index);

        // Then
        Assert.True(result.IsSubTaskIndex);
        Assert.Collection(result,
                          i => Assert.Equal(grandParentIndex, i),
                          i => Assert.Equal(parentIndex, i),
                          i => Assert.Equal(index, i));
    }
}

public class TaskIndexTests_ParentTaskIndex
{
    [Fact]
    public void TripleIndex_ReturnDoulbeIndex()
    {
        // Given
        TaskIndex index = new(new(7, 3), 5);

        // When
        TaskIndex result = index.ParentTaskIndex();

        // Then
        Assert.True(result.IsSubTaskIndex);
        Assert.Collection(result,
                          i => Assert.Equal(7, i),
                          i => Assert.Equal(3, i));
    }
    [Fact]
    public void NoSubIndex_Throws()
    {
        // Given
        TaskIndex index = 5;

        // When-Then
        Assert.Throws<InvalidOperationException>(() => index.ParentTaskIndex());
    }
}

public class TaskIndexTests_Increment
{
    [Fact]
    public void TripleIndex_LastIncremented()
    {
        // Given
        TaskIndex index = new(new(7, 3), 5);

        // When
        TaskIndex result = index.Increment();

        // Then
        Assert.True(result.IsSubTaskIndex);
        Assert.Collection(result,
                          i => Assert.Equal(7, i),
                          i => Assert.Equal(3, i),
                          i => Assert.Equal(6, i));
    }
    [Fact]
    public void SingleIndex_LastIncremented()
    {
        // Given
        TaskIndex index = 5;

        // When
        TaskIndex result = index.Increment();

        // Then
        Assert.False(result.IsSubTaskIndex);
        Assert.Collection(result,
                          i => Assert.Equal(6, i));
    }
}

public class TaskIndexTests_IComparable
{
    [Fact]
    public void NotEqual()
    {
        // Given
        TaskIndex left = new(3, 5);
        TaskIndex right = new(3, 9);

        // When

        // Then
        Assert.True(int.IsNegative(left.CompareTo(right)));
        Assert.True(int.IsPositive(right.CompareTo(left)));
    }
    [Fact]
    public void IsEqual()
    {
        // Given
        TaskIndex left = new(3, 7);
        TaskIndex right = new(3, 7);

        // When

        // Then
        Assert.Equal(0, left.CompareTo(right));
        Assert.Equal(0, right.CompareTo(left));
    }
}
