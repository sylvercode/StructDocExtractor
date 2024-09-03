using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Sylvercode.StructDocExtractor.Model.Init;

public static class IStructDocNodeHolderInitializerExtension
{
    public static IParentChildLinkInitializer<TChild> NewParentChildLinkInitializer<TChild>(this IStructDocNodeHolderInitializer<TChild> holderInitializer)
            where TChild : class, IStructDocNode
        => holderInitializer.NewParentChildLinkInitializer();

    public class ContentBuilder<TChild> : IEnumerable<TChild>
            where TChild : class, IStructDocNode
    {
        readonly List<TChild> _content = [];

        [return: NotNull]
        public T Add<T>([NotNull] T item)
            where T : class, TChild, IStructDocNode
        {
            _content.Add(item);
            return item;
        }

        public IEnumerator<TChild> GetEnumerator()
        {
            return ((IEnumerable<TChild>)_content).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_content).GetEnumerator();
        }
    }

    public static void InitWithContent<TChild>(this IStructDocNodeHolderInitializer<TChild> holderInitializer, Action<ContentBuilder<TChild>> init)
            where TChild : class, IStructDocNode
    {
        ContentBuilder<TChild> content = [];
        init(content);
        holderInitializer.InternalInitWithContent(content);
    }

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

    public static void InitContentAsEmpty(this IStructDocNodeHolderInitializer holderInitializer)
    {
        holderInitializer.InitContentAsEmpty();
    }
}
