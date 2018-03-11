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

            if (propValueDictionary.ContainsKey("ObjectToUpdate"))
                instance.ObjectToUpdate = propValueDictionary["ObjectToUpdate"].ToString();

            if (propValueDictionary.ContainsKey("SourceDataRule"))
                instance.SourceDataRule = (Rule) propValueDictionary["SourceDataRule"];

            return instance;
        }
    }
}