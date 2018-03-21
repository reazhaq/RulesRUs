using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using RuleEngine.Common;
using RuleEngine.Interfaces.Rules;
using System.Linq;

namespace RuleEngine.Rules
{
    public class ValidationRule<T> : Rule, IValidationRule<T>
    {
        private Func<T, bool> CompiledDelegate { get; set; }

        public Rule ValueToValidateAgainst;
        public string OperatorToUse;
        public string ObjectToValidate { get; set; }

        public List<Rule> ChildrenRules { get; } = new List<Rule>();

        public override Expression BuildExpression(params ParameterExpression[] parameters)
        {
            if (parameters == null || parameters.Length != 1 || parameters[0].Type != typeof(T))
                throw new RuleEngineException($"{nameof(BuildExpression)} must call with one parameter of {typeof(T)}");

            if (!Enum.TryParse(OperatorToUse, out ExpressionType operatorToUse) ||
                (!LogicalOperatorsToUseAtTheRuleLevel.Contains(operatorToUse) &&
                 !LogicalOperatorsToBindChildrenRules.Contains(operatorToUse)))
                throw new RuleEngineException($"Bad {nameof(operatorToUse)} value {operatorToUse}"); //todo: update message 

            var rootParameterExpression = parameters[0];

            if (!ChildrenRules.Any())
            {
                var targetValueParam = Expression.Parameter(typeof(Rule));
                var targetValueExpression = ValueToValidateAgainst?.BuildExpression(targetValueParam);

                var leftExpression = GetExpressionWithSubProperty(rootParameterExpression, ObjectToValidate);
                var binaryExpressionBody = Expression.MakeBinary(operatorToUse, leftExpression, targetValueExpression);
                Debug.WriteLine($"  {nameof(binaryExpressionBody)}: {binaryExpressionBody}");
                return binaryExpressionBody;
            }

            IList<Expression> childrenExpressions = new List<Expression>();
            foreach (var childrenRule in ChildrenRules)
            {
                childrenExpressions.Add(childrenRule.BuildExpression(rootParameterExpression));
            }

            ExpressionForThisRule = null;
            if (childrenExpressions.Any())
            {
                switch (operatorToUse)
                {
                    case ExpressionType.Not:
                        ExpressionForThisRule = Expression.Not(childrenExpressions[0]);
                        break;
                    case ExpressionType.AndAlso:
                        ExpressionForThisRule = Expression.AndAlso(childrenExpressions[0], childrenExpressions[1]);
                        for (var index = 2; index < childrenExpressions.Count; index++)
                            ExpressionForThisRule = Expression.AndAlso(ExpressionForThisRule, childrenExpressions[index]);
                        break;
                    default:
                        ExpressionForThisRule = Expression.OrElse(childrenExpressions[0], childrenExpressions[1]);
                        for (var index = 2; index < childrenExpressions.Count; index++)
                            ExpressionForThisRule = Expression.OrElse(ExpressionForThisRule, childrenExpressions[index]);
                        break;
                }
            }

            return ExpressionForThisRule;
        }

        public override bool Compile()
        {
            var funcParameter = Expression.Parameter(typeof(T));
            ExpressionForThisRule = BuildExpression(funcParameter);
            if (ExpressionForThisRule == null) return false;

            Debug.WriteLine($"{nameof(ExpressionForThisRule)} ready to compile:" +
                            $"{Environment.NewLine}{ExpressionDebugView()}");

            CompiledDelegate = Expression.Lambda<Func<T, bool>>(ExpressionForThisRule, funcParameter).Compile();
            return CompiledDelegate != null;
        }

        public bool IsValid(T targetObject)
        {
            if (CompiledDelegate == null)
                throw new RuleEngineException("A Rule must be compiled first");

            return CompiledDelegate(targetObject);
        }
    }

    public class ValidationRule<T1, T2> : Rule, IValidationRule<T1, T2>
    {
        private Func<T1, T2, bool> CompiledDelegate { get; set; }

        public string OperatorToUse;
        public string ObjectToValidate1;
        public string ObjectToValidate2;

        public override Expression BuildExpression(params ParameterExpression[] parameters)
        {
            if (parameters == null || parameters.Length != 2 || parameters[0].Type != typeof(T1) || parameters[1].Type != typeof(T2))
                throw new RuleEngineException($"{nameof(BuildExpression)} must call with two parameters of {typeof(T1)} and {typeof(T2)}");

            if (!Enum.TryParse(OperatorToUse, out ExpressionType operatorToUse) ||
                !LogicalOperatorsToUseAtTheRuleLevel.Contains(operatorToUse))
                throw new RuleEngineException($"Bad {nameof(operatorToUse)} value {operatorToUse}"); //todo: update message 

            var param1 = parameters[0];
            var param2 = parameters[1];

            var expression1 = GetExpressionWithSubProperty(param1, ObjectToValidate1);
            var expression2 = GetExpressionWithSubProperty(param2, ObjectToValidate2);

            ExpressionForThisRule = Expression.MakeBinary(operatorToUse, expression1, expression2);
            return ExpressionForThisRule;
        }

        public override bool Compile()
        {
            var param1 = Expression.Parameter(typeof(T1));
            var param2 = Expression.Parameter(typeof(T2));
            ExpressionForThisRule = BuildExpression(param1, param2);
            if (ExpressionForThisRule == null) return false;

            Debug.WriteLine($"{nameof(ExpressionForThisRule)} ready to compile:" +
                            $"{Environment.NewLine}{ExpressionDebugView()}");

            CompiledDelegate = Expression.Lambda<Func<T1, T2, bool>>(ExpressionForThisRule, param1, param2).Compile();
            return CompiledDelegate != null;
        }

        public bool IsValid(T1 param1, T2 param2)
        {
            if (CompiledDelegate == null)
                throw new RuleEngineException("A Rule must be compiled first");

            return CompiledDelegate(param1, param2);
        }
    }
}