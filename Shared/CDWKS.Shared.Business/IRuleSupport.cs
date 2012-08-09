using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDC.Shared.Framework.Business
{
    /// <summary>
    /// Facade over IRuleProcessor
    /// Objects requiring framework support for processing of rules should implement this interface
    /// </summary>
    public interface IRuleSupport : IRuleProcessor
    {
        /// <summary>
        /// This function serves as the registration area where registration of rules are initiated
        /// This function should be typically called by object's constructor
        /// </summary>
        void RegisterRules();
    }
}
