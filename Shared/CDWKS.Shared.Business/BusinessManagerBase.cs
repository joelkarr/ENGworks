using System;
using System.Collections.Generic;

namespace RDC.Shared.Framework.Business
{

    /// <summary>
    /// Base class for all the business managers
    /// Implements IBusinessManager and Dispose logic
    /// </summary>
    public abstract class BusinessManagerBase : IBusinessManager, IRuleSupport
    {
        #region Properties

        /// <summary>
        /// Indicates if Dispose has been called on an instance of this class
        /// </summary>
        private bool _disposed;

        [NonSerialized]
        private RuleProcessor _ruleProcessor;
        /// <summary>
        /// Internal object responsible for storing and processing of rules
        /// </summary>
        internal RuleProcessor RuleProcessor
        {
            get
            {
                if (this._ruleProcessor == null)
                {
                    this._ruleProcessor = new RuleProcessor();
                    RegisterRules();    //virtual function call
                }
                return _ruleProcessor;
            }
        }

        #endregion

        #region Construction
        #endregion

        #region Rules

        public virtual void RegisterRules()
        {
        }

        public virtual bool IsValid
        {
            get
            {
                return RuleProcessor.IsValid;
            }
        }

        public void RegisterRule(Func<string> rule)
        {
            RuleProcessor.RegisterRule(rule);
        }

        public void RegisterRuleForRelatedObject(Func<IEnumerable<string>> rule)
        {
            RuleProcessor.RegisterRuleForRelatedObject(rule);
        }

        public IEnumerable<string> Validate()
        {
            return RuleProcessor.Validate();
        }

        public void ClearRules()
        {
            RuleProcessor.ClearRules();
        }

        #endregion

        #region IDisposable
        
        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
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

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        /// <param name="disposing">true is when called manuall within called, false when called by Finalizer</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    DisposeManagedResources();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                DisposeUnmanagedResources();

                // Note disposing has been done.
                _disposed = true;
            }
        }

        /// <summary>
        /// Dispose any managed resources inside this function
        /// This must be implemented by any business manager that is holding on to Data Contexts, Repositories or other objects that support IDisposable
        /// </summary>
        protected abstract void DisposeManagedResources();

        /// <summary>
        /// Dispose any UN-managed resources inside this function
        /// This must be implemented by any business manager that is holding on to file references, DLL pointers, or other non-managed .NET resources
        /// </summary>
        protected abstract void DisposeUnmanagedResources();


        /// <summary>
        /// Do not provide destructors in types derived from this class.
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// </summary>
        ~BusinessManagerBase()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
