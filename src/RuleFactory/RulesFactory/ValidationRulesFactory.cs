using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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

        public static ValidationRule<T> CreateValidationRule<T>(LogicalOperatorAtTheRootLevel operatorToUse,
            Rule valueToValidateAgainst)
        {
            return new ValidationRule<T>
            {
                OperatorToUse = operatorToUse.ToString(),
                ValueToValidateAgainst = valueToValidateAgainst
            };
        }

        public static ValidationRule<T> CreateValidationRule<T>(Expression<Func<T, object>> objectToValidate,
                                            LogicalOperatorAtTheRootLevel operatorToUse, Rule valueToValidateAgainst)
        {
            return new ValidationRule<T>
            {
                ObjectToValidate = objectToValidate.GetObjectToValidateFromExpression(),
                OperatorToUse = operatorToUse.ToString(),
                ValueToValidateAgainst = valueToValidateAgainst
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