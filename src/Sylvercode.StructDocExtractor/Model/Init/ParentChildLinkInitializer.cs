
namespace Sylvercode.StructDocExtractor.Model.Init;

/// <summary>Default implementation of <see cref="IParentChildLinkInitializer{TChild}"/> that collects children and atomically wires parent/child relationships.</summary>
/// <typeparam name="TParent">The holder initializer type that will receive the child content.</typeparam>
/// <typeparam name="TChild">The type of child nodes being linked.</typeparam>
/// <remarks>
/// Validates each child against the holder's accepted type before registering it.
/// Once <see cref="InitializeParentChildLink"/> is called, further calls to <see cref="AddChild"/> throw <see cref="InvalidOperationException"/>.
/// </remarks>
public class ParentChildLinkInitializer<TParent, TChild>(TParent parent) :
    IParentChildLinkInitializer<TChild>
    where TParent : class, IStructDocNodeHolderInitializer<TChild>
    where TChild : class, IStructDocNode
{
    private List<TChild>? _children = [];

    /// <summary>Validates and registers <paramref name="child"/> for linking; throws <see cref="InvalidOperationException"/> if already committed.</summary>
    /// <param name="child">The child node initializer to register.</param>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="InitializeParentChildLink"/> has already been called.</exception>
    public void AddChild(IStructDocNodeInitializer child)
    {
        if (_children is null)
            throw new InvalidOperationException($"{nameof(InitializeParentChildLink)} Have been already called.");

        child.CheckParentTypeOrThrow(parent);
        _children.Add(parent.CastAsChildTypeOrThrow(child));
    }

    /// <inheritdoc/>
    public IEnumerable<TChild> ChildrenToAdd()
        => _children ?? Enumerable.Empty<TChild>();

    /// <summary>Assigns accumulated children as the holder's content and sets the parent on each child, then prevents further additions.</summary>
    /// <exception cref="InvalidOperationException">Thrown when called more than once.</exception>
    public void InitializeParentChildLink()
    {
        if (_children is null)
            throw new InvalidOperationException($"{nameof(InitializeParentChildLink)} Have been already called.");

        parent.SetContent(_children);
        _children.ForEach(c => ((IStructDocNodeInitializer)c).SetParent(parent));
        _children = null;
    }
}
