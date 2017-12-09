using System.Linq.Expressions;

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

        public abstract Expression BuildExpression(ParameterExpression parameter);
        public abstract bool Compile();
    }
}