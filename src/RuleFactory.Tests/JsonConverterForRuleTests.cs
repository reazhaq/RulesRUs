using FluentAssertions;
using ModelForUnitTests;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RuleEngine.Rules;
using Xunit;
using Xunit.Abstractions;

namespace RuleFactory.Tests
{
    public class JsonConverterForRuleTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public JsonConverterForRuleTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Test1()
        {
            var rule = new ConstantRule<int>{Value = "55"};
            var json = JsonConvert.SerializeObject(rule, new JsonConverterForRule());
            _testOutputHelper.WriteLine(json);

            //var rule2 = JsonConvert.DeserializeObject<Rule>(json, settings);
            var rule2 = JsonConvert.DeserializeObject<Rule>(json, new JsonConverterForRule());
            var foo = rule2.Compile();
            foo.Should().BeTrue();
            _testOutputHelper.WriteLine(rule2.ExpressionDebugView());
        }

        [Fact]
        public void Test2()
        {
            var rule = new ValidationRule<int>
            {
                ValueToValidateAgainst = new ConstantRule<int> {Value = "5"},
                OperatorToUse = "Equal",
                RuleError = new RuleError { Code="c1", Message = "number is not 5"}
            };
            var json = JsonConvert.SerializeObject(rule, new JsonConverterForRule());
            _testOutputHelper.WriteLine(json);

            var rule2 = JsonConvert.DeserializeObject<Rule>(json, new JsonConverterForRule());
            var foo = rule2.Compile();
            foo.Should().BeTrue();
            _testOutputHelper.WriteLine(rule2.ExpressionDebugView());
        }

        [Fact]
        public void Test3()
        {
            var rule = new ValidationRule<Game>
            {
                OperatorToUse = "AndAlso",
                RuleError = new RuleError {Code = "c", Message = "m"},
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
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };
            settings.Converters.Add(new JsonConverterForRule());
		
            var json = JsonConvert.SerializeObject(rule, settings);
            _testOutputHelper.WriteLine(json);

            var rule2 = JsonConvert.DeserializeObject<Rule>(json, settings);
            var foo = rule2.Compile();
            foo.Should().BeTrue();
            _testOutputHelper.WriteLine(rule2.ExpressionDebugView());
        }

        [Fact]
        public void Test4()
        {
            var rule = new ConstantRule<int>{Value = "55"};;
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };
            settings.Converters.Add(new JsonConverterForRule());
		
            var json = JsonConvert.SerializeObject(rule, settings);
            _testOutputHelper.WriteLine(json);

            var rule2 = JsonConvert.DeserializeObject<Rule>(json, settings);
            var foo = rule2.Compile();
            foo.Should().BeTrue();
            _testOutputHelper.WriteLine(rule2.ExpressionDebugView());
        }

        [Fact]
        public void Test4Point1()
        {
            var rule = new ConstantRule<int> { Value = "55" }; ;
            // settings will automatically be used by JsonConvert.SerializeObject/DeserializeObject
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            //string json = JsonConvert.SerializeObject(employee,
            //    Formatting.Indented,
            //    new KeysJsonConverter(typeof(Employee)));
            var json = JsonConvert.SerializeObject(rule, Formatting.Indented, new JsonConverterForRule2());
            _testOutputHelper.WriteLine(json);

            var rule2 = JsonConvert.DeserializeObject<Rule>(json, new JsonConverterForRule2());
            var foo = rule2.Compile();
            foo.Should().BeTrue();
            _testOutputHelper.WriteLine(rule2.ExpressionDebugView());
        }

        [Fact]
        public void Test3Point1()
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
            //var settings = new JsonSerializerSettings()
            //{
            //    ContractResolver = new CamelCasePropertyNamesContractResolver(),
            //    Formatting = Formatting.Indented,
            //    NullValueHandling = NullValueHandling.Ignore
            //};
            //settings.Converters.Add(new JsonConverterForRule2());
            // settings will automatically be used by JsonConvert.SerializeObject/DeserializeObject
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
            var settings = new JsonConverterForRule2();

            var json = JsonConvert.SerializeObject(rule, settings);
            _testOutputHelper.WriteLine(json);

            var rule2 = JsonConvert.DeserializeObject<Rule>(json, settings);
            var foo = rule2.Compile();
            foo.Should().BeTrue();
            _testOutputHelper.WriteLine(rule2.ExpressionDebugView());
        }
    }
}