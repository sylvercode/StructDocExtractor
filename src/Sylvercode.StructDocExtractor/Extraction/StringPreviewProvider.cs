namespace Sylvercode.StructDocExtractor.Extraction;

public class StringPreviewProvider(int characterCountForPreview = 20) : IDataPreviewProvider<string>
{

    public string GetPreview(string data)
        => data.Length <= characterCountForPreview ? data : data[..characterCountForPreview];
}
