using RuleEngine.Common;
using RuleEngine.Interfaces.Rules;
using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace RuleEngine.Rules
{
    public abstract class ConditionalRuleBase : Rule
    {
        public Rule ConditionRule;
        public Rule TrueRule;
        public Rule FalseRule;
    }
}