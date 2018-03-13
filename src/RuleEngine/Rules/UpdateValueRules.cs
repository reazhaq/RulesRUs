using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using RuleEngine.Common;
using RuleEngine.Interfaces.Rules;

namespace RuleEngine.Rules
{
    public class UpdateValueRuleBase : Rule
    {
        public string ObjectToUpdate;

        public override Expression BuildExpression(params ParameterExpression[] parameters) => throw new NotImplementedException();
        public override bool Compile() => throw new NotImplementedException();

        public override void WriteRuleValuesToDictionary(IDictionary<string, object> propValueDictionary)
        {
            if (propValueDictionary == null) return;
            base.WriteRuleValuesToDictionary(propValueDictionary);
            propValueDictionary.Add(nameof(ObjectToUpdate), ObjectToUpdate);
        }
    }

    public class UpdateValueRule<T> : UpdateValueRuleBase, IUpdateValueRule<T>
    {
        private Action<T> CompiledDelegate { get; set; }
        public Rule SourceDataRule;

        public override Expression BuildExpression(params ParameterExpression[] parameters)
        {
            if (parameters == null || parameters.Length != 1 || parameters[0].Type != typeof(T))
                throw new RuleEngineException($"{nameof(BuildExpression)} must call with one parameter of {typeof(T)}");

            var targetObject = parameters[0];
            var targetExpression = GetExpressionWithSubProperty(targetObject, ObjectToUpdate);
            var sourceExpression = SourceDataRule.BuildExpression(targetObject);
            ExpressionForThisRule = Expression.Assign(targetExpression, sourceExpression);
            return ExpressionForThisRule;
        }

        public override bool Compile()
        {
            var paramObjectToValidate = Expression.Parameter(typeof(T));
            ExpressionForThisRule = BuildExpression(paramObjectToValidate);
            if (ExpressionForThisRule == null) return false;

            Debug.WriteLine($"Expression for UpdateRule<{typeof(T)}>:" +
                            $"{Environment.NewLine}{ExpressionDebugView()}`");

            CompiledDelegate = Expression.Lambda<Action<T>>(ExpressionForThisRule, paramObjectToValidate).Compile();
            return CompiledDelegate != null;
        }

        public void UpdateFieldOrPropertyValue(T targetObject)
        {
            if (CompiledDelegate == null)
                throw new RuleEngineException("A Rule must be compiled first");

            CompiledDelegate(targetObject);
        }

        public override void WriteRuleValuesToDictionary(IDictionary<string, object> propValueDictionary)
        {
            if (propValueDictionary == null) return;
            base.WriteRuleValuesToDictionary(propValueDictionary);

            propValueDictionary.Add("RuleType", $"UpdateValueRule<{typeof(T).Name}>");
            var subDictionary = new Dictionary<string,object>();
            propValueDictionary.Add(nameof(SourceDataRule), subDictionary);
            SourceDataRule.WriteRuleValuesToDictionary(subDictionary);
        }
    }

    public class UpdateValueRule<T1, T2> : UpdateValueRuleBase, IUpdateValueRule<T1, T2>
    {
        private Action<T1, T2> CompiledDelegate { get; set; }

        public override Expression BuildExpression(params ParameterExpression[] parameters)
        {
            if (parameters == null || parameters.Length != 2 || parameters[0].Type != typeof(T1) || parameters[1].Type != typeof(T2))
                throw new RuleEngineException($"{nameof(BuildExpression)} must call with two parameters of {typeof(T1)} and {typeof(T2)}");

            var targetObject = parameters[0];
            var sourceParam = parameters[1];

            var targetExpression = GetExpressionWithSubProperty(targetObject, ObjectToUpdate);
            ExpressionForThisRule = Expression.Assign(targetExpression, sourceParam);
            return ExpressionForThisRule;
        }

        public override bool Compile()
        {
            var paramObjectToValidate = Expression.Parameter(typeof(T1));
            var paramSourceValue = Expression.Parameter(typeof(T2));
            ExpressionForThisRule = BuildExpression(paramObjectToValidate, paramSourceValue);
            if (ExpressionForThisRule == null) return false;

            Debug.WriteLine($"Expression for UpdateRule<{typeof(T1)},{typeof(T2)}>:" +
                            $"{Environment.NewLine}{ExpressionDebugView()}");

            CompiledDelegate = Expression.Lambda<Action<T1, T2>>(ExpressionForThisRule, paramObjectToValidate, paramSourceValue).Compile();
            return CompiledDelegate != null;
        }

        public void UpdateFieldOrPropertyValue(T1 targetObject, T2 source)
        {
            if (CompiledDelegate == null)
                throw new RuleEngineException("A Rule must be compiled first");

            CompiledDelegate(targetObject, source);
        }

        public override void WriteRuleValuesToDictionary(IDictionary<string, object> propValueDictionary)
        {
            if (propValueDictionary == null) return;
            base.WriteRuleValuesToDictionary(propValueDictionary);

            propValueDictionary.Add("RuleType", $"UpdateValueRule<{typeof(T1).Name},{typeof(T2).Name}>");
        }
    }
}