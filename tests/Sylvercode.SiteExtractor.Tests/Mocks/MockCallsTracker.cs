namespace Sylvercode.SiteExtractor.Tests.Mocks;

public class MockCallTracker
{
    public class Entry(object caller, string methodName, object?[] arguments)
    {
        public object Caller { get; } = caller;
        public string MethodName { get; } = methodName;
        public object?[] Arguments { get; } = arguments;
    }

    public List<Entry> Calls { get; } = [];

    public void TrackCall(object caller, string methodName, object?[] arguments)
    {
        Calls.Add(new Entry(caller, methodName, arguments));
    }
}
