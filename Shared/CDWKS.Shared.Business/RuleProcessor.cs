using System;
using System.Collections.Generic;
using System.Linq;

namespace RDC.Shared.Framework.Business
{
    /// <summary>
    /// Framework-level class responsible for evaluation of rules in the base BusinessEntity and BusinessManager classes
    /// At this time, the class is marked as internal to protect it from consumption outside of the base framework layer
    /// It is possible that future implementations of objects may require for this class to become public and be consumed outside of the base framework level
    /// </summary>
    internal class RuleProcessor : IRuleProcessor
    {
        #region Properties
        [NonSerialized]
        private IList<Func<string>> _rules;
        internal IList<Func<string>> Rules
        {
            get
            {
                return this._rules ?? (this._rules = new List<Func<string>>());
            }
        }

        private IList<Func<IEnumerable<string>>> _ruleCollections;
        internal IList<Func<IEnumerable<string>>> RuleCollections
        {
            get
            {
                return this._ruleCollections ?? (this._ruleCollections = new List<Func<IEnumerable<string>>>());
            }
        }

        private List<string> _errorMessages;
        internal List<string> ErrorMessages
        {
            get
            {
                return this._errorMessages ?? (this._errorMessages = new List<string>());
            }
        }
        #endregion

        #region IRuleProcessor
        
        public IEnumerable<string> Validate()
        {
            var errors = new List<string>();

            if (_rules != null)
            {
                foreach (var rule in Rules)
                {
                    var result = rule.Invoke();
                    if (!string.IsNullOrEmpty(result)) errors.Add(result);
                }
            }

            if (_ruleCollections != null)
            {
                foreach (var ruleCollection in RuleCollections)
                {
                    var result = ruleCollection.Invoke();
                    if (result != null && result.Count() > 0) errors.AddRange(result);
                }
            }

            this._errorMessages = errors;
            return this.ErrorMessages;
        }

        public void RegisterRule(Func<string> rule)
        {
            this.Rules.Add(rule);
        }

        public void RegisterRuleForRelatedObject(Func<IEnumerable<string>> rule)
        {
            this.RuleCollections.Add(rule);
        }

        public void ClearRules()
        {
            this.Rules.Clear();
            this.RuleCollections.Clear();
        }

        public bool IsValid
        {
            get
            {
                if (_errorMessages != null) return this.ErrorMessages.Count == 0;
                return true;
            }
        }

        #endregion
    }
}
