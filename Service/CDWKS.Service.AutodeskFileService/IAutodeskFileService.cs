using System.ServiceModel;
using CDWKS.Model.Poco.Content;

namespace CDWKS.BXC.AutodeskFileService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAutodeskFileService" in both code and config file together.
    [ServiceContract]
    public interface IAutodeskFileService
    {
        [OperationContract]
        string IndexAutodeskFile(AutodeskFile file, bool overwrite);

        [OperationContract]
        string AddTypeToFile(Item item, string fileName, string owner);

        [OperationContract]
        string UpdateLibraryTreeView(string tree, string owner, string revitVersion);

        [OperationContract]
        AutodeskFile GetAutodeskFile(string fileName, string libraryName);

        [OperationContract]
        int GetContentLibrary(string libraryName, string ownerId);

   
    }


}
