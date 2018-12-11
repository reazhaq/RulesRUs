using System;
using FluentAssertions;
using ModelForUnitTests;
using Newtonsoft.Json;
using RuleEngine.Rules;
using Xunit;
using Xunit.Abstractions;

namespace RuleFactory.Tests.JsonRules
{
    public class SelfReturnRuleJsonTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public SelfReturnRuleJsonTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData(5)]
        [InlineData(-5)]
        [InlineData(int.MaxValue)]
        public void IntSelfReturnToAndFromJson(int someValue)
        {
            var ruleBefore = new SelfReturnRule<int>();
            var customJsonConverter = new JsonConverterForRule();

            // serialize to json
            var ruleJson = JsonConvert.SerializeObject(ruleBefore, customJsonConverter);
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");

            // de-hydrate from json
            var ruleAfter = JsonConvert.DeserializeObject<SelfReturnRule<int>>(ruleJson, customJsonConverter);

            var compileResult = ruleAfter.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"selfReturnRule2 for Int:{Environment.NewLine}" +
                                        $"{ruleAfter.ExpressionDebugView()}");

            var value = ruleAfter.Get(someValue);
            value.Should().Be(someValue);
        }

        [Theory]
        [InlineData("one")]
        [InlineData(null)]
        [InlineData("")]
        public void StringSelfReturnToAndFromJson(string someValue)
        {
            var ruleBefore = new SelfReturnRule<string>();
            var customJsonConverter = new JsonConverterForRule();

            // serialize to json
            var ruleJson = JsonConvert.SerializeObject(ruleBefore, customJsonConverter);
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");

            // de-hydrate from json
            var ruleAfter = JsonConvert.DeserializeObject<SelfReturnRule<string>>(ruleJson, customJsonConverter);

            var compileResult = ruleAfter.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"selfReturnRule for String:{Environment.NewLine}" +
                                        $"{ruleAfter.ExpressionDebugView()}");

            var value = ruleAfter.Get(someValue);
            value.Should().Be(someValue);

            // both objects should be pointing to same objects
            var referenceEquals = ReferenceEquals(someValue, value);
            referenceEquals.Should().BeTrue();
        }

        [Fact]
        public void GameSelfReturnToAndFromJson()
        {
            var ruleBefore = new SelfReturnRule<Game>();
            var customJsonConverter = new JsonConverterForRule();

            // serialize to json
            var ruleJson = JsonConvert.SerializeObject(ruleBefore, customJsonConverter);
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");

            // de-hydrate from json
            var ruleAfter = JsonConvert.DeserializeObject<SelfReturnRule<Game>>(ruleJson, customJsonConverter);

            var compileResult = ruleAfter.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"selfReturnRule for Game:{Environment.NewLine}" +
                                        $"{ruleAfter.ExpressionDebugView()}");

            var someGame = new Game();
            var value = ruleAfter.Get(someGame);
            value.Should().Be(someGame);

            // both objects should be pointing to same objects
            var referenceEquals = ReferenceEquals(someGame, value);
            referenceEquals.Should().BeTrue();
        }
    }
}