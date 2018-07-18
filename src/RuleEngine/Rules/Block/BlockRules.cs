using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using RuleEngine.Common;
using RuleEngine.Interfaces.Rules;

namespace RuleEngine.Rules
{
    public abstract class BlockRules : Rule
    {
        public IList<Rule> Rules { get; } = new List<Rule>();
        protected IList<Expression> Expressions { get; } = new List<Expression>();
    }
}