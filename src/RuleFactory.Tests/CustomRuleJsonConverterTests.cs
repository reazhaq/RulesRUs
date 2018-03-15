using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RuleEngine.Rules;
using Xunit;
using Xunit.Abstractions;

namespace RuleFactory.Tests
{
    public class CustomRuleJsonConverterTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public CustomRuleJsonConverterTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Test1()
        {
            var rule = new ConstantRule<int>{Value = "55"};
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };
            settings.Converters.Add(new CustomRuleJsonConverter());
		
            var json = JsonConvert.SerializeObject(rule, settings);
            _testOutputHelper.WriteLine(json);
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
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };
            settings.Converters.Add(new CustomRuleJsonConverter());
		
            var json = JsonConvert.SerializeObject(rule, settings);
            _testOutputHelper.WriteLine(json);
        }
    }
}