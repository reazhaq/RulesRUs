using System;
using RuleEngine.Rules;

namespace RuleFactory
{
    public static class ConstantRuleFactories
    {
        public static ConstantRule<T> CreateConstantRule<T>(string value)
        {
            var constantRuleGenericType = typeof(ConstantRule<>);
            var typesToUse = new[] {typeof(T)};
            var constantRuleOfTypeT = constantRuleGenericType.MakeGenericType(typesToUse);
            var instanceOfConstantRuleOfTypeT = (ConstantRule<T>) Activator.CreateInstance(constantRuleOfTypeT);
            instanceOfConstantRuleOfTypeT.Value = value;
            return instanceOfConstantRuleOfTypeT;
        }

        //public static Rule CreateConstantRule(string ruleType, string value)
        //{
        //    var type = Type.GetType(ruleType);
        //    if (type == null) return null;

        //    var constantRuleGenericType = typeof(ConstantRule<>);
        //    var typesToUse = new[] {type};
        //    var constantRuleOfTypeT = constantRuleGenericType.MakeGenericType(typesToUse);
        //    var instanceOfConstantRuleOfTypeT = Activator.CreateInstance(constantRuleOfTypeT);

        //    var propertyInfo = instanceOfConstantRuleOfTypeT.GetType().GetProperty("Value");
        //    propertyInfo.SetValue(instanceOfConstantRuleOfTypeT, Convert.ChangeType(value, propertyInfo.PropertyType));

        //    return (Rule)instanceOfConstantRuleOfTypeT;
        //}
    }
}