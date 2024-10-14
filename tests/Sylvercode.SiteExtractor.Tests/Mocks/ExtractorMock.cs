using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Sylvercode.SiteExtractor.Tests.Stubs;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Metadatas;

namespace Sylvercode.SiteExtractor.Tests.Mocks;

public class ExtractorMock(MockCallTracker tracker, bool extractNothing = false) : IExtractor<Uri>
{
    public const string MetaKey = "meta";
    public ExtractionResult Extract([DisallowNull] Uri data, IObserver<ExtractionTask>? observer = null)
    {
        tracker.TrackCall(this, nameof(Extract), [data, observer]);

        List<(Uri resUri, Uri refUri)> references = GetUriReference(data);
        MetadataDictionary metadatas = new(GetUriData(data, MetaKey)
            .Select(kv => new KeyValuePair<string, object?>(kv.Key, kv.Value)));
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
            result.Metadatas.CopyMetadataFrom(metadatas);
            result.StructDocNodes.Add(new UriNode(data.ToString()));
            result.Summery.CountTaskResult(TaskResultType.Success);
        }

        return result;
    }

    private static List<(Uri resUri, Uri refUri)> GetUriReference(Uri source)
    {
        Uri baseUri = new(source.GetLeftPart(UriPartial.Authority));
        List<(Uri resUri, Uri refUri)> result = [];
        Dictionary<string, string?> refData = GetUriData(source, "referencer");

        foreach (var (key, value) in refData)
            result.Add((new Uri(baseUri, key), new Uri(baseUri, value)));

        return result;
    }

    private static Dictionary<string, string?> GetUriData(Uri source, string keyStart)
    {
        Dictionary<string, string?> result = [];
        NameValueCollection query = System.Web.HttpUtility.ParseQueryString(source.Query);

        foreach (var key in query.AllKeys ?? [])
        {
            if (key?.StartsWith(keyStart) ?? false)
                result.Add(key, query[key]);
        }

        return result;
    }
}
