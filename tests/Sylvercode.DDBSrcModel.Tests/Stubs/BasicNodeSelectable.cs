namespace Sylvercode.DDBSrcModel.Tests.Stubs;

public readonly struct BasicNodeSelectable(string id, string[] data)
{
    public BasicNodeSelectable() : this(id: "", data: []) { }

    public readonly string Id = id;
    public readonly string[] Data = data;

    public const string DefautltId = nameof(DefautltId);
    public const string DefautltParentId = nameof(DefautltParentId);
    public const string WrongId = nameof(WrongId);

    public const string DataValue1 = nameof(DataValue1);
    public const string DataValue2 = nameof(DataValue2);
    public const string ParentDataValue1 = nameof(ParentDataValue1);
    public const string ParentDataValue2 = nameof(ParentDataValue2);
    public const string WrongDataValue = nameof(WrongDataValue);

    public static BasicNodeSelectable NewDefault() => new(DefautltId, [DataValue1, DataValue2]);
    public static BasicNodeSelectable NewWrongId() => new(WrongId, [DataValue1, DataValue2]);
    public static BasicNodeSelectable NewWrongData() => new(DefautltId, [WrongDataValue]);
    public static BasicNodeSelectable NewWrongIdAndData() => new(WrongId, [WrongDataValue]);

    public static BasicNodeSelectable NewDefaultParent() => new(DefautltParentId, [ParentDataValue1, ParentDataValue2]);
    public static BasicNodeSelectable NewParentWrongId() => new(WrongId, [ParentDataValue1, ParentDataValue2]);
    public static BasicNodeSelectable NewParentWrongData() => new(DefautltParentId, [WrongDataValue]);
    public static BasicNodeSelectable NewParentWrongIdAndData() => new(WrongId, [WrongDataValue]);
}
