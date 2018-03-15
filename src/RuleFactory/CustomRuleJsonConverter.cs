using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RuleEngine.Rules;

namespace RuleFactory
{
    public class CustomRuleJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jo = new JObject();
            var valueType = value.GetType();
            jo.Add("RuleType", valueType.ToString());

            var genericTypeArguments = valueType.GenericTypeArguments;
            if (genericTypeArguments != null)
                jo.Add("BoundingTypes", string.Join(",", genericTypeArguments.Select(t => t.ToString())));

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
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            var canConvert = typeof(Rule).IsAssignableFrom(objectType);
            Debug.WriteLine($"----->objectType: {objectType}; canConvert: {canConvert}");
            return canConvert;
        }
    }
}