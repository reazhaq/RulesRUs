using System;

namespace RuleEngine.Common
{
    // this custom exception doesn't do much...
    // if you see this in your log... you know it is coming from rule engine..
    // if you get any other exception - than the rule is incomplete, like missing a null check
    public class RuleEngineException : Exception
    {
        public RuleEngineException(string message) : base(message) {}
        public RuleEngineException(string message, Exception innException) : base(message, innException) { }
    }
}