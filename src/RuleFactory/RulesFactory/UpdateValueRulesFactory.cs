using System;
using System.Linq.Expressions;
using RuleEngine.Rules;
using RuleEngine.Utils;

namespace RuleFactory.RulesFactory
{
    public static class UpdateValueRulesFactory
    {
        public static UpdateValueRule<T> CreateUpdateValueRule<T>(Expression<Func<T, object>> objectToValidate,
                                                                    Rule sourceDataRule)
        {
            return new UpdateValueRule<T>
            {
                ObjectToUpdate = objectToValidate?.GetObjectToValidateFromExpression(),
                SourceDataRule = sourceDataRule
            };
        }

        public static UpdateValueRule<T1, T2> CreateUpdateValueRule<T1, T2>(
            Expression<Func<T1, object>> objectToValidate)
        {
            return new UpdateValueRule<T1, T2>
            {
                ObjectToUpdate = objectToValidate?.GetObjectToValidateFromExpression()
            };
        }

        public static RefUpdateValueRule<T> CreateRefUpdateValueRule<T>() => new RefUpdateValueRule<T>();

        public static RefUpdateValueRule<T> CreateRefUpdateValueRule<T>(Rule sourceDataRule) =>
            new RefUpdateValueRule<T> {SourceDataRule = sourceDataRule};
    }
}