using FluentAssertions;
using RuleEngine.Rules;
using RuleEngine.Tests.Model;
using Xunit;

namespace RuleEngine.Tests.Rules
{
    public class UpdateRulesTests
    {
        [Fact]
        public void UpdatePropertyStingWithDifferentValue()
        {
            var game = new Game {Name = "game name"};
            var nameChangeRule = new UpdateValueRule<Game, string>
            {
                ObjectToUpdate = "Name"
            };

            var compileResult = nameChangeRule.Compile();
            compileResult.Should().BeTrue();

            nameChangeRule.UpdateFieldOrPropertyValue(game, "new name");
            game.Name.Should().Be("new name");
        }

        [Fact]
        public void UpdatePropertyFromAnotherRule()
        {
            var game = new Game {Name = "game name"};
            var nameChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Name",
                SourceDataRule = new ConstantRule<string> {Value = "name from constant rule"}
            };

            var compileResult = nameChangeRule.Compile();
            compileResult.Should().BeTrue();

            nameChangeRule.UpdateFieldOrPropertyValue(game);
            game.Name.Should().Be("name from constant rule");
        }
    }
}