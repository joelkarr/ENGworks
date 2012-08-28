using System;
using System.Linq;
using CDWKS.Model.EF.BIMXchange;
using CDWKS.Respository.Account;

namespace CDWKS.Business.AccountManager
{
    public class UserManager : IUserManager
    {
        #region Property

        private static IUnitOfWork _context;
        protected static IUnitOfWork DataContext
        {
            get { return _context ?? (_context = new EFUnitOfWork(new BXCModelEntities())); }
        }

        // Track whether Dispose has been called. 
        private bool _disposed;

        #endregion

        #region Implementation of IUserManager

        public User GetUser(string username)
        {
            return DataContext.UserRepository.Find(u => string.Equals(u.UserName, username, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }

        #endregion

        #region Implementation of IDisposable

        // Implement IDisposable. 
        // Do not make this method virtual. 
        // A derived class should not be able to override this method. 
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method. 
            // Therefore, you should call GC.SupressFinalize to 
            // take this object off the finalization queue 
            // and prevent finalization code for this object 
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios. 
        // If disposing equals true, the method has been called directly 
        // or indirectly by a user's code. Managed and unmanaged resources 
        // can be disposed. 
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed. 
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!_disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                }
                // Free your own state (unmanaged objects).
                // Set large fields to null.
                _context = null;

                // Note disposing has been done.
                _disposed = true;

            }
        }

        // Use C# destructor syntax for finalization code. 
        // This destructor will run only if the Dispose method 
        // does not get called. 
        // It gives your base class the opportunity to finalize. 
        // Do not provide destructors in types derived from this class.
        ~UserManager()
        {
            // Do not re-create Dispose clean-up code here. 
            // Calling Dispose(false) is optimal in terms of 
            // readability and maintainability.
            Dispose(false);
        }

        #endregion
    }
}
