using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Tests.Stubs;

public class BasicProcessTaskResult(IStructDocNode? node = null)
    : ProcessTaskResult<string, BasicNodeDiscriminator>(node)
{
}
