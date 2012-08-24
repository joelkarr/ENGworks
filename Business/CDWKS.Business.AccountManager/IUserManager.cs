using System;

using CDWKS.Model.EF.MasterControl;

namespace CDWKS.Business.AccountManager
{
    public interface IUserManager : IDisposable
    {
        User GetUser(string username);
    }
}
