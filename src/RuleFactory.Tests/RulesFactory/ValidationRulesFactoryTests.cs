using RuleFactory.RulesFactory;
using RuleFactory.Tests.Model;
using Xunit;

namespace RuleFactory.Tests.RulesFactory
{
    public class ValidationRulesFactoryTests
    {
        [Fact]
        public void CreateValidationRuleTest()
        {
            //var foo = ValidationRulesFactory.CreateValidationRule<Player>(p => p.Name);
            var foo2 = ValidationRulesFactory.CreateValidationRule<Player>(p => p.CurrentCoOrdinates.X);
            //var foo3 = ValidationRulesFactory.CreateValidationRule<Player>(p => p.CurrentCoOrdinates.Something);
        }
    }
}