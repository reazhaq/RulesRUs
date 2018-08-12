using FluentAssertions;
using RuleFactory.RulesFactory;
using Xunit;
using Xunit.Abstractions;

namespace RuleFactory.Tests.RulesFactory
{
    public class SelfReturnRuleFactoryTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public SelfReturnRuleFactoryTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void CreateASelfReturnStringRule()
        {
            var rule = SelfReturnRuleFactory.CreateSelfReturnRule<string>();
            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"rule: {rule}");

            var result = rule.Get("one");
            result.Should().Be("one");

            result = rule.Get(null);
            result.Should().BeNull();

            result = rule.Get("two");
            result.Should().Be("two");
        }

        [Fact]
        public void CreateASelfReturnNullableIntRule()
        {
            var rule = SelfReturnRuleFactory.CreateSelfReturnRule<int?>();
            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"rule: {rule}");

            var result = rule.Get(5);
            result.Should().Be(5);

            result = rule.Get(null);
            result.Should().BeNull();
        }
    }
}