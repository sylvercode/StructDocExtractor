namespace Sylvercode.DDBSrcModel.Extraction;

public interface IDataPreviewProvider<TData>
{
    string GetPreview(TData data);
}
