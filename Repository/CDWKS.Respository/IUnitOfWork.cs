using CDWKS.Model.EF.BIMXchange;

namespace CDWKS.Respository.Account
{
    /// <summary>
    /// Generic interface that can be used across all unit of works.
    /// </summary>
    public interface IUnitOfWork
    {
        IRepository<User> UserRepository {get;}
        void Save();
        void CloseConnection();
    }
}
