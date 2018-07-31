# Rules R Us
Couldn't come up with a name - "Rules R Us" came to mind; Sounded good.

Built using Expression Tree; create-compile-execute simple business rules at runtime.  [Rules Factory](/src/RuleEngine) can help create rules; since there may be too many details to keep track for a complex rule.  See [Sample 4](/samples/Sample4RuleCreateMadeEasy) for a simple step-by-step tutorial.

Json Converter For Rule is there to help read-write rule from-into a json document.  See [Sample 2](/samples/Sample2PlaceOrderRulesFromJsonFile).

## How To
Create a Rule, and compile it.  Once compiled, rule can be executed over and over... with almost next to nothing execution time.  Just remember, compile may take some time depending on the comlexity of the final expression tree. In my projects, I load the rules at start-up (or at some frequent interval) and compile them, keep using them there after for the life of the application.

Unit tests and samples - probably good place to look for examples and ideas.  Also checkout the [Release Notes](/RELEASENOTES.MD) for changes.

## Constant Rule
Simple and most used rule is a Constant Rule.  Constant rule is a simple Func; that returns a constant value.
```c#
[Fact]
public void ConstantRuleOfTypeIntThatReturns55WhenValueIsSetTo55()
{
    var ruleReturning55 = new ConstantRule<int>{Value = "55"};
    var compileResult = ruleReturning55.Compile();
    compileResult.Should().BeTrue();
    _testOutputHelper.WriteLine($"{nameof(ruleReturning55)}:{Environment.NewLine}" +
                                $"{ruleReturning55.ExpressionDebugView()}");

    var value = ruleReturning55.Get();
    _testOutputHelper.WriteLine($"expected: 55 - actual: {value}");
    value.Should().Be(55);
}
```
and the expression tree it creates:
```
Expression: Convert(55, Int32)
Expression Tree:
||- System.Linq.Expressions.UnaryExpression.NodeType: Convert
||- System.Linq.Expressions.UnaryExpression.Type: System.Int32
||- System.Linq.Expressions.UnaryExpression.DebugView: (System.Int32)55
||- System.Linq.Expressions.UnaryExpression.CanReduce: False
||- unaryExpression.Method: 
||- unaryExpression.IsLifted: False
||- unaryExpression.IsLiftedToNull: False
||
||- unaryExpression.Operand:
|  |- System.Linq.Expressions.ConstantExpression.NodeType: Constant
|  |- System.Linq.Expressions.ConstantExpression.Type: System.Int32
|  |- System.Linq.Expressions.ConstantExpression.DebugView: 55
|  |- System.Linq.Expressions.ConstantExpression.CanReduce: False
||
||- innerExpression:
```
If for some odd reason, you need something that takes an object of whatever and always returs some constant; regardless of source - you can use ConstantRule<,>.  I needed it only once... may drop it someday - feels like it is not needed.

## Contains Value Rule
Got a list of values that you always check against; this rule can help - make sure to use desired equality comparer or let it use the default.
```c#
[Theory]
[InlineData("one", true)]
[InlineData("Two", true)]
[InlineData("THREE", true)]
[InlineData("fIVe", true)]
[InlineData("Six", true)]
[InlineData("seven", false)]
public void ContainsValueTestWithIgnoreCase(string valueToSearch, bool expectedResult)
{
    var containsRule = new ContainsValueRule<string>
    {
        EqualityComparer = StringComparer.OrdinalIgnoreCase,
        CollectionToSearch = {"one", "two", "three", "four", "five", "six"}
    };

    var compileResult = containsRule.Compile();
    compileResult.Should().BeTrue();
    _testOutputHelper.WriteLine($"{nameof(containsRule)}:{Environment.NewLine}" +
                                $"{containsRule.ExpressionDebugView()}");

    var containsValue = containsRule.ContainsValue(valueToSearch);
    _testOutputHelper.WriteLine($"expected: {expectedResult} - actual: {containsValue}");
    containsValue.Should().Be(expectedResult);
}
```
__Note__: Contains Value rule requires an equality comparer; if none provided it uses the default for the object.  Loading from file, fill class name and property name to create the comparer for you.  Remember, not all comparer comes as property and those are not supported.

