using System.Collections.Generic;
using CDWKS.Model.EF.BIMXchange;

namespace CDWKS.Business.TreeManager
{
    public interface ITreeManager
    {
        List<TreeNode> GetStructure(string library, string owner);
    }


}
