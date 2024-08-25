using Sylvercode.DDBSrcModel.Model.Base;

namespace Sylvercode.DDBSrcModel.Model;

public class SrcTableRow(string id = "") : 
    BaseSrcBlock<SrcTable, SrcTableRowCell>(id)
{

}
