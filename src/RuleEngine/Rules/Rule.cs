using System;
using System.Linq.Expressions;
using System.Text;
using RuleEngine.Utils;

namespace RuleEngine.Rules
{
    public abstract class Rule
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // what does this rule do
        public string Description { get; set; }
        // in the event of a rule logic failure - send this error
        public RuleError RuleError { get; set; }
        // rule expresion
        protected Expression ExpressionForThisRule { get; set; }

        public virtual string ExpressionDebugView()
        {
            var sb = new StringBuilder();
            ExpressionForThisRule.TraceNode(sb);
            return sb.ToString();
        }

        public abstract Expression BuildExpression(params ParameterExpression[] parameters);
        public abstract bool Compile();
    }
}