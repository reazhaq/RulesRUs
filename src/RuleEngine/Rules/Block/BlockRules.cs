using System.Collections.Generic;
using System.Linq.Expressions;

namespace RuleEngine.Rules
{
    public abstract class BlockRules : Rule
    {
        public IList<Rule> Rules { get; } = new List<Rule>();
        protected IList<Expression> Expressions { get; } = new List<Expression>();
    }
}