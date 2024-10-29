namespace Sylvercode.StructDocExtractor.Model.Init;

public static class IStructDocNodeInitializerExtensions
{
    public static void MakeARoot(this IStructDocNodeInitializer node) => node.SetParent(node);
}
