using System.ServiceModel;

namespace CDWKS.BXC.UserService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IUserService" in both code and config file together.
    [ServiceContract]
    public interface IUserService
    {

        [OperationContract]
        bool IsUserValid(string userName, string password, string role);

    }


}
