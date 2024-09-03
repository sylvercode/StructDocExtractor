
namespace Sylvercode.StructDocExtractor.Model.Init;

public class ParentChildLinkInitializer<TParent, TChild>(TParent parent) :
    IParentChildLinkInitializer<TChild>
    where TParent : class, IStructDocNodeHolderInitializer<TChild>
    where TChild : class, IStructDocNode
{
    private List<TChild>? _children = [];

    public void AddChild(IStructDocNodeInitializer child)
    {
        if (_children is null)
            throw new InvalidOperationException($"{nameof(InitializeParentChildLink)} Have been already called.");

        child.CheckParentTypeOrThrow(parent);
        _children.Add(parent.CastAsChildTypeOrThrow(child));
    }

    public IEnumerable<TChild> ChildrenToAdd()
        => _children ?? Enumerable.Empty<TChild>();

    public void InitializeParentChildLink()
    {
        if (_children is null)
            throw new InvalidOperationException($"{nameof(InitializeParentChildLink)} Have been already called.");

        parent.SetContent(_children);
        _children.ForEach(c => ((IStructDocNodeInitializer)c).SetParent(parent));
        _children = null;
    }
}
