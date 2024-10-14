using System.Globalization;

namespace Sylvercode.StructDocExtractor.Metadatas;

public class Metadata(string name, object? value)
{
    public string Name { get; } = name;
    public object? Value { get; } = value;
    public TValue? GetValue<TValue>() => (TValue?)Convert.ChangeType(Value, typeof(TValue), CultureInfo.InvariantCulture);
    public string? GetStrValue() => (string?)Convert.ChangeType(Value, TypeCode.String, CultureInfo.InvariantCulture);
}
