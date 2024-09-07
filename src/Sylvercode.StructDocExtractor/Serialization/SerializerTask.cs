namespace Sylvercode.StructDocExtractor.Serialization;

public class SerializerTask(object Data, SerializerTaskParentInfo? ParentInfo = null)
{
    public object Data { get; } = Data;
    public SerializerTaskParentInfo? ParentInfo { get; } = ParentInfo;

    public ISerializer? Serializer { get; set; }
}
