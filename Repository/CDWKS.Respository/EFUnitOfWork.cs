using System.Data.Objects;

using CDWKS.Model.EF.BIMXchange;

namespace CDWKS.Respository.Account
{
    public class EFUnitOfWork : IUnitOfWork
    {
        #region Members

        private IRepository<User> _user;
        private readonly ObjectContext _context;

        #endregion

        #region Constructor(s)

        public EFUnitOfWork(ObjectContext context)
        {
            _context = context;
            _context.ContextOptions.LazyLoadingEnabled = true;
        }

        #endregion

        #region Implementation of IUnitOfWork

        public IRepository<User> UserRepository
        {
            get { return _user ?? (_user = new EFRepository<User>(_context)); }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void CloseConnection()
        {
            _context.Connection.Close();
        }

        #endregion
    }
}