## Expression Action Rule
Action and Func gives you most freedom to create appropriate lambda and use it as rule; when needed.  While other rules can be created on the fly during runtime (like values from a database or json file - see [RuleFactory](/src/RuleFactory)); these are compile time only.
```c#
[Fact]
public void UpdateGame1ScoreToNegative()
{
    var updateGameRankingRule = new ExpressionActionRule<Game>(g => ApplySomeRule(g));
    var compileResult = updateGameRankingRule.Compile();
    compileResult.Should().BeTrue();
    _testOutputHelper.WriteLine($"{nameof(updateGameRankingRule)}:{Environment.NewLine}" +
                                    $"{updateGameRankingRule.ExpressionDebugView()}");

    updateGameRankingRule.Execute(_game1);
    _game1.Ranking.Should().Be(int.MinValue);
}

private void ApplySomeRule(Game game) => game.Ranking = int.MinValue;
```

## Expression Func Rule
```c#
[Fact]
public void OneInOneOutParameterExpressionTest()
{
    // g => IIF(((g == null) OrElse (g.Players == null)), 0, g.Players.Count)
    var ruleReturningCountOfPlayers = new ExpressionFuncRule<Game, int>(
                                        g => (g == null || g.Players == null) ? 0 : g.Players.Count);
    var compileResult = ruleReturningCountOfPlayers.Compile();
    compileResult.Should().BeTrue();
    _testOutputHelper.WriteLine($"{nameof(ruleReturningCountOfPlayers)}:{Environment.NewLine}" +
                                    $"{ruleReturningCountOfPlayers.ExpressionDebugView()}");

    var executeResult = ruleReturningCountOfPlayers.Execute(_game1);
    executeResult.Should().BeOfType(typeof(int)).And.Be(_game1.Players.Count);
}
```
__Note__: mostly I use these expression rules for experiments and study the resultant expression tree

## Method Call Rule
Rule to call an object's method.  There are two flavors - a void call (which becomes an Action<>) or call that returns a value (which becomes a Func<,>).
```c#
[Fact]
public void CallToUpper()
{
    var rule = new MethodCallRule<string, string>{MethodToCall = "ToUpper"};
    var compileResult = rule.Compile();
    compileResult.Should().BeTrue();
    _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                $"{rule.ExpressionDebugView()}");

    var foo = "foo";
    var FOO = rule.Execute(foo);
    FOO.Should().Be("FOO");
}
```

### Static Method Call Rule:
Similar to method call rule; but works with static methods that returns a value.  Note, class name has to include complete namespace.
```c#
[Fact]
public void CallCreateGameStaticMethod2()
{
    //var game = Game.CreateGame("cool game");
    var rule = new StaticMethodCallRule<Game>
    {
        MethodClassName = "SampleModel.Game",
        MethodToCall = "CreateGame",
        MethodParameters = { new ConstantRule<string> { Value = "cool game" } }
    };

    var compileResult = rule.Compile();
    compileResult.Should().BeTrue();
    _testOutputHelper.WriteLine($"rule: {Environment.NewLine}" +
                                $"{rule.ExpressionDebugView()}");

    var game = rule.Execute();
    game.Should().NotBeNull();
    game.Name.Should().Be("cool game");
}
```

### Static Void Method Call Rule:
Same as Static call; but doesn't return any value, not it is generic.
```c#
[Fact]
public void CallStaticVoidMethod2()
{
    var rule = new StaticVoidMethodCallRule
    {
        MethodClassName = "SampleModel.Game",
        MethodToCall = "SomeVoidStaticMethod",
        MethodParameters = {new ConstantRule<int> {Value = "99"}}
    };

    var compileResult = rule.Compile();
    compileResult.Should().BeTrue();
    _testOutputHelper.WriteLine($"rule: {Environment.NewLine}" +
                                $"{rule.ExpressionDebugView()}");

    Game.SomeStaticIntValue = 0;
    rule.Execute();
    Game.SomeStaticIntValue.Should().Be(99);
}
```

