using System.Globalization;

namespace Sylvercode.StructDocExtractor.Metadata;

public class ResourceMetadata(string name, object? value)
{
    public string Name { get; } = name;
    public TValue? GetValue<TValue>() => (TValue?)Convert.ChangeType(value, typeof(TValue), CultureInfo.InvariantCulture);
    public string? GetStrValue() => (string?)Convert.ChangeType(value, TypeCode.String, CultureInfo.InvariantCulture);
}
