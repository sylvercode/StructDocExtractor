using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Sylvercode.DDBSrcModel.Model.Init;

public static class ISrcNodeHolderInitializerExtetnion
{
    public static IParentChildLinkIntializer<C> NewParentChildLinkIntializer<C>(this ISrcNodeHolderInitializer<C> holderInitializer)
            where C : class, ISrcNode
        => holderInitializer.NewParentChildLinkIntializer();

    public class ContentBuilder<C> : IEnumerable<C>
            where C : class, ISrcNode
    {
        readonly List<C> _content = [];

        [return: NotNull]
        public T Add<T>([NotNull] T item)
            where T : class, C, ISrcNode
        {
            _content.Add(item);
            return item;
        }

        public IEnumerator<C> GetEnumerator()
        {
            return ((IEnumerable<C>)_content).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_content).GetEnumerator();
        }
    }

    public static void InitWithContent<C>(this ISrcNodeHolderInitializer<C> holderInitializer, Action<ContentBuilder<C>> init)
            where C : class, ISrcNode
    {
        ContentBuilder<C> content = [];
        init(content);
        holderInitializer.InternalInitWithContent(content);
    }

    public static void InitWithContent<C>(this ISrcNodeHolderInitializer<C> holderInitializer, params C[] content)
            where C : class, ISrcNode
        => holderInitializer.InternalInitWithContent(content);

    private static void InternalInitWithContent<C>(this ISrcNodeHolderInitializer<C> holderInitializer, IEnumerable<C> content)
            where C : class, ISrcNode
    {
        IParentChildLinkIntializer<C>? linker = null;
        try
        {
            linker = holderInitializer.NewParentChildLinkIntializer();
            foreach (var c in content)
                ((IParentChildLinkIntializer)linker).AddChild((ISrcNodeIntializer)c);
        }
        finally
        {
            linker?.InitializeParentChildLink();
        }
    }

    public static void InitContentAsEmpty(this ISrcNodeHolderInitializer holderInitializer)
    {
        holderInitializer.InitContentAsEmpty();
    }
}
