using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using RuleEngine.Rules;
using RuleFactory.RulesFactory;
using RuleFactory.Tests.Model;
using Xunit;
using Xunit.Abstractions;

namespace RuleFactory.Tests.RulesFactory
{
    public class MethodCallRulesFactoryTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public MethodCallRulesFactoryTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }


        [Theory]
        [InlineData("Game 1", true)]
        [InlineData("game 1", true)]
        [InlineData("game 2", false)]
        [InlineData("gaMe 2", false)]
        public void CallEqualsMethodOnNameUsingConstantRule(string param1, bool expectedResult)
        {
            // call Equals method on Name string object
            // compiles to: Param_0.Name.Equals("Game 1", CurrentCultureIgnoreCase)
            var param1Const = ConstantRulesFactory.CreateConstantRule<string>(param1);
            var param2Const = ConstantRulesFactory.CreateConstantRule<StringComparison>("CurrentCultureIgnoreCase");
            var nameEqualsRule = MethodCallRulesFactory.CreateMethodCallRule<Game, bool>("Equals", null, (g => g.Name),
                new List<Rule> { param1Const, param2Const });
            var compileResult = nameEqualsRule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(nameEqualsRule)}:{Environment.NewLine}" +
                                        $"{nameEqualsRule.ExpressionDebugView()}");

            var game1 = new Game { Name = "Game 1" };
            var game2 = new Game { Name = "Game 2" };
            var executeResult = nameEqualsRule.Execute(game1);
            executeResult.Should().Be(expectedResult);

            executeResult = nameEqualsRule.Execute(game2);
            executeResult.Should().Be(!expectedResult);
        }
    }
}