﻿using System;
using RuleEngine.Rules;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;

namespace RuleEngineTests.Rules
{
    public class ConstantRulesTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ConstantRulesTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ConstantRuleOfTypeIntThatReturns55WhenValueIsSetTo55()
        {
            var ruleReturning55 = new ConstantRule<int>{Value = "55"};
            var compileResult = ruleReturning55.Compile();
            compileResult.Should().BeTrue();

            var value = ruleReturning55.Get();
            value.Should().Be(55);
        }

        [Fact]
        public void ConstantRuleOfTypeDoubleThatReturnsWhatIsSetAsValueString()
        {
            var ruleReturningDouble = new ConstantRule<double>{Value = "99.1"};
            var compileResult = ruleReturningDouble.Compile();
            compileResult.Should().BeTrue();

            var value = ruleReturningDouble.Get();
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
        public void ConsttantRuleChangesStringAssignedValueToTypedLambda(Type constantType, string valueToUse, object expectedResult)
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
        }

        [Fact]
        public void ContantRuleOfTypeIntThatReturnsString()
        {
            var stringValue = "55";
            var ruleReturningString = new ConstantRule<int, string> {Value = stringValue};
            var compileResult = ruleReturningString.Compile();
            compileResult.Should().BeTrue();

            var value = ruleReturningString.Get(int.MinValue);
            value.Should().BeOfType<string>().And.Be(stringValue);
        }

        [Fact]
        public void ConstantRuleOfTypeIntNullableBoolRetursNull()
        {
            var rule = new ConstantRule<int, bool?> {Value = "null"};
            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();

            var value = rule.Get(int.MinValue);
            value.Should().Be(default(bool?));
        }

        [Fact]
        public void ConstantRuleOfTypeIntNullableBoolReturnsFalse()
        {
            var rule = new ConstantRule<int, bool?> {Value = "false"};
            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();

            var value = rule.Get(int.MinValue);
            value.Should().Be(false);
        }
    }
}