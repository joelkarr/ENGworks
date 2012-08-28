using System.Collections.Generic;
using CDWKS.Model.EF.BIMXchange;
using CDWKS.Repository.Content;
using CDWKS.Shared.ObjectFactory;
using Ninject;

namespace CDWKS.Business.TreeManager
{
    public class TreeManager : ITreeManager
    {
        #region Properties
        private ITreeNodeRepository _treeNodeRepository { get; set; }

        public ITreeNodeRepository TreeNodeRepository
        {
            get { return _treeNodeRepository = _treeNodeRepository ?? Construction.StandardKernel.Get<ITreeNodeRepository>(); }
        }

        #endregion
        public List<TreeNode> GetStructure(string library, string owner)
        {
            //Move logic to here instead of repo
           return TreeNodeRepository.GetAllByLibrary(library, owner);
        }
    }
}
