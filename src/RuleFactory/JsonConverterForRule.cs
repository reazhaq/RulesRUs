using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using RuleEngine.Rules;

namespace RuleFactory
{
    public class JsonConverterForRule : JsonConverter<Rule>
    {
        public override bool CanConvert(Type typeToConvert) => typeof(Rule).IsAssignableFrom(typeToConvert);

        public override Rule Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

            // this should be "RuleType" property
            if (!reader.Read()
                && reader.TokenType != JsonTokenType.PropertyName
                && !reader.GetString().Equals("RuleType", StringComparison.InvariantCultureIgnoreCase))
                throw new JsonException();
            // read the value of the token
            if (!reader.Read() && reader.TokenType != JsonTokenType.String) throw new JsonException();
            var ruleType = reader.GetString();

            // this should be "BoundingTypes" property
            if (!reader.Read()
                && reader.TokenType != JsonTokenType.PropertyName
                && !reader.GetString().Equals("BoundingTypes", StringComparison.InvariantCultureIgnoreCase))
                throw new JsonException();
            // read the value of the token
            if (!reader.Read() && reader.TokenType != JsonTokenType.String) throw new JsonException();
            var boundingTypes = reader.GetString();

            var rule = RuleFactory.CreateRule(ruleType, boundingTypes?.Split(','));
            var fields = rule.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            var properties = rule.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject) return rule;
                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName:
                        var propertyName = reader.GetString();
                        var field = fields.FirstOrDefault(f =>
                            f.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
                        if (field != null)
                        {

                        }
                        else
                        {
                            var property = properties.FirstOrDefault(p =>
                                p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));

                        }

                        break;
                }
            }

            return rule;
        }

        public override void Write(Utf8JsonWriter writer, Rule value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }


    //public class JsonConverterForRule : JsonConverter
    //{
    //    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //    {
    //        var jo = new JObject();
    //        var valueType = value.GetType();
    //        if (typeof(Rule).IsAssignableFrom(valueType))
    //        {
    //            jo.Add("RuleType", valueType.Name);

    //            if (valueType.IsGenericType)
    //            {
    //                var genericTypeArguments = valueType.GenericTypeArguments;
    //                if (genericTypeArguments != null)
    //                    jo.Add("BoundingTypes", string.Join(",", genericTypeArguments.Select(t => t.ToString())));
    //            }
    //        }

    //        foreach (var prop in valueType.GetProperties())
    //        {
    //            if (prop.CanRead)
    //            {
    //                var propVal = prop.GetValue(value, null);
    //                if (propVal != null)
    //                {
    //                    jo.Add(prop.Name, JToken.FromObject(propVal, serializer));
    //                }
    //            }
    //        }

    //        foreach (var field in valueType.GetFields())
    //        {
    //            var fieldValue = field.GetValue(value);
    //            if (fieldValue != null)
    //                jo.Add(field.Name, JToken.FromObject(fieldValue, serializer));
    //        }

    //        jo.WriteTo(writer);
    //    }

    //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    //    {
    //        var jsonObject = JObject.Load(reader);
    //        Debug.WriteLine($"***** jsonObject: {jsonObject}");

    //        var target = GetRuleObject(objectType, jsonObject);

    //        // populate the properties of the object
    //        serializer.Populate(jsonObject.CreateReader(), target);

    //        // return the object
    //        return target;
    //    }

    //    private Rule GetRuleObject(Type objectType, JObject jsonObject)
    //    {
    //        Debug.WriteLine($"***** objectType: {objectType}");
    //        Debug.WriteLine($"********* jsonObject: {jsonObject}");
    //        var ruleType = jsonObject["RuleType"].ToObject<string>();
    //        Debug.WriteLine($"******* ruleType: {ruleType}");
    //        var boundingTypes = jsonObject["BoundingTypes"]?.ToObject<string>();
    //        Debug.WriteLine($"***** boundingTypes: {boundingTypes}");

    //        return RuleFactory.CreateRule(ruleType, boundingTypes?.Split(','));
    //    }

    //    public override bool CanConvert(Type objectType)
    //    {
    //        var canConvert = typeof(Rule).IsAssignableFrom(objectType);
    //        Debug.WriteLine($"----->objectType: {objectType}; canConvert: {canConvert}");
    //        return canConvert;
    //    }
    //}
}