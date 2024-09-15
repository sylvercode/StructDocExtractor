using System.Runtime.CompilerServices;

namespace Sylvercode.StructDocExtractor.Tests.Spy;

public static class ISpySerializerExtension
{
    public static void Log(this ISpySerializer serializer, object?[] parameters, [CallerMemberName] string methodName = "")
        => serializer.EntriesLog.Add(new SpyStructDocNodeSerializerEntry(serializer, methodName, parameters));
}
