using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RuleEngine.Rules;
using RuleFactory.Tests.Model;
using Xunit;
using Xunit.Abstractions;

namespace RuleFactory.Tests
{
    public class RuleFactoryTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public RuleFactoryTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        //[Fact]
        //public void Test1()
        //{
        //    var conditionalUpdateValue = new ConditionalIfThActionRule<Game>
        //    {
        //        ConditionRule = new MethodCallRule<Game, bool>
        //        {
        //            ObjectToCallMethodOn = "Name",
        //            MethodToCall = "Equals",
        //            Inputs = { "some name", StringComparison.CurrentCultureIgnoreCase }
        //        },
        //        TrueRule = new UpdateValueRule<Game>
        //        {
        //            ObjectToUpdate = "Name",
        //            SourceDataRule = new ConstantRule<string> { Value = "updated name" }
        //        }
        //    };

        //    var ruleDic = RuleFactory.ConvertRuleToDictionary<Game>(conditionalUpdateValue);
        //    _testOutputHelper.WriteLine(JsonConvert.SerializeObject(ruleDic, Formatting.Indented));
        //}

        [Fact]
        public void Test1()
        {
            var conditionalUpdateValue = new ConditionalIfThActionRule<Game>
            {
                ConditionRule = new MethodCallRule<Game, bool>
                {
                    ObjectToCallMethodOn = "Name",
                    MethodToCall = "Equals",
                    Inputs = { "some name", StringComparison.CurrentCultureIgnoreCase }
                },
                TrueRule = new UpdateValueRule<Game>
                {
                    ObjectToUpdate = "Name",
                    SourceDataRule = new ConstantRule<string> { Value = "updated name" }
                }
            };

            var propValueDictionary = new Dictionary<string, object>();
            conditionalUpdateValue.WriteRuleValuesToDictionary(propValueDictionary);
            _testOutputHelper.WriteLine(JsonConvert.SerializeObject(propValueDictionary, Formatting.Indented));
        }
    }
}