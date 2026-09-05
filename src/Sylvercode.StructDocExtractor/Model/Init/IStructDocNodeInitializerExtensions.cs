namespace Sylvercode.StructDocExtractor.Model.Init;

/// <summary>Provides extension methods for <see cref="IStructDocNodeInitializer"/> to simplify self-referential root setup.</summary>
public static class IStructDocNodeInitializerExtensions
{
    /// <summary>Configures <paramref name="node"/> as a root by setting its parent to itself.</summary>
    /// <param name="node">The node to promote to root status.</param>
    public static void MakeARoot(this IStructDocNodeInitializer node) => node.SetParent(node);
}
