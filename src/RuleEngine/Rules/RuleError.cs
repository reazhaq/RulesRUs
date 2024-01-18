namespace RuleEngine.Rules;

public class RuleError
{
    public string Code { get; set; }
    public string Message { get; set; }

    public override string ToString()
    {
        return $"Code: {Code}; Message: {Message}";
    }
}