## RegEx Rule
Helps call regular expression; like validating an name or email address.
```c#
[Theory]
[InlineData("ValidName", @"^[a-zA-Z]*$", true)]
[InlineData("BadName1", @"^[a-zA-Z]*$", false)]
[InlineData("AnotherBadName#", @"^[a-zA-Z]*$", false)]
[InlineData("BadName1", @"^[a-zA-Z0-9]*$", true)]
public void NameMatchesRegEx(string nameToUse, string regExToUse, bool expectedResult)
{
    var alphaRule = new RegExRule<Game>
    {
        ObjectToValidate = "Name",
        RegExToUse = regExToUse
    };

    var compileRuleResult = alphaRule.Compile();
    compileRuleResult.Should().BeTrue();
    _testOutputHelper.WriteLine($"{nameof(alphaRule)}:{Environment.NewLine}" +
                                $"{alphaRule.ExpressionDebugView()}");

    var game = new Game {Name = nameToUse};

    var executeResult = alphaRule.IsMatch(game);
    _testOutputHelper.WriteLine($"executeResult={executeResult}; expectedResult={expectedResult} " +
                                $"for nameToUse={nameToUse}");
    executeResult.Should().Be(expectedResult);
}
```

## Self Return Rule
This is a 'no-op' rule; returns whatever typed argument is given.
```c#
[Theory]
[InlineData(5)]
[InlineData(-5)]
[InlineData(int.MaxValue)]
public void IntSelfReturn(int someValue)
{
    var rule = new SelfReturnRule<int>();
    var compileResult = rule.Compile();
    compileResult.Should().BeTrue();
    _testOutputHelper.WriteLine($"selfReturnRule for Int:{Environment.NewLine}" +
                                $"{rule.ExpressionDebugView()}");

    var value = rule.Get(someValue);
    value.Should().Be(someValue);
}
```

## Update Value Rule
Want to update a value of an object; this is the rule.  I had been using this with Conditional Rule (see below)
```c#
[Fact]
public void UpdatePropertyFromAnotherRule()
{
    var game = new Game {Name = "game name"};
    var nameChangeRule = new UpdateValueRule<Game>
    {
        ObjectToUpdate = "Name",
        SourceDataRule = new ConstantRule<string> {Value = "name from constant rule"}
    };

    var compileResult = nameChangeRule.Compile();
    compileResult.Should().BeTrue();
    _testOutputHelper.WriteLine($"{nameof(nameChangeRule)}:{Environment.NewLine}" +
                                $"{nameChangeRule.ExpressionDebugView()}");

    _testOutputHelper.WriteLine($"before game.Name: {game.Name}");
    nameChangeRule.UpdateFieldOrPropertyValue(game);
    game.Name.Should().Be("name from constant rule");
    _testOutputHelper.WriteLine($"after game.Name: {game.Name}");
}
```

## Update Ref Value Rule
How about trying to update a variable by ref?  The rule comes in two flavors; you can define a source value with another rule or pass it as argument.
```c#
[Fact]
public void UpdateStringRef()
{
    // source value is fixed with a constant rule
    var rule = new UpdateRefValueRule<string>
    {
        SourceDataRule = new ConstantRule<string>{Value = "something"}
    };

    var compileResult = rule.Compile();
    compileResult.Should().BeTrue();
    _testOutputHelper.WriteLine($"UpdateRefValueRule<string>:{Environment.NewLine}" +
                                $"{rule.ExpressionDebugView()}");

    var string1 = "one";
    rule.RefUpdate(ref string1);
    string1.Should().Be("something");

    // source value shall come as argument
    var rule2 = new UpdateRefValueRule<string>();
    compileResult = rule2.Compile();
    compileResult.Should().BeTrue();
    _testOutputHelper.WriteLine($"UpdateRefValueRule<string, string>:{Environment.NewLine}" +
                                $"{rule2.ExpressionDebugView()}");

    string1 = null;
    rule2.RefUpdate(ref string1, "some other value");
    string1.Should().Be("some other value");
}
```

