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

        [Theory]
        [InlineData("25", 25)]
        [InlineData("3", 3)]
        [InlineData("999", 999)]
        public void IntegerConstantRuleShowsStringValueAssigned(string value, int expectedValue)
        {
            var constRule = new ConstantRule<int>{Value = value};
            var isCompiled = constRule.Compile();
            isCompiled.ShouldBeTrue();

            constRule.Execute().ShouldBeEqualTo(expectedValue);
            _testOutputHelper.WriteLine($"constRule.Execute() with init returned {constRule.Execute()}");
        }
    }
}