using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Sylvercode.StructDocExtractor.Model.Init;

/// <summary>Provides extension methods for <see cref="IStructDocNodeHolderInitializer{TChild}"/> to simplify tree construction and content initialization.</summary>
public static class IStructDocNodeHolderInitializerExtension
{
    /// <summary>Creates a new typed parent/child link initializer from a typed holder initializer.</summary>
    /// <typeparam name="TChild">The type of child nodes managed by the holder.</typeparam>
    /// <param name="holderInitializer">The holder to create the link initializer for.</param>
    /// <returns>A new <see cref="IParentChildLinkInitializer{TChild}"/> scoped to <paramref name="holderInitializer"/>.</returns>
    public static IParentChildLinkInitializer<TChild> NewParentChildLinkInitializer<TChild>(this IStructDocNodeHolderInitializer<TChild> holderInitializer)
            where TChild : class, IStructDocNode
        => holderInitializer.NewParentChildLinkInitializer();

    /// <summary>Fluent builder for accumulating typed child nodes before assigning them to a holder.</summary>
    /// <typeparam name="TChild">The type of child nodes this builder accepts.</typeparam>
    public class ContentBuilder<TChild> : IEnumerable<TChild>
            where TChild : class, IStructDocNode
    {
        readonly List<TChild> _content = [];

        /// <summary>Adds <paramref name="item"/> to the builder and returns it.</summary>
        /// <typeparam name="T">The concrete child type, which must satisfy <typeparamref name="TChild"/>.</typeparam>
        /// <param name="item">The child node to add.</param>
        /// <returns>The same <paramref name="item"/> for fluent chaining.</returns>
        [return: NotNull]
        public T Add<T>([NotNull] T item)
            where T : class, TChild, IStructDocNode
        {
            _content.Add(item);
            return item;
        }

        /// <summary>Creates a default-constructed instance of <typeparamref name="T"/>, adds it to the builder, and returns it.</summary>
        /// <typeparam name="T">The concrete child type to create; must have a parameterless constructor.</typeparam>
        /// <returns>The newly created child node.</returns>
        [return: NotNull]
        public T Add<T>()
            where T : class, TChild, IStructDocNode, new()
        {
            return Add(new T());
        }

        /// <inheritdoc/>
        public IEnumerator<TChild> GetEnumerator()
        {
            return ((IEnumerable<TChild>)_content).GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_content).GetEnumerator();
        }
    }

    /// <summary>Initializes the holder's content by invoking <paramref name="init"/> on a <see cref="ContentBuilder{TChild}"/> and committing the result.</summary>
    /// <typeparam name="TChild">The type of child nodes.</typeparam>
    /// <param name="holderInitializer">The holder to initialize.</param>
    /// <param name="init">An action that adds child nodes to the builder.</param>
    public static void InitWithContent<TChild>(this IStructDocNodeHolderInitializer<TChild> holderInitializer, Action<ContentBuilder<TChild>> init)
            where TChild : class, IStructDocNode
    {
        ContentBuilder<TChild> content = [];
        init(content);
        holderInitializer.InternalInitWithContent(content);
    }

    /// <summary>Initializes the holder's content from a parameter array of child nodes.</summary>
    /// <typeparam name="TChild">The type of child nodes.</typeparam>
    /// <param name="holderInitializer">The holder to initialize.</param>
    /// <param name="content">The child nodes to assign.</param>
    public static void InitWithContent<TChild>(this IStructDocNodeHolderInitializer<TChild> holderInitializer, params TChild[] content)
            where TChild : class, IStructDocNode
        => holderInitializer.InternalInitWithContent(content);

    private static void InternalInitWithContent<TChild>(this IStructDocNodeHolderInitializer<TChild> holderInitializer, IEnumerable<TChild> content)
            where TChild : class, IStructDocNode
    {
        IParentChildLinkInitializer<TChild>? linker = null;
        try
        {
            linker = holderInitializer.NewParentChildLinkInitializer();
            foreach (var c in content)
                ((IParentChildLinkInitializer)linker).AddChild((IStructDocNodeInitializer)c);
        }
        finally
        {
            linker?.InitializeParentChildLink();
        }
    }

    /// <summary>Initializes the holder's content to an empty collection.</summary>
    /// <param name="holderInitializer">The holder to initialize as empty.</param>
    public static void InitContentAsEmpty(this IStructDocNodeHolderInitializer holderInitializer)
    {
        holderInitializer.InitContentAsEmpty();
    }
}
