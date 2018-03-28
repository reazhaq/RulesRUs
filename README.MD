# Rules R Us
Couldn't come up with a name - this came to mind.
Built using Expression Tree; this helps create-compile-execute simple business rules at runtime; or just wrap some expressions with the same look and feel while writing code.

## How To
Create a Rule, and compile it.  Once compiled, rule can be executed over and over... with almost next to nothing execution time.  Just remember, compile may take some time depending on the comlexity of the expression tree.  Unit tests - probably good place to look for examples and ideas.

## Constant Rule
Most simple rule, and most used rule is a Constant Rule.  Constant rule is a simple Func; that returns a constant value.
```
    var ruleReturning55 = new ConstantRule<int>{Value = "55"};
    var compileResult = ruleReturning55.Compile();

    var value = ruleReturning55.Get(); // value is 55
```
and the expression tree it creates:
```
    ||- unaryExpression.NodeType: Convert
    ||- unaryExpression.DebugView: (System.Int32)55
    ||- unaryExpression.Method: 
    ||- unaryExpression.Operand:
    |  |- constantExpression.Value: 55
    |  |- constantExpression.Type: System.Int32
    |  |- constantExpression.DebugView: 55
```

## Contains Value Rule
Got a list of values that you always check against; this rule can help - make sure to use desired equality comparer.
```
    var containsRule = new ContainsValueRule<string>
    {
        EqualityComparerClassName = "System.StringComparer",
        EqualityComparerPropertyName = "OrdinalIgnoreCase",
        CollectionToSearch = { "one", "two", "three", "four", "five", "six" }
    };

    var compileResult = containsRule.Compile();
    compileResult.Should().BeTrue();
    _testOutputHelper.WriteLine($"{nameof(containsRule)}:{Environment.NewLine}" +
                                $"{containsRule.ExpressionDebugView()}");

    var a1 = containsRule.ContainsValue("One");
    a1.Should().BeTrue();
    var a2 = containsRule.ContainsValue("tWo");
    a2.Should().BeTrue();
    var a7 = containsRule.ContainsValue("seven");
    a7.Should().BeFalse();
```
Note: Contains Value rule requires an equality comparer; if none provided it uses the default for the object.  Loading from file, fill class name and property name to create the comparer for you.  Remember, not all comparer comes as property and those are not supported.

## Expression Action Rule
Action and Func gives you most freedom to create appropriate lambda and use it as rule; when needed
```
    var updateGameRankingRule = new ExpressionActionRule<Game>(g => ApplySomeRule(g));
```

## Expression Func Rule
```
    // g => IIF(((g == null) OrElse (g.Players == null)), 0, g.Players.Count)
    var ruleReturningCountOfPlayers = new ExpressionFuncRules<Game, int>(
                                        g => (g == null || g.Players == null) ? 0 : g.Players.Count);
```

## Method Call Rule

## RegEx Rule

## Validation Rule

# Thanks to everyone
Started playing with Expression Tree and had some sample/example code going for some years.  Some of the ideas for rule engine came from [MicroRuleEngine](https://github.com/runxc1/MicroRuleEngine); and [AutoMapper](https://github.com/AutoMapper/AutoMapper) source code.  [CoreFx](https://github.com/dotnet/corefx) code and unit tests - an excellent way to study and understand what's going on under the hood with Expression tree.  Thanks to many more blogs, example codes, SO answers, .....  Thought I would just glue my experiments together and release it into the wild.  Hope this helps...
