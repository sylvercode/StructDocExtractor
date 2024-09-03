using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.DnDBeyond.Model;

public class DDBTOCSection(string id = "") :
    BaseStructDocBlock<DDBTOC, DDBTOCSectionElement>(id)
{

}
