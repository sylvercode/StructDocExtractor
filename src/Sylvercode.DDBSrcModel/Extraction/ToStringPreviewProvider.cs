namespace Sylvercode.DDBSrcModel.Extraction;

public class ToStringPreviewProvider<TData>(int characterCountForPreview = 20) : IDataPreviewProvider<TData>
{

    public string GetPreview(TData data) => data is null ? string.Empty : (data.ToString() ?? "")[..characterCountForPreview];
}
