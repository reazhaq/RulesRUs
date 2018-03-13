using System.Collections.Generic;
using RuleEngine.Rules;

namespace RuleFactory.Factory
{
    public class UpdateValueRuleFactories
    {
        public static UpdateValueRule<T> CreateUpdateValueRule<T>(IDictionary<string, object> propValueDictionary)
        {
            if (propValueDictionary == null) return null;
            var instance = new UpdateValueRule<T>();
            RuleFactories.ReadRuleValues(instance, propValueDictionary);

            if (propValueDictionary.ContainsKey("ObjectToUpdate"))
                instance.ObjectToUpdate = propValueDictionary["ObjectToUpdate"].ToString();

            if (propValueDictionary.ContainsKey("SourceDataRule"))
            {
                var sourceRuleDic = (IDictionary<string,object>) propValueDictionary["SourceDataRule"];
                instance.SourceDataRule = RuleFactory.CreateRuleFromDictionary<T>(sourceRuleDic);
            }

            return instance;
        }
    }
}