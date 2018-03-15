//using System;
//using System.Collections.Generic;
//using FluentAssertions;
//using Newtonsoft.Json;
//using RuleEngine.Rules;
//using RuleFactory.Factory;
//using RuleFactory.Tests.Model;
//using Xunit;
//using Xunit.Abstractions;

//namespace RuleFactory.Tests.Factory
//{
//    public class ConditionalRuleFactoriesTests
//    {
//        private readonly ITestOutputHelper _testOutputHelper;

//        public ConditionalRuleFactoriesTests(ITestOutputHelper testOutputHelper)
//        {
//            _testOutputHelper = testOutputHelper;
//        }

//        [Fact]
//        public void CreateConditionalIfThActionRule()
//        {
//            var propValueDictionary = new Dictionary<string, object>
//            {
//                {"ConditionRule",
//                    new Dictionary<string, object>
//                    {
//                        {"MethodToCall", "Equals"},
//                        {"RuleType", "MethodCallRule"},
//                        {"BoundingTypes", new List<string>{"RuleFactory.Tests.Model.Game","System.Boolean"}},
//                        {"ObjectToCallMethodOn", "Name"},
//                        {"Inputs", new List<object>
//                        {
//                            "Some Name",
//                            new Dictionary<string,object>
//                            {
//                                {"Id", 0},
//                                {"RuleType", "ConstantRule"},
//                                {"BoundingTypes", new List<string>{"System.StringComparison"}},
//                                {"Value", "CurrentCultureIgnoreCase"}
//                            }
//                        }}
//                    }
//                },
//                {"TrueRule",
//                    new Dictionary<string, object>
//                    {
//                        {"RuleType", "UpdateValueRule"},
//                        {"BoundingTypes", new List<string>{"RuleFactory.Tests.Model.Game"}},
//                        {"ObjectToUpdate", "Name"},
//                        {
//                            "SourceDataRule",
//                            new Dictionary<string,object>
//                            {
//                                {"Id", 0},
//                                {"RuleType", "ConstantRule"},
//                                {"BoundingTypes", new List<string>{"System.String"}},
//                                {"Value", "updated name"}
//                            }
//                        }
//                    }
//                }
//            };
//            _testOutputHelper.WriteLine(JsonConvert.SerializeObject(propValueDictionary, Formatting.Indented));

//            var rule = ConditionalRuleFactories.CreateConditionalIfThActionRule<Game>(propValueDictionary);
//            var compileResult = rule.Compile();
//            compileResult.Should().BeTrue();
//            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
//                                        $"{rule.ExpressionDebugView()}");

//            var game = new Game { Name = "some name" };
//            _testOutputHelper.WriteLine($"before game.Name: {game.Name}");
//            rule.Execute(game);
//            _testOutputHelper.WriteLine($"after game.Name: {game.Name}");
//            game.Name.Should().Be("updated name");
//        }
//    }
//}