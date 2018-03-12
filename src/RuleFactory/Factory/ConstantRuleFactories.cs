using System;
using System.Collections.Generic;
using RuleEngine.Rules;

namespace RuleFactory.Factory
{
    public static class ConstantRuleFactories
    {
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

            switch (targetType)
            {
                //System.String
                case Type stringType when stringType == typeof(string):
                    return new ConstantRule<string> {Value = value};
                //System.Boolean
                case Type boolType when boolType == typeof(bool):
                    return new ConstantRule<bool> {Value = value};
                case Type boolNType when boolNType == typeof(bool?):
                    return new ConstantRule<bool?> {Value = value};
                //System.Byte
                case Type byteType when byteType == typeof(byte):
                    return new ConstantRule<byte> {Value = value};
                case Type byteNType when byteNType == typeof(byte?):
                    return new ConstantRule<byte?> {Value = value};
                //System.SByte
                case Type sbyteType when sbyteType == typeof(sbyte):
                    return new ConstantRule<sbyte> {Value = value};
                case Type sbyteNType when sbyteNType == typeof(sbyte?):
                    return new ConstantRule<sbyte?> {Value = value};
                //System.Int16
                case Type intType when intType == typeof(short):
                    return new ConstantRule<short> {Value = value};
                case Type intNType when intNType == typeof(short?):
                    return new ConstantRule<short?> {Value = value};
                //System.UInt16
                case Type uintType when uintType == typeof(ushort):
                    return new ConstantRule<ushort> {Value = value};
                case Type uintNType when uintNType == typeof(ushort?):
                    return new ConstantRule<ushort?> {Value = value};
                //System.Int32
                case Type int32Type when int32Type == typeof(int):
                    return new ConstantRule<int> {Value = value};
                case Type int32NType when int32NType == typeof(int?):
                    return new ConstantRule<int?> {Value = value};
                //System.UInt32
                case Type uint32Type when uint32Type == typeof(uint):
                    return new ConstantRule<uint> {Value = value};
                case Type uint32NType when uint32NType == typeof(uint?):
                    return new ConstantRule<uint?> {Value = value};
                //System.Int64
                case Type int64Type when int64Type == typeof(long):
                    return new ConstantRule<long> {Value = value};
                case Type int64NType when int64NType == typeof(long?):
                    return new ConstantRule<long?> {Value = value};
                //System.UInt64
                case Type uint64Type when uint64Type == typeof(ulong):
                    return new ConstantRule<ulong> {Value = value};
                case Type uint64NType when uint64NType == typeof(ulong?):
                    return new ConstantRule<ulong?> {Value = value};
                //System.Char
                case Type charType when charType == typeof(char):
                    return new ConstantRule<char> {Value = value};
                case Type charNType when charNType == typeof(char?):
                    return new ConstantRule<char?> {Value = value};
                //System.Double
                case Type dblType when dblType == typeof(double):
                    return new ConstantRule<double> {Value = value};
                case Type dblNType when dblNType == typeof(double?):
                    return new ConstantRule<double?> {Value = value};
                //System.Single
                case Type singleType when singleType == typeof(float):
                    return new ConstantRule<float> {Value = value};
                case Type singleNType when singleNType == typeof(float?):
                    return new ConstantRule<float?> {Value = value};
                //StringComparison
                case Type strComType when strComType==typeof(StringComparison):
                    return new ConstantRule<StringComparison> {Value = value};
            }

            return null;
        }
    }
}