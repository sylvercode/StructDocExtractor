using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Sylvercode.SiteExtractor.Tests.Stubs;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.SiteExtractor.Tests.Mocks;

public class ExtractorMock(MockCallTracker tracker, bool extractNothing = false) : IExtractor<Uri>
{
    private readonly IChildrenTaskInfoFactory childrenTaskInfoFactory = new ChildrenTaskInfoFactory();

    public ExtractionResult Extract([DisallowNull] Uri data, IObserver<ExtractionTask>? observer = null)
    {

        tracker.TrackCall(this, nameof(Extract), [data, observer]);

        List<Uri> references = GetUriReference(data);
        foreach (var reference in references)
        {
            ExtractionTask task = new(reference);

            ProcessTaskResult<Uri, Uri> taskResult = new(TaskResultType.Success, new UriReferenceNode(reference.ToString()));
            task.SetResult(taskResult, childrenTaskInfoFactory);

            observer?.OnNext(task);
        }

        ExtractionResult.ExtractionSummery summery = new();
        summery.CountTaskResult(extractNothing ? TaskResultType.Skipped : TaskResultType.Success);

        IReadOnlyList<IStructDocNode> srcNodes = extractNothing ? [] : [new UriNode(data.ToString())];

        return new ExtractionResult(summery, srcNodes);
    }

    private static List<Uri> GetUriReference(Uri source)
    {
        Uri baseUri = new(source.GetLeftPart(UriPartial.Authority));
        List<Uri> result = [];
        NameValueCollection query = System.Web.HttpUtility.ParseQueryString(source.Query);

        foreach (var value in query.GetValues("ref") ?? [])
        {
            result.Add(new Uri(baseUri, value));
        }

        return result;
    }
}
