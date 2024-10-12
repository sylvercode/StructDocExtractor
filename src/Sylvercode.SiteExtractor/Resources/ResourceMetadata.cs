namespace Sylvercode.SiteExtractor.Resources;

public class ResourceMetadata(string name, object? value)
{
    public string Name { get; } = name;
    public TValue? GetValue<TValue>() => (TValue?)Convert.ChangeType(value, typeof(TValue));
    public string? GetStrValue() => (string?)Convert.ChangeType(value, TypeCode.String);
}
