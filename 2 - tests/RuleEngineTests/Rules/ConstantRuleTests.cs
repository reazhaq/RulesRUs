using System;
using RuleEngine.Rules;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;

namespace RuleEngineTests.Rules
{
    public class ConstantRuleTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ConstantRuleTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

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
        public void ConsttantRuleChangesStringAssignedValueToTypedLambda(Type constantType, string value, object expectedResult)
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
            propertyInfo.SetValue(instanceOfConstantRuleOfTypeT, Convert.ChangeType(value, propertyInfo.PropertyType));
            
            var compileResult = instanceOfConstantRuleOfTypeT.GetType().GetMethod("Compile").Invoke(instanceOfConstantRuleOfTypeT, null);
            compileResult
                .Should()
                .NotBeNull()
                .And.BeOfType<bool>()
                .And.Be(true);
            _testOutputHelper.WriteLine($"compileResult = {compileResult}");

            var executeResult = instanceOfConstantRuleOfTypeT.GetType().GetMethod("Execute").Invoke(instanceOfConstantRuleOfTypeT, null);
            _testOutputHelper.WriteLine($"executeResult = {executeResult ?? "null"}");

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

            Assert.True(executeResult?.Equals(expectedTypedResult) ?? expectedTypedResult == null);
        }
    }
}