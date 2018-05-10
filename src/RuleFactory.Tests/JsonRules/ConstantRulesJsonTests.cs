
using System;
using FluentAssertions;
using Newtonsoft.Json;
using RuleEngine.Rules;
using Xunit;
using Xunit.Abstractions;

namespace RuleFactory.Tests.JsonRules
{
    public class ConstantRulesJsonTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ConstantRulesJsonTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        
        [Fact]
        public void ConstantRuleOfTypeIntThatReturns55WhenValueIsSetTo55()
        {
            var rule = new ConstantRule<int>{Value = "55"};
            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var value = rule.Get();
            _testOutputHelper.WriteLine($"expected: 55 - actual: {value}");
            value.Should().Be(55);

            // convert to json
            var ruleJson = JsonConvert.SerializeObject(rule, new JsonConverterForRule());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<Rule>(ruleJson, new JsonConverterForRule());
            compileResult = ruleFromJson.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");

            value = ((ConstantRule<int>)ruleFromJson).Get();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}.... expected: 55 - actual: {value}");
            value.Should().Be(55);
        }

        [Fact]
        public void ConstantRuleOfTypeDoubleThatReturnsWhatIsSetAsValueString()
        {
            var rule = new ConstantRule<double>{Value = "99.1"};
            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var value = rule.Get();
            _testOutputHelper.WriteLine($"expected: 99.1 - actual: {value}");
            value.Should().Be(99.1);

            // convert to json
            var ruleJson = JsonConvert.SerializeObject(rule, new JsonConverterForRule());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<Rule>(ruleJson, new JsonConverterForRule());
            compileResult = ruleFromJson.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");

            value = ((ConstantRule<double>)ruleFromJson).Get();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}.... expected: 99.1 - actual: {value}");
            value.Should().Be(99.1);
        }

        // shortcut - quick tests to work with different types
        // unfortunately, it is not very read-friendly; due to generic with reflection
        [Theory]
        [InlineData(typeof(string), "something", "something")]
        [InlineData(typeof(string), "null", null)]
        [InlineData(typeof(int), "5", 5)]
        [InlineData(typeof(int?), "null", null)]
        [InlineData(typeof(int?), "99", 99)]
        [InlineData(typeof(decimal), "123.45", 123.45)]
        [InlineData(typeof(decimal?), "null", null)]
        [InlineData(typeof(decimal?), "456.78", 456.78)]
        [InlineData(typeof(bool), "true", true)]
        [InlineData(typeof(bool?), "null", null)]
        [InlineData(typeof(bool?), "false", false)]
        [InlineData(typeof(float), "1.2", 1.2)]
        [InlineData(typeof(float?), "null", null)]
        [InlineData(typeof(float?), "12.34", 12.34)]
        public void ConstantRuleChangesStringAssignedValueToTypedLambda(Type constantType, string valueToUse, object expectedResult)
        {
            var constantRuleGenericType = typeof(ConstantRule<>);
            var typesToUse = new[] {constantType};
            var constantRuleOfTypeT = constantRuleGenericType.MakeGenericType(typesToUse);
            var instanceOfConstantRuleOfTypeT = Activator.CreateInstance(constantRuleOfTypeT);
            _testOutputHelper.WriteLine($"instanceOfConstantRuleOfTypeT = {instanceOfConstantRuleOfTypeT}");

            var propertyInfo = instanceOfConstantRuleOfTypeT.GetType().GetProperty("Value");
            propertyInfo
                .Should()
                .NotBeNull();
            propertyInfo.SetValue(instanceOfConstantRuleOfTypeT, Convert.ChangeType(valueToUse, propertyInfo.PropertyType));
            
            var compileResult = instanceOfConstantRuleOfTypeT.GetType().GetMethod("Compile").Invoke(instanceOfConstantRuleOfTypeT, null);
            compileResult
                .Should()
                .NotBeNull()
                .And.BeOfType<bool>()
                .And.Be(true);
            _testOutputHelper.WriteLine($"compileResult = {compileResult}");
            _testOutputHelper.WriteLine(
                $"{instanceOfConstantRuleOfTypeT.GetType().GetMethod("ExpressionDebugView").Invoke(instanceOfConstantRuleOfTypeT, null)}");

            var value = instanceOfConstantRuleOfTypeT.GetType().GetMethod("Get").Invoke(instanceOfConstantRuleOfTypeT, null);
            _testOutputHelper.WriteLine($"value = {value ?? "null"}");

            object expectedTypedResult;
            var underyingType = Nullable.GetUnderlyingType(constantType) ?? constantType;
            if (expectedResult == null)
            {
                if (constantType.IsValueType && Nullable.GetUnderlyingType(constantType) != null)
                    expectedTypedResult = null;
                else
                    expectedTypedResult = Convert.ChangeType(null, constantType);
            }
            else
                expectedTypedResult = Convert.ChangeType(expectedResult, underyingType);

            Assert.True(value?.Equals(expectedTypedResult) ?? expectedTypedResult == null);

            // convert to json...
            var ruleJson = JsonConvert.SerializeObject(instanceOfConstantRuleOfTypeT, new JsonConverterForRule());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json...
            var ruleFromJson = JsonConvert.DeserializeObject<Rule>(ruleJson, new JsonConverterForRule());
            var compileResult2 = ruleFromJson.Compile();
            compileResult2.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");
        }

        [Fact]
        public void ContantRuleOfTypeIntThatReturnsString()
        {
            var stringValue = "55";
            var rule = new ConstantRule<int, string> {Value = stringValue};
            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var value = rule.Get(int.MinValue);
            value.Should().BeOfType<string>().And.Be(stringValue);

            // convert to json
            var ruleJson = JsonConvert.SerializeObject(rule, new JsonConverterForRule());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<Rule>(ruleJson, new JsonConverterForRule());
            var compileResult2 = ruleFromJson.Compile();
            compileResult2.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");

            var value2 = ((ConstantRule<int, string>)ruleFromJson).Get(int.MinValue);
            value2.Should().BeOfType<string>().And.Be(stringValue);
        }

        [Fact]
        public void ConstantRuleOfTypeIntNullableBoolRetursNull()
        {
            var rule = new ConstantRule<int, bool?> {Value = "null"};
            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var value = rule.Get(int.MinValue);
            value.Should().Be(default(bool?));

            // convert to json
            var ruleJson = JsonConvert.SerializeObject(rule, new JsonConverterForRule());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<Rule>(ruleJson, new JsonConverterForRule());
            var compileResult2 = ruleFromJson.Compile();
            compileResult2.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");

            var value2 = ((ConstantRule<int, bool?>)ruleFromJson).Get(int.MinValue);
            value2.Should().Be(default(bool?));
        }

        [Fact]
        public void ConstantRuleOfTypeIntNullableBoolReturnsFalse()
        {
            var rule = new ConstantRule<int, bool?> {Value = "false"};
            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var value = rule.Get(int.MinValue);
            value.Should().Be(false);

            // convert to json
            var ruleJson = JsonConvert.SerializeObject(rule, new JsonConverterForRule());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<Rule>(ruleJson, new JsonConverterForRule());
            var compileResult2 = ruleFromJson.Compile();
            compileResult2.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");

            var value2 = ((ConstantRule<int, bool?>)ruleFromJson).Get(int.MinValue);
            value2.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(int), typeof(string), 1, "six-six-six", "six-six-six")]
        [InlineData(typeof(int?), typeof(bool?), null, null, null)]
        [InlineData(typeof(bool), typeof(int), false, "666", 666)]
        [InlineData(typeof(string), typeof(double?), "69", "123.45", 123.45)]
        public void ConstantRuleTwoTypeReturnsSecondIgnoresParameter(Type type1, Type type2, object paramValue,
            string valueToUse, object expectedResult)
        {
            var constantRuleGenericType = typeof(ConstantRule<,>);
            var typesToUse = new[] {type1, type2};
            var constantRuleOfTypeT1AndT2 = constantRuleGenericType.MakeGenericType(typesToUse);
            var instanceOfConstantRule = Activator.CreateInstance(constantRuleOfTypeT1AndT2);
            _testOutputHelper.WriteLine($"instanceOfConstantRule = {instanceOfConstantRule}");

            var propertyInfo = instanceOfConstantRule.GetType().GetProperty("Value");
            propertyInfo.Should().NotBeNull();
            propertyInfo.SetValue(instanceOfConstantRule, Convert.ChangeType(valueToUse, propertyInfo.PropertyType));

            var compileResult = instanceOfConstantRule.GetType().GetMethod("Compile").Invoke(instanceOfConstantRule, null);
            compileResult.Should().NotBeNull().And.BeOfType<bool>().And.Be(true);
            _testOutputHelper.WriteLine($"compileResult for {nameof(instanceOfConstantRule)} = {compileResult}");
            _testOutputHelper.WriteLine(
                $"{instanceOfConstantRule.GetType().GetMethod("ExpressionDebugView").Invoke(instanceOfConstantRule, null)}");

            var getResult = instanceOfConstantRule.GetType().GetMethod("Get").Invoke(instanceOfConstantRule, new[]{paramValue});
            _testOutputHelper.WriteLine($"result from Get({paramValue}): {getResult ?? "nulll"}");

            object expectedTypedResult;
            var underyingType = Nullable.GetUnderlyingType(type2) ?? type2;
            if (expectedResult == null)
            {
                if (type2.IsValueType && Nullable.GetUnderlyingType(type2) != null)
                    expectedTypedResult = null;
                else
                    expectedTypedResult = Convert.ChangeType(null, type2);
            }
            else
                expectedTypedResult = Convert.ChangeType(expectedResult, underyingType);

            Assert.True(getResult?.Equals(expectedTypedResult) ?? expectedTypedResult == null);

            // convert to json
            var ruleJson = JsonConvert.SerializeObject(instanceOfConstantRule, new JsonConverterForRule());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<Rule>(ruleJson, new JsonConverterForRule());
            var compileResult2 = ruleFromJson.Compile();
            compileResult2.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");
        }
    }
}