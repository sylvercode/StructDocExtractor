namespace Sylvercode.StructDocExtractor.Serialization;

public class SerializerTaskParentInfo(SerializerTask parent)
{
    public SerializerTask Parent { get; set; } = parent;
    public SerializerTask? PreviousSibling { get; set; }
    public SerializerTask? NextSibling { get; set; }

    public bool IsFirstChild => PreviousSibling is null;
    public bool IsLastChild => NextSibling is null;
}
