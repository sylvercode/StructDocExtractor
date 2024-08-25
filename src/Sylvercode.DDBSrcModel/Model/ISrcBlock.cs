namespace Sylvercode.DDBSrcModel.Model;

public interface ISrcBlock : ISrcNode, ISrcNodeHolder
{

}

public interface ISrcBlock<out P, out C> : ISrcBlock, ISrcNode<P>, ISrcNodeHolder<C>
    where P : class, ISrcNodeHolder
    where C : class, ISrcNode
{

}
