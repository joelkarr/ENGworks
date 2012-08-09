using System.Configuration;
using System.Data.Objects;

using CDWKS.Social.Taco.Interfaces;
using CDWKS.Social.Taco.Models;
using CDWKS.Social.Taco.Repositories;

namespace CDWKS.Social.Taco.UnitOfWork
{
    public class EFUnitOfWork : IUnitOfWork
    {
        #region Members

        private IRepository<SocialFeedbackForm> _socialFeebackForm;
        private readonly ObjectContext _context;

        #endregion

        #region Constructor(s)

        public EFUnitOfWork()
        {
            string connString = ConfigurationManager.ConnectionStrings["CDWKS_SocialFeedbackEntities"].ConnectionString;
            _context = new ObjectContext(connString);
            _context.ContextOptions.LazyLoadingEnabled = true;
        }

        #endregion

        #region Implement IUnitOfWork Members

        public IRepository<SocialFeedbackForm> SocialFeedbackFormRespository
        {
            get { return _socialFeebackForm ?? (_socialFeebackForm = new EFRepository<SocialFeedbackForm>(_context)); }
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