## Validation Rule
Most used rule - takes an object and return boolean.  Combined with children rules, like sub-rules; this can be used to validate a number of things all at once.  See [Sample 1](/samples/Sample1PlaceOrder) or unit tests for examples.
```c#
[Fact]
public void RuleToCheckIfAnIntegerMatchesRuleValueOrNot()
{
    var numberShouldBe5Rule = new ValidationRule<int>
    {
        ValueToValidateAgainst = new ConstantRule<int> {Value = "5"},
        OperatorToUse = "Equal",
        RuleError = new RuleError { Code="c1", Message = "number is not 5"}
    };
    var compileResult = numberShouldBe5Rule.Compile();
    compileResult.Should().BeTrue();
    _testOutputHelper.WriteLine($"{nameof(numberShouldBe5Rule)}:{Environment.NewLine}{numberShouldBe5Rule.ExpressionDebugView()}");

    var numberShouldNotBe5Rule = new ValidationRule<int>
    {
        ValueToValidateAgainst = new ConstantRule<int> { Value = "5" },
        OperatorToUse = "NotEqual",
        RuleError = new RuleError { Code = "c2", Message = "number is 5"}
    };
    compileResult = numberShouldNotBe5Rule.Compile();
    compileResult.Should().BeTrue();
    _testOutputHelper.WriteLine($"{nameof(numberShouldNotBe5Rule)}:{Environment.NewLine}{numberShouldNotBe5Rule.ExpressionDebugView()}");

    var ruleExecuteResult = numberShouldBe5Rule.IsValid(5);
    ruleExecuteResult.Should().BeTrue();
    _testOutputHelper.WriteLine($"with 5 {nameof(numberShouldBe5Rule)} execute result: {ruleExecuteResult}");

    ruleExecuteResult = numberShouldBe5Rule.IsValid(6);
    ruleExecuteResult.Should().BeFalse();
    _testOutputHelper.WriteLine($"with 6 {nameof(numberShouldBe5Rule)} failed. " +
                                    $"Error code={numberShouldBe5Rule.RuleError.Code}, " +
                                    $"message={numberShouldBe5Rule.RuleError.Message}");

    ruleExecuteResult = numberShouldNotBe5Rule.IsValid(6);
    ruleExecuteResult.Should().BeTrue();
    _testOutputHelper.WriteLine($"with 6 {nameof(numberShouldNotBe5Rule)} execute result: {ruleExecuteResult}");

    ruleExecuteResult = numberShouldNotBe5Rule.IsValid(5);
    ruleExecuteResult.Should().BeFalse();
    _testOutputHelper.WriteLine($"with 5 {nameof(numberShouldNotBe5Rule)} failed. " +
                                    $"Error code={numberShouldNotBe5Rule.RuleError.Code}, " +
                                    $"message={numberShouldNotBe5Rule.RuleError.Message}");
}
```

## Conditional Rule
Represents simple if-then-else construct of some flavors.  If-then action rule (ConditionalIfThActionRule) execute true action (rule) when the condition is true; else nothing.  If-then-action rule (ConditionalIfThElActionRule) similar, but has an extra false caluse.  And if-then-else func rule (ConditionalFuncRule) returns a value.
```c#
[Fact]
public void ConditionalRuleToUpdateNameToSomethingElse()
{
    var conditionalIfThElRule = new ConditionalIfThElActionRule<Game>
    {
        ConditionRule = new MethodCallRule<Game, bool>
        {
            ObjectToCallMethodOn = "Name",
            MethodToCall = "Equals",
            MethodParameters = { new ConstantRule<string> { Value = "some name" }, 
                new ConstantRule<StringComparison> { Value = "CurrentCultureIgnoreCase" }
            }
        },
        TrueRule = new UpdateValueRule<Game>
        {
            ObjectToUpdate = "Name",
            SourceDataRule = new ConstantRule<string> { Value = "true name" }
        },
        FalseRule = new UpdateValueRule<Game>
        {
            ObjectToUpdate = "Name",
            SourceDataRule = new ConstantRule<string> { Value = "false name" }
        }
    };

    var compileResult = conditionalIfThElRule.Compile();
    compileResult.Should().BeTrue();
    _testOutputHelper.WriteLine($"{nameof(conditionalIfThElRule)}:{Environment.NewLine}" +
                                $"{conditionalIfThElRule.ExpressionDebugView()}");

    var game = new Game { Name = "some name" };
    _testOutputHelper.WriteLine($"before game.Name: {game.Name}");
    conditionalIfThElRule.Execute(game);
    _testOutputHelper.WriteLine($"after game.Name: {game.Name}");
    game.Name.Should().Be("true name");

    conditionalIfThElRule.Execute(game);
    _testOutputHelper.WriteLine($"after after game.Name: {game.Name}");
    game.Name.Should().Be("false name");
}
```
resultant expression:
```
Expression: IIF(Invoke(Param_0 => Param_0.Name.Equals(Convert("some name", String), Convert(CurrentCultureIgnoreCase, StringComparison)), Param_0), Invoke(Param_0 => (Param_0.Name = Convert("true name", String)), Param_0), Invoke(Param_0 => (Param_0.Name = Convert("false name", String)), Param_0))
Expression Tree:
||- System.Linq.Expressions.FullConditionalExpression.NodeType: Conditional
||- System.Linq.Expressions.FullConditionalExpression.Type: System.String
||- System.Linq.Expressions.FullConditionalExpression.DebugView: .If (
    .Invoke (.Lambda #Lambda1<System.Func`2[SampleModel.Game,System.Boolean]>)($var1)
) {
    .Invoke (.Lambda #Lambda2<System.Func`2[SampleModel.Game,System.String]>)($var1)
} .Else {
    .Invoke (.Lambda #Lambda3<System.Func`2[SampleModel.Game,System.String]>)($var1)
}

.Lambda #Lambda1<System.Func`2[SampleModel.Game,System.Boolean]>(SampleModel.Game $var1) {
    .Call ($var1.Name).Equals(
        (System.String)"some name",
        (System.StringComparison).Constant<System.StringComparison>(CurrentCultureIgnoreCase))
}

.Lambda #Lambda2<System.Func`2[SampleModel.Game,System.String]>(SampleModel.Game $var1) {
    $var1.Name = (System.String)"true name"
}

