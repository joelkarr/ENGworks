using System;

using CDWKS.BXC.Domain.MasterControl;

namespace CDWKS.Business.AccountManager
{
    public interface IUserManager : IDisposable
    {
        User GetUser(string username);
    }
}
