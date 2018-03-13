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

        //public static void WriteRuleValues<T>(UpdateValueRule<T> updateValueRule, Dictionary<string, object> propValueDictionary)
        //{
        //    if (updateValueRule == null || propValueDictionary == null) return;
        //    if (!string.IsNullOrEmpty("ObjectToUpdate"))
        //        propValueDictionary.Add("ObjectToUpdate", updateValueRule.ObjectToUpdate);
        //    if (updateValueRule.SourceDataRule != null)
        //        propValueDictionary.Add("SourceDataRule",
        //            RuleFactory.ConvertRuleToDictionary<T>(updateValueRule.SourceDataRule));
        //}
    }
}