.Lambda #Lambda3<System.Func`2[SampleModel.Game,System.String]>(SampleModel.Game $var1) {
    $var1.Name = (System.String)"false name"
}
||- System.Linq.Expressions.FullConditionalExpression.CanReduce: False
||
||- IfTrue:
|  |- System.Linq.Expressions.InvocationExpression1.NodeType: Invoke
|  |- System.Linq.Expressions.InvocationExpression1.Type: System.String
|  |- System.Linq.Expressions.InvocationExpression1.DebugView: .Invoke (.Lambda #Lambda1<System.Func`2[SampleModel.Game,System.String]>)($var1)

.Lambda #Lambda1<System.Func`2[SampleModel.Game,System.String]>(SampleModel.Game $var1) {
    $var1.Name = (System.String)"true name"
}
|  |- System.Linq.Expressions.InvocationExpression1.CanReduce: False
|  |
|  |- Expression:
|    |- System.Linq.Expressions.Expression1`1[System.Func`2[SampleModel.Game,System.String]].NodeType: Lambda
|    |- System.Linq.Expressions.Expression1`1[System.Func`2[SampleModel.Game,System.String]].Type: System.Func`2[SampleModel.Game,System.String]
|    |- System.Linq.Expressions.Expression1`1[System.Func`2[SampleModel.Game,System.String]].DebugView: .Lambda #Lambda1<System.Func`2[SampleModel.Game,System.String]>(SampleModel.Game $var1) {
    $var1.Name = (System.String)"true name"
}
|    |- System.Linq.Expressions.Expression1`1[System.Func`2[SampleModel.Game,System.String]].CanReduce: False
|    |
|    |- Parameters count: 1
|      |- System.Linq.Expressions.TypedParameterExpression.NodeType: Parameter
|      |- System.Linq.Expressions.TypedParameterExpression.Type: SampleModel.Game
|      |- System.Linq.Expressions.TypedParameterExpression.DebugView: $var1
|      |- System.Linq.Expressions.TypedParameterExpression.CanReduce: False
|      |- parameterExpression.Name: null
|      |- parameterExpression.IsByRef: False
|    |
|    |- Body [Assign]
|      |- System.Linq.Expressions.AssignBinaryExpression.NodeType: Assign
|      |- System.Linq.Expressions.AssignBinaryExpression.Type: System.String
|      |- System.Linq.Expressions.AssignBinaryExpression.DebugView: $var1.Name = (System.String)"true name"
|      |- System.Linq.Expressions.AssignBinaryExpression.CanReduce: False
|      |
|      |- Left:
|        |- System.Linq.Expressions.PropertyExpression.NodeType: MemberAccess
|        |- System.Linq.Expressions.PropertyExpression.Type: System.String
|        |- System.Linq.Expressions.PropertyExpression.DebugView: $var1.Name
|        |- System.Linq.Expressions.PropertyExpression.CanReduce: False
|        |- memberExpression.Member - field: 
|        |- memberExpression.Member - prop: System.String Name
|      |
|      |- Right:
|        |- System.Linq.Expressions.UnaryExpression.NodeType: Convert
|        |- System.Linq.Expressions.UnaryExpression.Type: System.String
|        |- System.Linq.Expressions.UnaryExpression.DebugView: (System.String)"true name"
|        |- System.Linq.Expressions.UnaryExpression.CanReduce: False
|        |- unaryExpression.Method: 
|        |- unaryExpression.IsLifted: False
|        |- unaryExpression.IsLiftedToNull: False
|        |
|        |- unaryExpression.Operand:
|          |- System.Linq.Expressions.ConstantExpression.NodeType: Constant
|          |- System.Linq.Expressions.ConstantExpression.Type: System.String
|          |- System.Linq.Expressions.ConstantExpression.DebugView: "true name"
|          |- System.Linq.Expressions.ConstantExpression.CanReduce: False
|        |
|        |- innerExpression:
|  |
|  |- Arguments count: 1
|    |- System.Linq.Expressions.TypedParameterExpression.NodeType: Parameter
|    |- System.Linq.Expressions.TypedParameterExpression.Type: SampleModel.Game
|    |- System.Linq.Expressions.TypedParameterExpression.DebugView: $var1
|    |- System.Linq.Expressions.TypedParameterExpression.CanReduce: False
|    |- parameterExpression.Name: null
|    |- parameterExpression.IsByRef: False
||
||- IfFalse:
|  |- System.Linq.Expressions.InvocationExpression1.NodeType: Invoke
|  |- System.Linq.Expressions.InvocationExpression1.Type: System.String
|  |- System.Linq.Expressions.InvocationExpression1.DebugView: .Invoke (.Lambda #Lambda1<System.Func`2[SampleModel.Game,System.String]>)($var1)

.Lambda #Lambda1<System.Func`2[SampleModel.Game,System.String]>(SampleModel.Game $var1) {
    $var1.Name = (System.String)"false name"
}
|  |- System.Linq.Expressions.InvocationExpression1.CanReduce: False
|  |
|  |- Expression:
|    |- System.Linq.Expressions.Expression1`1[System.Func`2[SampleModel.Game,System.String]].NodeType: Lambda
|    |- System.Linq.Expressions.Expression1`1[System.Func`2[SampleModel.Game,System.String]].Type: System.Func`2[SampleModel.Game,System.String]
|    |- System.Linq.Expressions.Expression1`1[System.Func`2[SampleModel.Game,System.String]].DebugView: .Lambda #Lambda1<System.Func`2[SampleModel.Game,System.String]>(SampleModel.Game $var1) {
    $var1.Name = (System.String)"false name"
}
|    |- System.Linq.Expressions.Expression1`1[System.Func`2[SampleModel.Game,System.String]].CanReduce: False
|    |
|    |- Parameters count: 1
|      |- System.Linq.Expressions.TypedParameterExpression.NodeType: Parameter
|      |- System.Linq.Expressions.TypedParameterExpression.Type: SampleModel.Game
|      |- System.Linq.Expressions.TypedParameterExpression.DebugView: $var1
|      |- System.Linq.Expressions.TypedParameterExpression.CanReduce: False
|      |- parameterExpression.Name: null
|      |- parameterExpression.IsByRef: False
|    |
|    |- Body [Assign]
|      |- System.Linq.Expressions.AssignBinaryExpression.NodeType: Assign
|      |- System.Linq.Expressions.AssignBinaryExpression.Type: System.String
|      |- System.Linq.Expressions.AssignBinaryExpression.DebugView: $var1.Name = (System.String)"false name"
|      |- System.Linq.Expressions.AssignBinaryExpression.CanReduce: False
|      |
|      |- Left:
|        |- System.Linq.Expressions.PropertyExpression.NodeType: MemberAccess
|        |- System.Linq.Expressions.PropertyExpression.Type: System.String
|        |- System.Linq.Expressions.PropertyExpression.DebugView: $var1.Name
|        |- System.Linq.Expressions.PropertyExpression.CanReduce: False
|        |- memberExpression.Member - field: 
|        |- memberExpression.Member - prop: System.String Name
|      |
|      |- Right:
|        |- System.Linq.Expressions.UnaryExpression.NodeType: Convert
|        |- System.Linq.Expressions.UnaryExpression.Type: System.String
|        |- System.Linq.Expressions.UnaryExpression.DebugView: (System.String)"false name"
|        |- System.Linq.Expressions.UnaryExpression.CanReduce: False
|        |- unaryExpression.Method: 
|        |- unaryExpression.IsLifted: False
|        |- unaryExpression.IsLiftedToNull: False
|        |
|        |- unaryExpression.Operand:
|          |- System.Linq.Expressions.ConstantExpression.NodeType: Constant
|          |- System.Linq.Expressions.ConstantExpression.Type: System.String
|          |- System.Linq.Expressions.ConstantExpression.DebugView: "false name"
|          |- System.Linq.Expressions.ConstantExpression.CanReduce: False
|        |
|        |- innerExpression:
|  |
|  |- Arguments count: 1
|    |- System.Linq.Expressions.TypedParameterExpression.NodeType: Parameter
|    |- System.Linq.Expressions.TypedParameterExpression.Type: SampleModel.Game
|    |- System.Linq.Expressions.TypedParameterExpression.DebugView: $var1
|    |- System.Linq.Expressions.TypedParameterExpression.CanReduce: False
|    |- parameterExpression.Name: null
|    |- parameterExpression.IsByRef: False
```

## Block Rule
Idea was to group couple of rules together and execute as one rule. As of now, we only have Action Block Rule. This might get expanded to cover more scenarios.

### Action Block Rule
Like execute couple of update value rule to update various properties of a game object (see unit tests) - glue together bunch of action rules where all the rules take one parameter.
```c#
[Fact]
public void UpdateMultiplePropertiesOfaGameObject()
{
    var nameChangeRule = new UpdateValueRule<Game>
    {
        ObjectToUpdate = "Name",
        SourceDataRule = new ConstantRule<string> {Value = "some fancy name"}
    };
    var rankingChangeRule = new UpdateValueRule<Game>
    {
        ObjectToUpdate = "Ranking",
        SourceDataRule = new ConstantRule<int>{Value = "1000"}
    };
    var descriptionChangeRule = new UpdateValueRule<Game>
    {
        ObjectToUpdate = "Description",
        SourceDataRule = new ConstantRule<string>{Value = "some cool description"}
    };

    var blockRule = new ActionBlockRule<Game>();
    blockRule.Rules.Add(nameChangeRule);
    blockRule.Rules.Add(rankingChangeRule);
    blockRule.Rules.Add(descriptionChangeRule);

    var compileResult = blockRule.Compile();
    compileResult.Should().BeTrue();

    _testOutputHelper.WriteLine(blockRule.ExpressionDebugView());

    var game = new Game();
    blockRule.Exectue(game);
    _testOutputHelper.WriteLine($"game object updated:{Environment.NewLine}{game}");
}
```
resultant expression:
```
Expression: { ... }
Expression Tree:
||- System.Linq.Expressions.Block3.NodeType: Block
||- System.Linq.Expressions.Block3.Type: System.String
||- System.Linq.Expressions.Block3.DebugView: .Block() {
    $var1.Name = (System.String)"some fancy name";
    $var1.Ranking = (System.Int32)1000;
    $var1.Description = (System.String)"some cool description"
}
.
.
```

# [Rule Factory](/src/RuleFactory)
Library to help create rules using bunch of static methods; or use custom json converter to read-write rules from-to json documents.  See [Sample 4](/samples/Sample4RuleCreateMadeEasy) for a simple step-by-step tutorial.

# Samples
## [Sample1 Place an Order](/samples/Sample1PlaceOrder)
Simple console application that have handful of rules to validate if an order is valid or not.

## [Sample 2](/samples/Sample2PlaceOrderRulesFromJsonFile)
Same as sample 1, but loads the rules from a text file using custom JSON converter.

## [Sample 3](/samples/Sample3PlaceOrderRulesUsingFactory)
Same as sample 1, but rules were created using factory methods.

## [Sample 4](/samples/Sample4RuleCreateMadeEasy)
Quick how-to sample/tutorial.

# Misc
 - [How to Build/Publish](/HOWTO.MD)

# Thanks to everyone
Started playing with Expression Tree and had some sample/example code going for some years.  Some of the ideas for rule engine came from [MicroRuleEngine](https://github.com/runxc1/MicroRuleEngine); and [AutoMapper](https://github.com/AutoMapper/AutoMapper) source code.  [CoreFx](https://github.com/dotnet/corefx) code and unit tests - an excellent way to study and understand what's going on under the hood with Expression tree.  Thanks to many more blogs, example codes, SO answers, .....  Thought I would just glue my experiments together and release it into the wild.  Hope this helps...
