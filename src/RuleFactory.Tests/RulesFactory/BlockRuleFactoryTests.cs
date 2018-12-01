using FluentAssertions;
using RuleEngine.Rules;
using RuleFactory.RulesFactory;
using System;
using System.Collections.Generic;
using ModelForUnitTests;
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

        [Fact]
        public void ReturnsUpdatedGameUsingFactory()
        {
            var nameChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Name",
                SourceDataRule = new ConstantRule<string> { Value = "some fancy name" }
            };
            var rankingChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Ranking",
                SourceDataRule = new ConstantRule<int> { Value = "1000" }
            };
            var descriptionChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Description",
                SourceDataRule = new ConstantRule<string> { Value = "some cool description" }
            };
            var selfReturnRule = new SelfReturnRule<Game>();

            var blockRule = new FuncBlockRule<Game, Game>();
            blockRule.Rules.Add(nameChangeRule);
            blockRule.Rules.Add(rankingChangeRule);
            blockRule.Rules.Add(descriptionChangeRule);
            blockRule.Rules.Add(selfReturnRule);

            var compileResult = blockRule.Compile();
            compileResult.Should().BeTrue();

            var game = blockRule.Execute(new Game());
            game.Name.Should().Be("some fancy name");
            game.Ranking.Should().Be(1000);
            game.Description.Should().Be("some cool description");
            _testOutputHelper.WriteLine($"{game}");

            var jsonConverterForRule = new JsonConverterForRule();
            var json = JsonConvert.SerializeObject(blockRule, jsonConverterForRule);
            _testOutputHelper.WriteLine(json);

            var blockRule2 = JsonConvert.DeserializeObject<Rule>(json, jsonConverterForRule);
            compileResult = blockRule2.Compile();
            compileResult.Should().BeTrue();

            var game2 = (blockRule2 as FuncBlockRule<Game, Game>)?.Execute(new Game());
            game2?.Name.Should().Be("some fancy name");
            game2?.Ranking.Should().Be(1000);
            game2?.Description.Should().Be("some cool description");
            _testOutputHelper.WriteLine($"{game2}");
        }
    }
}
