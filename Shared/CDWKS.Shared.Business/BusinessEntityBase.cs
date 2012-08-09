using System;
using System.Collections.Generic;
using System.Reflection;

namespace RDC.Shared.Framework.Business
{
    [Serializable]
    public abstract class BusinessEntityBase : IBusinessEntity, IRuleSupport//, IComparable
    {
        #region Properties

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
                    RegisterRules();        //virtual function call
                }
                return _ruleProcessor;
            }
        }

        #endregion

        #region Construction
        #endregion

        #region Functionality
        #endregion

        #region Events
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

        //public int CompareTo(object target)
        //{
        //    try
        //    {
        //        var result = (target.GetType() == this.GetType());
        //        if (!result)
        //            return 1;


        //        Type type = this.GetType();


        //        foreach (var prop in type.GetProperties())
        //        {
        //            Type targetType = target.GetType();
        //            PropertyInfo propertie2 = targetType.GetProperty(prop.Name);

        //            object[] index = null;

        //            object Obj1 = prop.GetValue(this, index);
        //            object Obj2 = propertie2.GetValue(target, index);

        //            IComparable Ic1 = (IComparable)Obj1;
        //            IComparable Ic2 = (IComparable)Obj2;

        //            int returnValue = Ic1.CompareTo(Ic2);
                    
        //        }


        //        PropertyInfo property = type.GetProperty(Property);



        //        return returnValue;

        //    }
        //    catch (Exception Ex)
        //    {
        //        throw new ArgumentException("CompareTo is not possible !");
        //    }
        //}
    }
}
