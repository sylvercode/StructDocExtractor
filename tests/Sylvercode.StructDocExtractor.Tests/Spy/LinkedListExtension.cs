using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Tests.Spy;

public static class ListExtension
{
    public static void AddEntry(
        this List<SpyStructDocNodeSerializerEntry> entriesLog,
        ISpySerializer serializer,
        string methodName,
        params IStructDocNode?[] parameters)
        => entriesLog.Add(new SpyStructDocNodeSerializerEntry(serializer, SpyStructDocNodeSerializerEntry.GetMethod(methodName), parameters.Select(p => p?.Id)));

    public static void AddEntry(
        this List<SpyStructDocNodeSerializerEntry> entriesLog,
        ISpySerializer serializer,
        SpyStructDocNodeSerializerEntry.Methods method,
        params IStructDocNode?[] parameters)
        => entriesLog.Add(new SpyStructDocNodeSerializerEntry(serializer, method, parameters));

    public static Action<SpyStructDocNodeSerializerEntry>[] AsAsserter(this List<SpyStructDocNodeSerializerEntry> entriesLog)
        => entriesLog.Select(expected => new Action<SpyStructDocNodeSerializerEntry>(e => e.Assert(expected))).ToArray();
}
