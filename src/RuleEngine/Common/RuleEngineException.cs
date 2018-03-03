using System;

namespace RuleEngine.Common
{
    public class RuleEngineException : Exception
    {
        public RuleEngineException(string message) : base(message) {}
        public RuleEngineException(string message, Exception innException) : base(message, innException) { }
    }
}