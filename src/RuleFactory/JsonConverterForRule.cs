using System;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RuleEngine.Rules;

namespace RuleFactory
{
    public class JsonConverterForRule : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jo = new JObject();
            var valueType = value.GetType();
            if(typeof(Rule).IsAssignableFrom(valueType))
            {
                jo.Add("RuleType", valueType.Name);

                var genericTypeArguments = valueType.GenericTypeArguments;
                if (genericTypeArguments != null)
                    jo.Add("BoundingTypes", string.Join(",", genericTypeArguments.Select(t => t.ToString())));
            }

            foreach (var prop in valueType.GetProperties())
            {
                if (prop.CanRead)
                {
                    var propVal = prop.GetValue(value, null);
                    if (propVal != null)
                    {
                        jo.Add(prop.Name, JToken.FromObject(propVal, serializer));
                    }
                }
            }

            foreach (var field in valueType.GetFields())
            {
                var fieldValue = field.GetValue(value);
                if (fieldValue != null)
                    jo.Add(field.Name, JToken.FromObject(fieldValue, serializer));
            }

            jo.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            Debug.WriteLine($"***** jsonObject: {jsonObject}");

            var target = GetRuleObject(objectType, jsonObject);

            // populate the properties of the object
            serializer.Populate(jsonObject.CreateReader(), target);

            // return the object
            return target;
        }

        private Rule GetRuleObject(Type objectType, JObject jsonObject)
        {
            Debug.WriteLine($"***** objectType: {objectType}");
            Debug.WriteLine($"********* jsonObject: {jsonObject}");
            var ruleType = jsonObject["RuleType"].ToObject<string>();
            Debug.WriteLine($"******* ruleType: {ruleType}");
            var boundingTypes = jsonObject["BoundingTypes"].ToObject<string>();
            Debug.WriteLine($"***** boundingTypes: {boundingTypes}");
            
            return RuleFactory.CreateRule(ruleType, boundingTypes.Split(','));
        }

        public override bool CanConvert(Type objectType)
        {
            var canConvert = typeof(Rule).IsAssignableFrom(objectType);
            Debug.WriteLine($"----->objectType: {objectType}; canConvert: {canConvert}");
            return canConvert;
        }
    }
}