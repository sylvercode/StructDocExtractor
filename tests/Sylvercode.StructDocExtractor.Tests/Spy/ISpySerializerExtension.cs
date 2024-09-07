using System.Runtime.CompilerServices;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Tests.Spy;

public static class ISpySerializerExtension
{
    public static void Log(this ISpySerializer serializer, IStructDocNode?[] parameters, [CallerMemberName] string methodName = "")
        => serializer.EntriesLog.Add(new SpyStructDocNodeSerializerEntry(serializer, methodName, parameters.Select(p => p?.Id)));
}
