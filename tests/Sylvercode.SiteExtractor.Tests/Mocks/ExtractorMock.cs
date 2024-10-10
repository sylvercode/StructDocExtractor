using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Sylvercode.SiteExtractor.Tests.Stubs;
using Sylvercode.StructDocExtractor.Extraction;

namespace Sylvercode.SiteExtractor.Tests.Mocks;

public class ExtractorMock(MockCallTracker tracker, bool extractNothing = false) : IExtractor<Uri>
{
    public ExtractionResult Extract([DisallowNull] Uri data, IObserver<ExtractionTask>? observer = null)
    {

        tracker.TrackCall(this, nameof(Extract), [data, observer]);

        List<(Uri resUri, Uri refUri)> references = GetUriReference(data);
        foreach ((Uri resUri, Uri refUri) in references)
        {
            ExtractionTask task = new(resUri);

            ProcessTaskResult<Uri, Uri> taskResult = new(
                TaskResultType.Success,
                new UriReferenceNode(resUri.ToString(), refUri.ToString()));
            task.SetResult(taskResult);

            observer?.OnNext(task);
        }

        ExtractionResult result = new();
        if (extractNothing)
            result.Summery.CountTaskResult(TaskResultType.Skipped);
        else
        {
            result.StructDocNodes.Add(new UriNode(data.ToString()));
            result.Summery.CountTaskResult(TaskResultType.Success);
        }

        return result;
    }

    private static List<(Uri resUri, Uri refUri)> GetUriReference(Uri source)
    {
        Uri baseUri = new(source.GetLeftPart(UriPartial.Authority));
        List<(Uri resUri, Uri refUri)> result = [];
        NameValueCollection query = System.Web.HttpUtility.ParseQueryString(source.Query);

        foreach (var key in query.AllKeys ?? [])
        {
            var value = query[key];
            if (value is not null)
                result.Add((new Uri(baseUri, key), new Uri(baseUri, value)));
        }

        return result;
    }
}
