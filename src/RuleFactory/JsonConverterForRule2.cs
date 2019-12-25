using System;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RuleEngine.Rules;

namespace RuleFactory
{
    public class JsonConverterForRule2 : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //JToken t = JToken.FromObject(value);

            //if (t.Type != JTokenType.Object)
            //{
            //    t.WriteTo(writer);
            //}
            //else
            //{
            //    JObject o = (JObject)t;
            //    IList<string> propertyNames = o.Properties().Select(p => p.Name).ToList();

            //    o.AddFirst(new JProperty("Keys", new JArray(propertyNames)));

            //    o.WriteTo(writer);
            //}

            var jToken = JToken.FromObject(value);
            if (jToken.Type != JTokenType.Object)
            {
                jToken.WriteTo(writer);
            }
            else
            {
                //var jo = new JObject();
                var jo = (JObject)jToken;
                var valueType = value.GetType();
                if (typeof(Rule).IsAssignableFrom(valueType))
                {
                    //jo.Add("RuleType", valueType.Name);

                    if (valueType.IsGenericType)
                    {
                        var genericTypeArguments = valueType.GenericTypeArguments;
                        if (genericTypeArguments != null)
                        {
                            var boundingTypes = string.Join(",", genericTypeArguments.Select(t => t.ToString()));
                            jo.AddFirst(new JProperty("BoundingTypes", boundingTypes));
                            //jo.Add("BoundingTypes", string.Join(",", genericTypeArguments.Select(t => t.ToString())));
                        }
                    }
                    jo.AddFirst(new JProperty("RuleType", valueType.Name));
                }

                //foreach (var prop in valueType.GetProperties())
                //{
                //    if (prop.CanRead)
                //    {
                //        var propVal = prop.GetValue(value, null);
                //        if (propVal != null)
                //        {
                //            jo.Add(prop.Name, JToken.FromObject(propVal, serializer));
                //        }
                //    }
                //}

                //foreach (var field in valueType.GetFields())
                //{
                //    var fieldValue = field.GetValue(value);
                //    if (fieldValue != null)
                //        jo.Add(field.Name, JToken.FromObject(fieldValue, serializer));
                //}

                jo.WriteTo(writer);
            }




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
            var boundingTypes = jsonObject["BoundingTypes"]?.ToObject<string>();
            Debug.WriteLine($"***** boundingTypes: {boundingTypes}");

            return RuleFactory.CreateRule(ruleType, boundingTypes?.Split(','));
        }

        public override bool CanConvert(Type objectType)
        {
            var canConvert = typeof(Rule).IsAssignableFrom(objectType);
            Debug.WriteLine($"----->objectType: {objectType}; canConvert: {canConvert}");
            return canConvert;
        }
    }
}