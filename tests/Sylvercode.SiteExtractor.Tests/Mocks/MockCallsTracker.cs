namespace Sylvercode.SiteExtractor.Tests.Mocks;

/// <summary>Helper that records method invocation order and arguments across mock objects.</summary>
public class MockCallTracker
{
    /// <summary>Immutable record of a single tracked method call.</summary>
    public class Entry(object caller, string methodName, object?[] arguments)
    {
        /// <summary>Gets the object instance on which the method was called.</summary>
        public object Caller { get; } = caller;

        /// <summary>Gets the name of the method that was called.</summary>
        public string MethodName { get; } = methodName;

        /// <summary>Gets the arguments passed to the method at the time of the call.</summary>
        public object?[] Arguments { get; } = arguments;
    }

    /// <summary>Gets the ordered list of all recorded call entries.</summary>
    public List<Entry> Calls { get; } = [];

    /// <summary>Records a method call from the specified caller with its arguments.</summary>
    /// <param name="caller">The object instance invoking the method.</param>
    /// <param name="methodName">The name of the method being called.</param>
    /// <param name="arguments">The arguments passed to the method.</param>
    public void TrackCall(object caller, string methodName, object?[] arguments)
    {
        Calls.Add(new Entry(caller, methodName, arguments));
    }
}
