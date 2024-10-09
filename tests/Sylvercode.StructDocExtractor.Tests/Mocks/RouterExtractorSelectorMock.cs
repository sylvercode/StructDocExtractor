using Sylvercode.StructDocExtractor.Extraction;

namespace Sylvercode.StructDocExtractor.Tests.Mocks;

public class RouterExtractorSelectorMock(string key = "") : IRouterExtractorSelector<string>
{
    public string Key => key;

    public string? Match(string data)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        var parts = data.Split(':');
        if (parts.Length != 2)
            throw new ArgumentException("Data must be in the format 'key:value'.", nameof(data));

        return parts[0] == Key ? parts[1] : null;
    }
}
