using System;
using FluentAssertions;
using Newtonsoft.Json;
using RuleEngine.Rules;
using RuleFactory.Tests.Fixture;
using RuleFactory.Tests.Model;
using Xunit;
using Xunit.Abstractions;

namespace RuleFactory.Tests.JsonRules
{
    public class ValidationRulesJsonTests : IClassFixture<ValidationRulesFixture>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly Game _game;

        public ValidationRulesJsonTests(ValidationRulesFixture validationRulesJsonFixture, ITestOutputHelper testOutputHelper)
        {
            _game = validationRulesJsonFixture.Game;
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void RuleToCheckIfAnIntegerMatchesRuleValueOrNot()
        {
            var rule = new ValidationRule<int>
            {
                ValueToValidateAgainst = new ConstantRule<int> { Value = "5" },
                OperatorToUse = "Equal",
                RuleError = new RuleError { Code = "c1", Message = "number is not 5" }
            };
            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var not5Rule = new ValidationRule<int>
            {
                ValueToValidateAgainst = new ConstantRule<int> { Value = "5" },
                OperatorToUse = "NotEqual",
                RuleError = new RuleError { Code = "c2", Message = "number is 5" }
            };
            compileResult = not5Rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(not5Rule)}:{Environment.NewLine}" +
                                        $"{not5Rule.ExpressionDebugView()}");

