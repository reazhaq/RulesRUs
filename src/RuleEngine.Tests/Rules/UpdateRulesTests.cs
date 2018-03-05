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
            var nameChangeRule = new UpdateRule<Game, string>
            {
                ObjectToUpdate = "Name"
            };

            var compileResult = nameChangeRule.Compile();
            compileResult.Should().BeTrue();

            nameChangeRule.UpdateFieldOrPropertyValue(game, "new name");
            game.Name.Should().Be("new name");
        }
    }
}