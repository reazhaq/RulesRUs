using System;
using FluentAssertions;
using Newtonsoft.Json;
using RuleEngine.Rules;
using SampleModel;
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
        public void IntSelfReturn(int someValue)
        {
            var ruleBefore = new SelfReturnRule<int>();
            var customJsonConverter = new JsonConverterForRule();

            // serialize to json
            var ruleJson = JsonConvert.SerializeObject(ruleBefore, customJsonConverter);
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");

            // de-hyrate from json
            var ruleAfter = JsonConvert.DeserializeObject<Rule>(ruleJson, customJsonConverter);

            var compileResult = ruleAfter.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"selfReturnRule2 for Int:{Environment.NewLine}" +
                                        $"{ruleAfter.ExpressionDebugView()}");

            var value = ((SelfReturnRule<int>)ruleAfter).Get(someValue);
            value.Should().Be(someValue);
        }

        [Theory]
        [InlineData("one")]
        [InlineData(null)]
        [InlineData("")]
        public void StringSelfReturn(string someValue)
        {
            var ruleBefore = new SelfReturnRule<string>();
            var customJsonConverter = new JsonConverterForRule();

            // serialize to json
            var ruleJson = JsonConvert.SerializeObject(ruleBefore, customJsonConverter);
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");

            // de-hyrate from json
            var ruleAfter = JsonConvert.DeserializeObject<Rule>(ruleJson, customJsonConverter);

            var compileResult = ruleAfter.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"selfReturnRule for String:{Environment.NewLine}" +
                                        $"{ruleAfter.ExpressionDebugView()}");

            var value = ((SelfReturnRule<string>)ruleAfter).Get(someValue);
            value.Should().Be(someValue);

            var referenceEquals = object.ReferenceEquals(someValue, value);
            referenceEquals.Should().BeTrue();
        }

        [Fact]
        public void GameSelfReturn()
        {
            var ruleBefore = new SelfReturnRule<Game>();
            var customJsonConverter = new JsonConverterForRule();

            // serialize to json
            var ruleJson = JsonConvert.SerializeObject(ruleBefore, customJsonConverter);
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");

            // de-hyrate from json
            var ruleAfter = JsonConvert.DeserializeObject<Rule>(ruleJson, customJsonConverter);

            var compileResult = ruleAfter.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"selfReturnRule for Game:{Environment.NewLine}" +
                                        $"{ruleAfter.ExpressionDebugView()}");

            var someGame = new Game();
            var value = ((SelfReturnRule<Game>)ruleAfter).Get(someGame);
            value.Should().Be(someGame);

            var referenceEquals = object.ReferenceEquals(someGame, value);
            referenceEquals.Should().BeTrue();
        }
    }
}