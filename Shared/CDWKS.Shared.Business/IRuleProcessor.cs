using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDC.Shared.Framework.Business
{
    public interface IRuleProcessor
    {
        /// <summary>
        /// Validates the object against collection of rules
        /// </summary>
        /// <returns>Error messages for any broken rules</returns>
        IEnumerable<string> Validate();

        /// <summary>
        /// Adds validation rule to the rules engine for this object
        /// </summary>
        /// <param name="rule">Predicate that returns string.Empty if no validation errors were found or a message if a rule was broken</param>
        void RegisterRule(Func<string> rule);

        /// <summary>
        /// Registers validation rule to the rules engine for this object
        /// 
        /// </summary>
        /// <param name="rule">Predicate that returns an empty list if no validation errors were found or a list of messages if multiple rules were broken </param>
        void RegisterRuleForRelatedObject(Func<IEnumerable<string>> rule);

        /// <summary>
        /// Clears all the rules in the object
        /// </summary>
        void ClearRules();

        /// <summary>
        /// Returns TRUE if no rules were broken during the last Validate() execution, FALSE otherwise
        /// </summary>
        bool IsValid { get; }
    }
}
