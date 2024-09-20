namespace Sylvercode.StructDocExtractor.Extraction.PreviewProvider;

public class ToStringPreviewProvider<TData>(int characterCountForPreview = 20) : IDataPreviewProvider<TData>
{

    public string GetPreview(TData? data)
    {
        var dataToString = data?.ToString();
        if (dataToString is null)
            return string.Empty;

        if (dataToString.Length <= characterCountForPreview)
            return dataToString;

        return dataToString[..characterCountForPreview];
    }
}
