using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.DnDBeyond.Model;

public class DDBTOC(string id = "") :
    BaseStructDocRootBlock<DDBTOCSection>(id)
{
}
