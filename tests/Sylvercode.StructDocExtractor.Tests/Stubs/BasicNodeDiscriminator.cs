namespace Sylvercode.StructDocExtractor.Tests.Stubs;

public readonly struct BasicNodeDiscriminator(string id, string[] data)
{
    public BasicNodeDiscriminator() : this(id: "") { }
    public BasicNodeDiscriminator(string id) : this(id, data: []) { }

    public readonly string Id = id;
    public readonly string[] Data = data;

    public const string DefaultId = nameof(DefaultId);
    public const string DefaultParentId = nameof(DefaultParentId);
    public const string WrongId = nameof(WrongId);

    public const string DataValue1 = nameof(DataValue1);
    public const string DataValue2 = nameof(DataValue2);
    public const string ParentDataValue1 = nameof(ParentDataValue1);
    public const string ParentDataValue2 = nameof(ParentDataValue2);
    public const string WrongDataValue = nameof(WrongDataValue);

    public static BasicNodeDiscriminator NewDefault() => new(DefaultId, [DataValue1, DataValue2]);
    public static BasicNodeDiscriminator NewWrongId() => new(WrongId, [DataValue1, DataValue2]);
    public static BasicNodeDiscriminator NewWrongData() => new(DefaultId, [WrongDataValue]);
    public static BasicNodeDiscriminator NewWrongIdAndData() => new(WrongId, [WrongDataValue]);

    public static BasicNodeDiscriminator NewDefaultParent() => new(DefaultParentId, [ParentDataValue1, ParentDataValue2]);
    public static BasicNodeDiscriminator NewParentWrongId() => new(WrongId, [ParentDataValue1, ParentDataValue2]);
    public static BasicNodeDiscriminator NewParentWrongData() => new(DefaultParentId, [WrongDataValue]);
    public static BasicNodeDiscriminator NewParentWrongIdAndData() => new(WrongId, [WrongDataValue]);
}
