using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using RuleEngine.Common;
using RuleEngine.Interfaces.Rules;
using RuleEngine.Utils;

namespace RuleEngine.Rules
{
    public class ContainsValueRule<T> : Rule, IContainsValueRule<T>
    {
        private Func<T, bool> CompiledDelegate { get; set; }

        public List<T> CollectionToSearch = new List<T>();
        public IEqualityComparer<T> EqualityComparer { get; set; } = EqualityComparer<T>.Default;

        public override Expression BuildExpression(params ParameterExpression[] parameters)
        {
            Expression<Func<T, bool>> expression = s => CollectionToSearch.Contains(s, EqualityComparer);
            return expression;
        }

        public override bool Compile()
        {
            var parameter = Expression.Parameter(typeof(T));
            var expression = BuildExpression(parameter);
            if (!(expression is Expression<Func<T, bool>>)) return false;
#if DEBUG
            Debug.WriteLine($"expression = {expression}");
            expression.TraceNode();
#endif

            CompiledDelegate = (expression as Expression<Func<T, bool>>).Compile();
            return CompiledDelegate != null;
        }

        public bool ContainsValue(T valueToSearch)
        {
            if (CompiledDelegate == null)
                throw new RuleEngineException("A Rule must be compiled first");

            return CompiledDelegate(valueToSearch);
        }
    }
}