            var ruleExecuteResult = rule.IsValid(5);
            ruleExecuteResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"with 5 {nameof(rule)} execute result: {ruleExecuteResult}");

            ruleExecuteResult = rule.IsValid(6);
            ruleExecuteResult.Should().BeFalse();
            _testOutputHelper.WriteLine($"with 6 {nameof(rule)} failed. " +
                                         $"Error code={rule.RuleError.Code}, " +
                                         $"message={rule.RuleError.Message}");

            ruleExecuteResult = not5Rule.IsValid(6);
            ruleExecuteResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"with 6 {nameof(not5Rule)} execute result: {ruleExecuteResult}");

            ruleExecuteResult = not5Rule.IsValid(5);
            ruleExecuteResult.Should().BeFalse();
            _testOutputHelper.WriteLine($"with 5 {nameof(not5Rule)} failed. " +
                                         $"Error code={not5Rule.RuleError.Code}, " +
                                         $"message={not5Rule.RuleError.Message}");

            // convert to json
            var ruleJson = JsonConvert.SerializeObject(rule, new CustomRuleJsonConverter());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<Rule>(ruleJson, new CustomRuleJsonConverter());
            compileResult = ruleFromJson.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");

            var validationResult2 = ((ValidationRule<int>)ruleFromJson).IsValid(5);
            validationResult2.Should().BeTrue();
            validationResult2 = ((ValidationRule<int>)ruleFromJson).IsValid(6);
            validationResult2.Should().BeFalse();
        }

        [Fact]
        public void RuleToCheckIfRootObjectIsNullOrNot()
        {
            var rule = new ValidationRule<Game>
            {
                ValueToValidateAgainst = new ConstantRule<Game> { Value = "null" },
                OperatorToUse = "NotEqual"
            };
            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}{rule.ExpressionDebugView()}");


            var nullRule = new ValidationRule<Game>
            {
                ValueToValidateAgainst = new ConstantRule<Game> { Value = "null" },
                OperatorToUse = "Equal"
            };
            compileResult = nullRule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(nullRule)}:{Environment.NewLine}{nullRule.ExpressionDebugView()}");


            var ruleExecuteResult = rule.IsValid(_game);
            ruleExecuteResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"with non-null parameter validationResult = {ruleExecuteResult}; expecting true");

            ruleExecuteResult = rule.IsValid(null);
            ruleExecuteResult.Should().BeFalse();
            _testOutputHelper.WriteLine($"with null parameter validationResult = {ruleExecuteResult}; expecting false");

            ruleExecuteResult = nullRule.IsValid(_game);
            ruleExecuteResult.Should().BeFalse();
            _testOutputHelper.WriteLine($"with non-null parameter validationResult = {ruleExecuteResult}; expecting false");

            ruleExecuteResult = nullRule.IsValid(null);
            ruleExecuteResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"with null parameter validationResult = {ruleExecuteResult}; expecting true");

            // convert to json
            var ruleJson = JsonConvert.SerializeObject(rule, new CustomRuleJsonConverter());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<Rule>(ruleJson, new CustomRuleJsonConverter());
            compileResult = ruleFromJson.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");

            var validationResult2 = ((ValidationRule<Game>)ruleFromJson).IsValid(_game);
            validationResult2.Should().BeTrue();
            validationResult2 = ((ValidationRule<Game>)ruleFromJson).IsValid(null);
            validationResult2.Should().BeFalse();
        }

        [Fact]
        public void ApplyRuleToFieldOrProperty()
        {
            var rule = new ValidationRule<Game>
            {
                OperatorToUse = "LessThan",
                ValueToValidateAgainst = new ConstantRule<int> { Value = "100" },
                ObjectToValidate = "Ranking",
                RuleError = new RuleError { Code = "c1", Message = "Ranking must be less than 100" }
            };

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");
            
            var validationResult = rule.IsValid(_game);
            validationResult.Should().BeTrue();

            var someOtherGameWithHighRanking = new Game { Ranking = 101 };
            validationResult = rule.IsValid(someOtherGameWithHighRanking);
            validationResult.Should().BeFalse();
            _testOutputHelper.WriteLine($"with {nameof(someOtherGameWithHighRanking.Ranking)}={someOtherGameWithHighRanking.Ranking} " +
                                         $"{nameof(rule)} failed. " +
                                         $"Error code={rule.RuleError.Code}, " +
                                         $"message={rule.RuleError.Message}");

            // convert to json
            var ruleJson = JsonConvert.SerializeObject(rule, new CustomRuleJsonConverter());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<Rule>(ruleJson, new CustomRuleJsonConverter());
            compileResult = ruleFromJson.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");

            var validationResult2 = ((ValidationRule<Game>)ruleFromJson).IsValid(_game);
            validationResult2.Should().BeTrue();
            validationResult2 = ((ValidationRule<Game>)ruleFromJson).IsValid(someOtherGameWithHighRanking);
            validationResult2.Should().BeFalse();
        }

        [Fact]
        public void ApplyRuleToSubFieldOrProperty()
        {
            var rule = new ValidationRule<Game>
            {
                OperatorToUse = "GreaterThan",
                ValueToValidateAgainst = new ConstantRule<int> { Value = "3" },
                ObjectToValidate = "Name.Length",
                RuleError = new RuleError { Code = "c1", Message = "Name length must be greater than 3" }
            };

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var validationResult = rule.IsValid(_game);
            validationResult.Should().BeTrue();

            var someGameWithShortName = new Game { Name = "foo" };
            validationResult = rule.IsValid(someGameWithShortName);
            validationResult.Should().BeFalse();
            _testOutputHelper.WriteLine($"with {nameof(someGameWithShortName.Name)}={someGameWithShortName.Name} " +
                                         $"{nameof(rule)} failed. " +
                                         $"Error code={rule.RuleError.Code}, " +
                                         $"message={rule.RuleError.Message}");

            // convert to json
            var ruleJson = JsonConvert.SerializeObject(rule, new CustomRuleJsonConverter());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<Rule>(ruleJson, new CustomRuleJsonConverter());
            compileResult = ruleFromJson.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");

            var validationResult2 = ((ValidationRule<Game>)ruleFromJson).IsValid(_game);
            validationResult2.Should().BeTrue();
            validationResult2 = ((ValidationRule<Game>)ruleFromJson).IsValid(new Game { Name = "foo" });
            validationResult2.Should().BeFalse();
        }

        [Fact]
        public void ValidationRuleWithAndAlsoChildrenValidationRules()
        {
            var rule = new ValidationRule<Game>
            {
                OperatorToUse = "AndAlso",
                RuleError = new RuleError { Code = "c", Message = "m" },
                ChildrenRules =
                {
                    new ValidationRule<Game>
                    {
                        OperatorToUse = "NotEqual",
                        ValueToValidateAgainst = new ConstantRule<Game> {Value = "null"}
                    },
                    new ValidationRule<Game>
                    {
                        ValueToValidateAgainst = new ConstantRule<string> {Value = "null"},
                        ObjectToValidate = "Name",
                        OperatorToUse = "NotEqual"
                    },
                    new ValidationRule<Game>
                    {
                        ValueToValidateAgainst = new ConstantRule<int> {Value = "3"},
                        ObjectToValidate = "Name.Length",
                        OperatorToUse = "GreaterThan"
                    }
                }
            };

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");
            
            var validationResult = rule.IsValid(_game);
            validationResult.Should().BeTrue();

            validationResult = rule.IsValid(null);
            validationResult.Should().BeFalse();
            _testOutputHelper.WriteLine($"{nameof(rule)} failed. " +
                                         $"Error code={rule.RuleError.Code}, " +
                                         $"message={rule.RuleError.Message}");

            // convert to json
            var ruleJson = JsonConvert.SerializeObject(rule, new CustomRuleJsonConverter());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<Rule>(ruleJson, new CustomRuleJsonConverter());
            compileResult = ruleFromJson.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");

            var validationResult2 = ((ValidationRule<Game>)ruleFromJson).IsValid(_game);
            validationResult2.Should().BeTrue();
            validationResult2 = ((ValidationRule<Game>)ruleFromJson).IsValid(null);
            validationResult2.Should().BeFalse();
        }

        [Fact]
        public void ValidataionRuleWithOneNotChild()
        {
            var rule = new ValidationRule<Game>
            {
                OperatorToUse = "Not",
                RuleError = new RuleError { Code = "c", Message = "m" }
            };
            rule.ChildrenRules.Add(
                new ValidationRule<Game>
                {
                    OperatorToUse = "NotEqual",
                    ValueToValidateAgainst = new ConstantRule<Game> { Value = "null" }
                }
            );

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");
            
            var validationResult = rule.IsValid(_game);
            validationResult.Should().BeFalse();

            validationResult = rule.IsValid(null);
            validationResult.Should().BeTrue();

            // convert to json
            var ruleJson = JsonConvert.SerializeObject(rule, new CustomRuleJsonConverter());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<Rule>(ruleJson, new CustomRuleJsonConverter());
            compileResult = ruleFromJson.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");

            var validationResult2 = ((ValidationRule<Game>)ruleFromJson).IsValid(_game);
            validationResult2.Should().BeFalse();

            validationResult2 = ((ValidationRule<Game>) ruleFromJson).IsValid(null);
            validationResult2.Should().BeTrue();
        }

        [Fact]
        public void ValidationRuleWithOrElseChildrenValidationRules()
        {
            var rule = new ValidationRule<Game> { OperatorToUse = "OrElse" };
            rule.ChildrenRules.Add(new ValidationRule<Game>
            {
                OperatorToUse = "Equal",
                ValueToValidateAgainst = new ConstantRule<Game> { Value = "null" }
            });
            rule.ChildrenRules.Add
            (
                new ValidationRule<Game>
                {
                    OperatorToUse = "AndAlso",
                    ChildrenRules =
                    {
                        new ValidationRule<Game>
                        {
                            ValueToValidateAgainst = new ConstantRule<string> {Value = "null"},
                            ObjectToValidate = "Name",
                            OperatorToUse = "NotEqual"
                        },
                        new ValidationRule<Game>
                        {
                            ValueToValidateAgainst = new ConstantRule<int> {Value = "3"},
                            ObjectToValidate = "Name.Length",
                            OperatorToUse = "GreaterThan"
                        }

                    }
                }
            );

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}{rule.ExpressionDebugView()}");

            var validationResult = rule.IsValid(_game);
            validationResult.Should().BeTrue();

            validationResult = rule.IsValid(null);
            validationResult.Should().BeTrue();

            validationResult = rule.IsValid(new Game { Name = null });
            validationResult.Should().BeFalse();

            validationResult = rule.IsValid(new Game { Name = "a" });
            validationResult.Should().BeFalse();

            // convert to json
            var ruleJson = JsonConvert.SerializeObject(rule, new CustomRuleJsonConverter());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<Rule>(ruleJson, new CustomRuleJsonConverter());
            compileResult = ruleFromJson.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");

            var validationResult2 = ((ValidationRule<Game>)ruleFromJson).IsValid(_game);
            validationResult2.Should().BeTrue();

            validationResult2 = ((ValidationRule<Game>) ruleFromJson).IsValid(null);
            validationResult2.Should().BeTrue();

            validationResult2 = ((ValidationRule<Game>) ruleFromJson).IsValid(new Game { Name = null });
            validationResult2.Should().BeFalse();

            validationResult2 = ((ValidationRule<Game>) ruleFromJson).IsValid(new Game { Name = "a" });
            validationResult2.Should().BeFalse();
        }

        [Fact]
        public void ValidationRuleWithTwoTypes()
        {
            var rule = new ValidationRule<Player, Player>
            {
                OperatorToUse = "GreaterThan",
                ObjectToValidate1 = "CurrentScore",
                ObjectToValidate2 = "CurrentScore"
            };

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}{rule.ExpressionDebugView()}");

            var validationResult = rule.IsValid(_game.Players[0], _game.Players[1]);
            validationResult.Should().BeTrue();
            validationResult = rule.IsValid(_game.Players[1], _game.Players[0]);
            validationResult.Should().BeFalse();

            // convert to json
            var ruleJson = JsonConvert.SerializeObject(rule, new CustomRuleJsonConverter());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<Rule>(ruleJson, new CustomRuleJsonConverter());
            compileResult = ruleFromJson.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");

            var validationResult2 = ((ValidationRule<Player, Player>)ruleFromJson).IsValid(_game.Players[0], _game.Players[1]);
            validationResult2.Should().BeTrue();
            validationResult2 = ((ValidationRule<Player, Player>)ruleFromJson).IsValid(_game.Players[1], _game.Players[0]);
            validationResult2.Should().BeFalse();
        }
    }
}