using System;
using System.Diagnostics;
using System.Net.Http;
using FluentAssert;
using RuleEngine.Rules;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngineTests.Rules
{
    public class ConstantRuleTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ConstantRuleTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        //[Theory]
        //[InlineData("25", 25)]
        //[InlineData("3", 3)]
        //[InlineData("999", 999)]
        //public void IntegerConstantRuleShowsStringValueAssigned(string value, int expectedValue)
        //{
        //    var constRule = new ConstantRule<int>{Value = value};
        //    var isCompiled = constRule.Compile();
        //    isCompiled.ShouldBeTrue();

        //    constRule.Execute().ShouldBeEqualTo(expectedValue);
        //    _testOutputHelper.WriteLine($"constRule.Execute() with init returned {constRule.Execute()}");
        //}

        [Theory]
        [InlineData(typeof(string), "something", "something")]
        [InlineData(typeof(int?), "null", null)]
        [InlineData(typeof(string), "null", null)]
        [InlineData(typeof(int), "5", 5)]
        [InlineData(typeof(int?), "99", 99)]
        [InlineData(typeof(decimal), "123.45", 123.45)]
        public void ConsttantRuleChangesStringAssignedValueToTypedLambda(Type constantType, string value, object expectedResult)
        {
            var constantRuleGenericType = typeof(ConstantRule<>);
            var typesToUse = new[] {constantType};
            var constantRuleOfTypeT = constantRuleGenericType.MakeGenericType(typesToUse);
            var instanceOfConstantRuleOfTypeT = Activator.CreateInstance(constantRuleOfTypeT);

            var propertyInfo = instanceOfConstantRuleOfTypeT.GetType().GetProperty("Value");
            propertyInfo.ShouldNotBeNull();
            propertyInfo.SetValue(instanceOfConstantRuleOfTypeT, Convert.ChangeType(value, propertyInfo.PropertyType));
            
            var compileResult = instanceOfConstantRuleOfTypeT.GetType().GetMethod("Compile").Invoke(instanceOfConstantRuleOfTypeT, null);
            compileResult.ShouldBeOfType<bool>().ShouldBeEqualTo(true);

            var executeResult = instanceOfConstantRuleOfTypeT.GetType().GetMethod("Execute").Invoke(instanceOfConstantRuleOfTypeT, null);
            var expectedResultTyped = Convert.ChangeType(expectedResult, constantType);
            Assert.True(executeResult?.Equals(expectedResultTyped) ?? expectedResultTyped == null);
        }
    }
}