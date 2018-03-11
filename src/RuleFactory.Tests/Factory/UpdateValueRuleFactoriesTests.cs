using System.Collections.Generic;
using FluentAssertions;
using RuleFactory.Factory;
using RuleFactory.Tests.Model;
using Xunit;
using Xunit.Abstractions;

namespace RuleFactory.Tests.Factory
{
    public class UpdateValueRuleFactoriesTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public UpdateValueRuleFactoriesTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void CreateUpdateValueRule()
        {
            var sourceRule =
                ConstantRuleFactories.CreateConstantRuleFromPrimitiveTypeAndString("System.String", "new name");

            var propValueDictionary = new Dictionary<string, object>
            {
                {"ObjectToUpdate", "Name"},
                {"SourceDataRule", sourceRule}
            };

            var updateRule = UpdateValueRuleFactories.CreateUpdateValueRule<Game>(propValueDictionary);
            var compileResult = updateRule.Compile();
            compileResult.Should().BeTrue();

            var game = new Game {Name = "some name"};
            updateRule.UpdateFieldOrPropertyValue(game);
            game.Name.Should().Be("new name");
        }
    }
}