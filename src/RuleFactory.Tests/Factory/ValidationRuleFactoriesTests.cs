using System.Collections.Generic;
using FluentAssertions;
using RuleFactory.Factory;
using RuleFactory.Tests.Model;
using Xunit;
using Xunit.Abstractions;

namespace RuleFactory.Tests.Factory
{
    public class ValidationRuleFactoriesTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ValidationRuleFactoriesTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void CreateValidationRuleForGame()
        {
            var propValueDictionary = new Dictionary<string, object>
            {
                {"OperatorToUse", "GreaterThan"},
                {"ObjectToValidate", "Name.Length"},
                {
                    "ValueToValidateAgainst",
                    ConstantRuleFactories.CreateConstantRuleFromPrimitiveTypeAndString("System.Int32", "3")
                }
            };
            var rule = ValidationRuleFactories.CreateValidationRule<Game>(propValueDictionary);

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{rule.ExpressionDebugView()}");

            var game = new Game { Name = "blah" };
            var result = rule.IsValid(game);
            result.Should().BeTrue();

            game.Name = "foo";
            result = rule.IsValid(game);
            result.Should().BeFalse();
        }
    }
}