using FluentAssertions;
using RuleEngine.Rules;
using RuleFactory.RulesFactory;
using SampleModel;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;
using ConstantRulesFactory = RuleFactory.RulesFactory.ConstantRulesFactory;

namespace RuleFactory.Tests.RulesFactory
{
    public class BlockRuleFactoryTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public BlockRuleFactoryTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ConditionalRuleWithBlockUsingFactory()
        {
            var sourceNameRule = ConstantRulesFactory.CreateConstantRule<string>("some fancy name");
            var nameChangeRule = UpdateValueRulesFactory.CreateUpdateValueRule<Game>(g => g.Name, sourceNameRule);

            var sourceRankRule = ConstantRulesFactory.CreateConstantRule<int>("1000");
            var rankingChangeRule = UpdateValueRulesFactory.CreateUpdateValueRule<Game>(g => g.Ranking, sourceRankRule);

            var sourceDescRule = ConstantRulesFactory.CreateConstantRule<string>("some cool description");
            var descriptionChangeRule = UpdateValueRulesFactory.CreateUpdateValueRule<Game>(g=>g.Description, sourceDescRule);
 
            var subRules = new List<Rule>
            {
                nameChangeRule,
                rankingChangeRule,
                descriptionChangeRule
            };
            var blockRule = BlockRulesFactory.CreateActionBlockRule<Game>(subRules);

            var param1Const = ConstantRulesFactory.CreateConstantRule<string>("some name");
            var param2Const = ConstantRulesFactory.CreateConstantRule<StringComparison>("CurrentCultureIgnoreCase");
            var nameEqualsRule = MethodCallRulesFactory.CreateMethodCallRule<Game, bool>("Equals", null, (g => g.Name),
                new List<Rule> { param1Const, param2Const });

            var conditionalUpdateValue =
                ConditionalRulesFactory.CreateConditionalIfThActionRule<Game>(nameEqualsRule, blockRule);


            var compileResult = conditionalUpdateValue.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(conditionalUpdateValue)}:{Environment.NewLine}" +
                                        $"{conditionalUpdateValue.ExpressionDebugView()}");

            var game = new Game { Name = "some name" };
            _testOutputHelper.WriteLine($"before game.Name: {game.Name}");
            conditionalUpdateValue.Execute(game);
            _testOutputHelper.WriteLine($"after game.Name: {game.Name}");
            game.Name.Should().Be("some fancy name");
            _testOutputHelper.WriteLine($"{game}");

            var jsonConverterForRule = new JsonConverterForRule();
            var json = JsonConvert.SerializeObject(conditionalUpdateValue, jsonConverterForRule);
            _testOutputHelper.WriteLine(json);

            var conditionalUpdateValue2 = JsonConvert.DeserializeObject<Rule>(json, jsonConverterForRule);
            compileResult = conditionalUpdateValue2.Compile();
            compileResult.Should().BeTrue();

            var game2 = new Game { Name = "some name" };
            _testOutputHelper.WriteLine($"before game2.Name: {game2.Name}");
            (conditionalUpdateValue2 as ConditionalIfThActionRule<Game>)?.Execute(game2);
            _testOutputHelper.WriteLine($"after game2.Name: {game2.Name}");
            game.Name.Should().Be("some fancy name");
            _testOutputHelper.WriteLine($"{game2}");
        }
    }
}
