using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Extraction;

/// <summary>Immutable snapshot of a single task's outcome, capturing result type, the produced node, and discriminator context.</summary>
/// <param name="resultType">The outcome classification for this task.</param>
/// <param name="srcNode">The structural node produced by the factory, or <see langword="null"/> when none was created.</param>
/// <param name="dataDiscriminator">The discriminator value that identified the matching factory, or <see langword="null"/>.</param>
/// <param name="nodeFactoryProvider">The scoped factory provider set by the task result for child tasks, or <see langword="null"/>.</param>
public class ExtractionTaskResult(TaskResultType resultType, IStructDocNode? srcNode = null, object? dataDiscriminator = null, object? nodeFactoryProvider = null)
{
    /// <summary>Initializes a new instance of <see cref="ExtractionTaskResult"/> from the fields of an <see cref="IProcessTaskResult"/>.</summary>
    /// <param name="p">The process task result whose fields are copied.</param>
    public ExtractionTaskResult(IProcessTaskResult p) : this(p.ResultType, p.SrcNode, p.DataDiscriminator, p.NodeFactoryProvider) { }
    /// <summary>Gets the outcome classification for this task.</summary>
    public TaskResultType ResultType => resultType;
    /// <summary>Gets the structural node produced by the factory, or <see langword="null"/> when none was created.</summary>
    public IStructDocNode? SrcNode => srcNode;
    /// <summary>Gets the discriminator value that identified the matching factory, or <see langword="null"/>.</summary>
    public object? DataDiscriminator => dataDiscriminator;
    /// <summary>Gets the scoped factory provider to use for child tasks, or <see langword="null"/> when the default remains in effect.</summary>
    public object? NodeFactoryProvider => nodeFactoryProvider;
}
