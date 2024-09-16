namespace Sylvercode.StructDocExtractor.Extraction.PreviewProvider;


public interface IDataPreviewProvider
{
    string GetPreview(object? data);
}

public interface IDataPreviewProvider<TData> : IDataPreviewProvider
{
    string GetPreview(TData? data);
    string IDataPreviewProvider.GetPreview(object? data) => GetPreview((TData?)data);
}
