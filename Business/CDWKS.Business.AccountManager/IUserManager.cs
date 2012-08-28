using System;
using CDWKS.Model.EF.BIMXchange;

namespace CDWKS.Business.AccountManager
{
    public interface IUserManager : IDisposable
    {
        User GetUser(string username);
    }
}
