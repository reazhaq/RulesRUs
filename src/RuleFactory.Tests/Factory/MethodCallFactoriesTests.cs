using System.Collections.Generic;
using FluentAssertions;
using RuleEngine.Utils;
using RuleFactory.Factory;
using RuleFactory.Tests.Model;
using Xunit;
using Xunit.Abstractions;

namespace RuleFactory.Tests.Factory
{
    public class MethodCallFactoriesTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public MethodCallFactoriesTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void CreateMethodCallRule()
        {
            IDictionary<string, string> propValueDictionary = new Dictionary<string, string>
            {
                {"MethodToCall", "Equals"},
                {"ObjectToCallMethodOn", "Name"}
            };
            IList<object> inputs = new List<object>
            {
                "Some Name",
                ConstantRuleFactories.CreateConstantRuleFromPrimitiveTypeAndString("System.StringComparison", "CurrentCultureIgnoreCase")
            };
            var rule = MethodCallFactories.CreateMethodCallRule<Game, bool>(propValueDictionary, inputs);
            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{rule.ExpressionDebugView()}");

            var game = new Game {Name = "foo"};
            var result = rule.Execute(game);
            result.Should().BeFalse();

            game.Name = "Some NAME";
            result = rule.Execute(game);
            result.Should().BeTrue();
        }
    }
}