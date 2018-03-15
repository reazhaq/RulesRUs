//using System.Collections.Generic;
//using FluentAssertions;
//using RuleEngine.Utils;
//using RuleFactory.Factory;
//using RuleFactory.Tests.Model;
//using Xunit;
//using Xunit.Abstractions;

//namespace RuleFactory.Tests.Factory
//{
//    public class MethodCallFactoriesTests
//    {
//        private readonly ITestOutputHelper _testOutputHelper;

//        public MethodCallFactoriesTests(ITestOutputHelper testOutputHelper)
//        {
//            _testOutputHelper = testOutputHelper;
//        }

//        [Fact]
//        public void CreateMethodCallRule()
//        {
//            var inputs = new List<object>
//            {
//                "Some Name",
//                ConstantRuleFactories.CreateConstantRuleFromPrimitiveTypeAndString("System.StringComparison",
//                    "CurrentCultureIgnoreCase")
//            };
//            var propValueDictionary = new Dictionary<string, object>
//            {
//                {"MethodToCall", "Equals"},
//                {"ObjectToCallMethodOn", "Name"},
//                {"Inputs", inputs}
//            };
//            var rule = MethodCallRuleFactories.CreateMethodCallRule<Game, bool>(propValueDictionary);
//            var compileResult = rule.Compile();
//            compileResult.Should().BeTrue();
//            _testOutputHelper.WriteLine($"{rule.ExpressionDebugView()}");

//            var game = new Game {Name = "foo"};
//            var result = rule.Execute(game);
//            result.Should().BeFalse();

//            game.Name = "Some NAME";
//            result = rule.Execute(game);
//            result.Should().BeTrue();
//        }
//    }
//}