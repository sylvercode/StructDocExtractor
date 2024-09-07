using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.StructDocExtractor.Tests.Spy;

public class SpyStructDocNodeSerializerEntry(
    ISerializer serializer,
    SpyStructDocNodeSerializerEntry.Methods methodName,
    IEnumerable<object?> parameters)
{
    public enum Methods
    {
        Serialize,
        OnBeforeChildSerialize,
        OnAfterChildSerialize,
        OnBeforeFirstChildSerialize,
        OnBetweenSiblingSerialize,
        OnAfterLastChildSerialize,
        OnNoChildSerialize
    }

    public static Methods GetMethod(string methodName)
        => methodName switch
        {
            nameof(Methods.Serialize) => Methods.Serialize,
            nameof(Methods.OnBeforeChildSerialize) => Methods.OnBeforeChildSerialize,
            nameof(Methods.OnAfterChildSerialize) => Methods.OnAfterChildSerialize,
            nameof(Methods.OnBeforeFirstChildSerialize) => Methods.OnBeforeFirstChildSerialize,
            nameof(Methods.OnBetweenSiblingSerialize) => Methods.OnBetweenSiblingSerialize,
            nameof(Methods.OnAfterLastChildSerialize) => Methods.OnAfterLastChildSerialize,
            nameof(Methods.OnNoChildSerialize) => Methods.OnNoChildSerialize,
            _ => throw new ArgumentException($"Unknown method name: {methodName}")
        };

    public SpyStructDocNodeSerializerEntry(
        ISerializer serializer,
        string methodName,
        IEnumerable<object?> parameters)
        : this(serializer, GetMethod(methodName), parameters)
    {
    }

    public ISerializer Serializer { get; } = serializer;
    public Methods Method { get; } = methodName;
    public List<object?> ParamsId { get; } = [.. parameters];

    public void Assert(SpyStructDocNodeSerializerEntry expected)
    {
        Xunit.Assert.Same(expected.Serializer, Serializer);
        Xunit.Assert.Equal(expected.Method, Method);
        Xunit.Assert.Collection(ParamsId, AsAsserter(expected.ParamsId));
    }

    private static Action<object?>[] AsAsserter(List<object?> parameters)
        => parameters.Select(p => new Action<object?>(expected => Xunit.Assert.Same(expected, p))).ToArray();
}
