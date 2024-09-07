using System.Diagnostics.CodeAnalysis;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

public class StructDocSerializerExecutor(StreamWriter stream, object rootData, ISerializerProvider serializerProvider)
{
    private readonly LinkedList<SerializerTask> _pendingTasks = new([new SerializerTask(rootData)]);

    private bool HasPendingTasks => _pendingTasks.Count > 0;

    public void ExecuteTasks()
    {
        while (HasPendingTasks)
        {
            var firstListNode = _pendingTasks.First
                              ?? throw new InvalidOperationException("No task to process");

            _pendingTasks.Remove(firstListNode);

            ExecuteTask(firstListNode.Value);
        }
    }

    private void ExecuteTask(SerializerTask task)
    {
        task.Serializer = serializerProvider.GetSerializerFor(task.Data);
        if (task.Serializer is null)
        {
            task.OnToProcess(stream);
            task.HasChildTasks = false;
            task.OnProcessed(stream);
            throw new InvalidOperationException($"No serializer found for {task.Data.GetType()}");
        }

        task.OnToProcess(stream);

        task.Serializer.Serialize(task.Data, stream);

        bool hasChildTasks = false;
        if (task.Data is IStructDocNodeHolder childHolderData)
        {
            SerializerTask? nextTask = null;
            foreach (var child in childHolderData.Content.Reverse())
            {
                hasChildTasks = true;
                var childTask = new SerializerTask(child, new SerializerTaskParentInfo(task));
                _pendingTasks.AddFirst(childTask);

                if (nextTask is not null)
                {
                    nextTask.ParentInfo!.PreviousSibling = childTask;
                    childTask.ParentInfo!.NextSibling = nextTask;
                }
                nextTask = childTask;
            }
        }
        task.HasChildTasks = hasChildTasks;

        task.OnProcessed(stream);
    }
}
