using System;
using System.Collections.Generic;
using FluentAssertions;
using RuleEngine.Rules;
using RuleFactory.Factory;
using RuleFactory.Tests.Model;
using Xunit;
using Xunit.Abstractions;

namespace RuleFactory.Tests.Factory
{
    public class ConditionalRuleFactoriesTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ConditionalRuleFactoriesTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void CreateConditionalIfThActionRule()
        {
            var inputs = new List<object>
            {
                "Some Name",
                ConstantRuleFactories.CreateConstantRuleFromPrimitiveTypeAndString("System.StringComparison",
                    "CurrentCultureIgnoreCase")
            };
            var propValueDictionaryForMethod = new Dictionary<string, object>
            {
                {"MethodToCall", "Equals"},
                {"ObjectToCallMethodOn", "Name"},
                {"Inputs", inputs}
            };
            var conditionRule = MethodCallRuleFactories.CreateMethodCallRule<Game, bool>(propValueDictionaryForMethod);

            var propValueDictionaryForTrue = new Dictionary<string, object>
            {
                {"ObjectToUpdate", "Name"},
                {
                    "SourceDataRule",
                    ConstantRuleFactories.CreateConstantRuleFromPrimitiveTypeAndString("System.String", "updated name")
                }
            };
            var trueRule = UpdateValueRuleFactories.CreateUpdateValueRule<Game>(propValueDictionaryForTrue);

            var propValueDictionary = new Dictionary<string, object>
            {
                {"ConditionRule", conditionRule},
                {"TrueRule", trueRule}
            };

            var rule = ConditionalRuleFactories.CreateConditionalIfThActionRule<Game>(propValueDictionary);
            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var game = new Game {Name = "some name"};
            _testOutputHelper.WriteLine($"before game.Name: {game.Name}");
            rule.Execute(game);
            _testOutputHelper.WriteLine($"after game.Name: {game.Name}");
            game.Name.Should().Be("updated name");
        }
    }
}