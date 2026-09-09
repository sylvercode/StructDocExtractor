namespace Sylvercode.StructDocExtractor.Serialization;

/// <summary>Holds the outcome of serializing a single node, indicating whether content was directly emitted by the serializer.</summary>
public class NodeSerializationResult
{
    /// <summary>Gets or sets a value indicating whether the serializer wrote the node's content directly, bypassing automatic child recursion.</summary>
    public bool ContentSerialized { get; set; }
}
