using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using RuleEngine.Rules;
using RuleEngine.Utils;

namespace RuleFactory.RulesFactory
{
    public static class ValidationRulesFactory
    {
        public enum LogicalOperatorAtTheRootLevel
        {
            NotEqual,
            Equal,
            GreaterThan,
            GreaterThanOrEqual,
            LessThan,
            LessThanOrEqual
        }

        public enum ChildrenBindingOperator
        {
            Not,
            AndAlso,
            OrElse
        }

        public static ValidationRule<T> CreateValidationRule<T>(Expression<Func<T, object>> objectToValidate, LogicalOperatorAtTheRootLevel operatorToUse, Rule valueValueToValidateAgainst)
        {
            return new ValidationRule<T>
            {
                ObjectToValidate = objectToValidate.GetObjectToValidateFromExpression(),
                OperatorToUse = operatorToUse.ToString(),
                ValueToValidateAgainst = valueValueToValidateAgainst
            };
        }

        public static ValidationRule<T> CreateValidationRule<T>(ChildrenBindingOperator operatorToUse, IList<Rule> childrenRules)
        {
            var rule = new ValidationRule<T>
            {
                OperatorToUse = operatorToUse.ToString()
            };
            rule.ChildrenRules.AddRange(childrenRules);
            return rule;
        }
    }
}