using System;
using System.Collections.Generic;
using RuleEngine.Rules;

namespace RuleFactory
{
    public static class ConstantRuleFactories
    {
        public static ConstantRule<T> CreateConstantRule<T>(string value)
        {
            var constantRuleGenericType = typeof(ConstantRule<>);
            var typesToUse = new[] { typeof(T) };
            var constantRuleOfTypeT = constantRuleGenericType.MakeGenericType(typesToUse);
            var instanceOfConstantRuleOfTypeT = (ConstantRule<T>)Activator.CreateInstance(constantRuleOfTypeT);
            instanceOfConstantRuleOfTypeT.Value = value;
            return instanceOfConstantRuleOfTypeT;
        }

        public static Rule CreateConstantRule(IDictionary<string, string> propValueDictionary)
        {
            if (propValueDictionary == null) return null;
            if (propValueDictionary.ContainsKey("TypeName") && propValueDictionary.ContainsKey("Value"))
                return CreateConstantRuleFromPrimitiveTypeAndString(propValueDictionary["TypeName"], propValueDictionary["Value"]);

            return null;
        }

        // for most common types that make sense for constant rules
        public static Rule CreateConstantRuleFromPrimitiveTypeAndString(string typeName, string value)
        {
            var targetType = Type.GetType(typeName);
            if (targetType != typeof(string) && !targetType.IsPrimitive) return null;

            switch (targetType)
            {
                //System.String
                case Type stringType when stringType == typeof(string):
                    return CreateConstantRule<string>(value);
                //System.Boolean
                case Type boolType when boolType == typeof(bool):
                    return CreateConstantRule<bool>(value);
                case Type boolNType when boolNType == typeof(bool?):
                    return CreateConstantRule<bool?>(value);
                //System.Byte
                case Type byteType when byteType == typeof(byte):
                    return CreateConstantRule<byte>(value);
                case Type byteNType when byteNType == typeof(byte?):
                    return CreateConstantRule<byte?>(value);
                //System.SByte
                case Type sbyteType when sbyteType == typeof(sbyte):
                    return CreateConstantRule<sbyte>(value);
                case Type sbyteNType when sbyteNType == typeof(sbyte?):
                    return CreateConstantRule<sbyte?>(value);
                //System.Int16
                case Type intType when intType == typeof(short):
                    return CreateConstantRule<short>(value);
                case Type intNType when intNType == typeof(short?):
                    return CreateConstantRule<short?>(value);
                //System.UInt16
                case Type uintType when uintType == typeof(ushort):
                    return CreateConstantRule<ushort>(value);
                case Type uintNType when uintNType == typeof(ushort?):
                    return CreateConstantRule<ushort?>(value);
                //System.Int32
                case Type int32Type when int32Type == typeof(int):
                    return CreateConstantRule<int>(value);
                case Type int32NType when int32NType == typeof(int?):
                    return CreateConstantRule<int?>(value);
                //System.UInt32
                case Type uint32Type when uint32Type == typeof(uint):
                    return CreateConstantRule<uint>(value);
                case Type uint32NType when uint32NType == typeof(uint?):
                    return CreateConstantRule<uint?>(value);
                //System.Int64
                case Type int64Type when int64Type == typeof(long):
                    return CreateConstantRule<long>(value);
                case Type int64NType when int64NType == typeof(long?):
                    return CreateConstantRule<long?>(value);
                //System.UInt64
                case Type uint64Type when uint64Type == typeof(ulong):
                    return CreateConstantRule<ulong>(value);
                case Type uint64NType when uint64NType == typeof(ulong?):
                    return CreateConstantRule<ulong?>(value);
                //System.Char
                case Type charType when charType == typeof(char):
                    return CreateConstantRule<char>(value);
                case Type charNType when charNType == typeof(char?):
                    return CreateConstantRule<char?>(value);
                //System.Double
                case Type dblType when dblType == typeof(double):
                    return CreateConstantRule<double>(value);
                case Type dblNType when dblNType == typeof(double?):
                    return CreateConstantRule<double?>(value);
                //System.Single
                case Type singleType when singleType == typeof(float):
                    return CreateConstantRule<float>(value);
                case Type singleNType when singleNType == typeof(float?):
                    return CreateConstantRule<float?>(value);
            }

            return null;
        }
    }
}