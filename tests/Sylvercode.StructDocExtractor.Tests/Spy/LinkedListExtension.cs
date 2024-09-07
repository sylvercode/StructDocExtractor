using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Tests.Spy;

public static class LinkedListExtension
{
    public static void AddEntry(this List<SpyStructDocNodeSerializerEntry> entriesLog, ISpySerializer serializer, string methodName, params IStructDocNode?[] parameters)
        => entriesLog.Add(new SpyStructDocNodeSerializerEntry(serializer, methodName, parameters.Select(p => p?.Id)));
}
