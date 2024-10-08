using Sylvercode.StructDocExtractor.Extraction;

namespace Sylvercode.StructDocExtractor.Tests.Mocks;

public class RouterExtractorSelectorMock(bool result = false) : IRouterExtractorSelector<object>
{
    public object? Match(object data) => result ? data : null;
}
