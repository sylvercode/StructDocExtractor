using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.Tests.Stubs;

public class BasicSrcNode(string id = BasicSrcNode.DefaultId, string data = "") : BaseStructDocNode(id)
{
    public const string DefaultId = nameof(DefaultId);

    public string Data { get; } = data;
}
