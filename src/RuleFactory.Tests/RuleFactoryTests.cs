//using System;
//using System.Collections.Generic;
//using FluentAssertions;
//using Newtonsoft.Json;
//using RuleEngine.Rules;
//using RuleFactory.Tests.Model;
//using Xunit;
//using Xunit.Abstractions;

//namespace RuleFactory.Tests
//{
//    public class RuleFactoryTests
//    {
//        private readonly ITestOutputHelper _testOutputHelper;

//        public RuleFactoryTests(ITestOutputHelper testOutputHelper)
//        {
//            _testOutputHelper = testOutputHelper;
//        }

//        [Fact]
//        public void ConstantRuleCreateTest()
//        {
//            var rule = new ConstantRule<string>{Value = "one"};
//            var serializedDictionary = new Dictionary<string, object>();
//            rule.WriteRuleValuesToDictionary(serializedDictionary);
//            _testOutputHelper.WriteLine(JsonConvert.SerializeObject(serializedDictionary, Formatting.Indented));

//            var compileResult = rule.Compile();
//            _testOutputHelper.WriteLine(rule.ExpressionDebugView());
//            compileResult.Should().BeTrue();
//            var result1 = rule.Get();
//            result1.Should().Be("one");

//            var rule2 = RuleFactory.CreateRuleFromDictionary<string>(serializedDictionary);
//            compileResult = rule2.Compile();
//            _testOutputHelper.WriteLine(rule2.ExpressionDebugView());
//            compileResult.Should().BeTrue();
//            var result2 = (rule2 as ConstantRule<string>)?.Get();
//            result2.Should().Be("one");
//        }

//        [Fact]
//        public void ConditionalIfThActionRuleTest()
//        {
//            var conditionalUpdateValue = new ConditionalIfThActionRule<Game>
//            {
//                ConditionRule = new MethodCallRule<Game, bool>
//                {
//                    ObjectToCallMethodOn = "Name",
//                    MethodToCall = "Equals",
//                    Inputs = { "some name", StringComparison.CurrentCultureIgnoreCase }
//                },
//                TrueRule = new UpdateValueRule<Game>
//                {
//                    ObjectToUpdate = "Name",
//                    SourceDataRule = new ConstantRule<string> { Value = "updated name" }
//                }
//            };

//            var propValueDictionary = new Dictionary<string, object>();
//            conditionalUpdateValue.WriteRuleValuesToDictionary(propValueDictionary);
//            _testOutputHelper.WriteLine(JsonConvert.SerializeObject(propValueDictionary, Formatting.Indented));

//            var compileResult = conditionalUpdateValue.Compile();
//            _testOutputHelper.WriteLine(conditionalUpdateValue.ExpressionDebugView());
//            compileResult.Should().BeTrue();

//            var game = new Game{Name="some name"};
//            conditionalUpdateValue.Execute(game);
//            game.Name.Should().Be("updated name");

//            var rule2 = RuleFactory.CreateRuleFromDictionary<Game>(propValueDictionary);
//            compileResult = rule2.Compile();
//            _testOutputHelper.WriteLine(rule2.ExpressionDebugView());
//            game.Name = "some name";
//            (rule2 as ConditionalIfThActionRule<Game>)?.Execute(game);
//            game.Name.Should().Be("updated name");
//        }
//    }
//}