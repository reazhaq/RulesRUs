using System;
using FluentAssertions;
using RuleEngine.Rules;
using Xunit;

namespace RuleEngineTests.Rules
{
    public class ConditionalRulesTests
    {
        [Fact]
        public void IfValueContainsReturnDiffValue()
        {
            var valueReplacementIfBad = new ConditionalRule<string, string>
            {
                ConditionRule = new ContainsValueRule<string>
                {
                    EqualityComparer = StringComparer.OrdinalIgnoreCase,
                    CollectionToSearch = {"one", "two", "three", "four", "five", "six"}
                },
                TrueRule = new ConstantRule<string> {Value = "six-six-six"},
                FalseRule = new ConstantRule<string> {Value = null}
            };

            var compileResult = valueReplacementIfBad.Compile();
            compileResult.Should().BeTrue();

            var ruleResult = valueReplacementIfBad.Execute("one");
            ruleResult.ShouldBeEquivalentTo("six-six-six");

            ruleResult = valueReplacementIfBad.Execute("tWo");
            ruleResult.ShouldBeEquivalentTo("six-six-six");

            ruleResult = valueReplacementIfBad.Execute("nine");
            ruleResult.Should().BeNullOrEmpty();
        }
    }
}