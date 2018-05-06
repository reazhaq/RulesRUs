# Rules R Us
Couldn't come up with a name - this came to mind.
Built using Expression Tree; this helps create-compile-execute simple business rules at runtime; or just wrap some expressions with the same look and feel while writing code.

## How To
Create a Rule, and compile it.  Once compiled, rule can be executed over and over... with almost next to nothing execution time.  Just remember, compile may take some time depending on the comlexity of the expression tree. I load the rules at start-up (or at some frequent interval) and compile them, keep using them there after.

Unit tests - probably good place to look for examples and ideas.

## Constant Rule
Most simple rule, and most used rule is a Constant Rule.  Constant rule is a simple Func; that returns a constant value.
```c#
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
If for some odd reason, you need something that takes an object of whatever and always returs some constant (regardless) - you can use ConstantRule<,>.  I needed it only once... may drop it someday - feels like it is not needed.

## Contains Value Rule
Got a list of values that you always check against; this rule can help - make sure to use desired equality comparer.
```c#
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
__Note__: Contains Value rule requires an equality comparer; if none provided it uses the default for the object.  Loading from file, fill class name and property name to create the comparer for you.  Remember, not all comparer comes as property and those are not supported.

## Expression Action Rule
Action and Func gives you most freedom to create appropriate lambda and use it as rule; when needed.  While other rules can be created on the fly during runtime (like values from a database or json file - see [RuleFactory](/src/RuleFactory)); these are compile time only.
```c#
    var updateGameRankingRule = new ExpressionActionRule<Game>(g => ApplySomeRule(g));
```

## Expression Func Rule
```c#
    // g => IIF(((g == null) OrElse (g.Players == null)), 0, g.Players.Count)
    var ruleReturningCountOfPlayers = new ExpressionFuncRules<Game, int>(
                                        g => (g == null || g.Players == null) ? 0 : g.Players.Count);
```
__Note__: mostly I use these expression rules for experiments

## Method Call Rule
Rule to call an object's method.  There are two flavors - a void call (which becomes an Action<>) or call that returns a value (which becomes a Func<,>).
```c#
    var rule = new MethodCallRule<string, string>{MethodToCall = "ToUpper"};
    var compileResult = rule.Compile();
    compileResult.Should().BeTrue();
    _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                $"{rule.ExpressionDebugView()}");

    var foo = "foo";
    var FOO = rule.Execute(foo);
    FOO.Should().Be("FOO");
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

## Ref Update Value Rule
How about trying to update a variable by ref?  The rule comes in two flavors; you can define a source value with another rule or pass it as argument.
```c#
        [Fact]
        public void UpdateStringRef()
        {
            // source value is fixed with a constant rule
            var rule = new RefUpdateValueRule<string>
            {
                SourceDataRule = new ConstantRule<string>{Value = "something"}
            };

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"RefUpdateValueRule<string>:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var string1 = "one";
            rule.RefUpdate(ref string1);
            string1.Should().Be("something");

            // source value shall come as argument
            var rule2 = new RefUpdateValueRule<string>();
            compileResult = rule2.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"RefUpdateValueRule<string, string>:{Environment.NewLine}" +
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
||- Expression Type: FullConditionalExpression
||- Expression.NodeType: Conditional
||- Expression.DebugView: .If (
    .Invoke (.Lambda #Lambda1<System.Func`2[RuleEngine.Tests.Model.Game,System.Boolean]>)($var1)
) {
    .Invoke (.Lambda #Lambda2<System.Func`2[RuleEngine.Tests.Model.Game,System.String]>)($var1)
} .Else {
    .Invoke (.Lambda #Lambda3<System.Func`2[RuleEngine.Tests.Model.Game,System.String]>)($var1)
}

.Lambda #Lambda1<System.Func`2[RuleEngine.Tests.Model.Game,System.Boolean]>(RuleEngine.Tests.Model.Game $var1) {
    .Call ($var1.Name).Equals(
        (System.String)"some name",
        (System.StringComparison).Constant<System.StringComparison>(CurrentCultureIgnoreCase))
}

.Lambda #Lambda2<System.Func`2[RuleEngine.Tests.Model.Game,System.String]>(RuleEngine.Tests.Model.Game $var1) {
    $var1.Name = (System.String)"true name"
}

.Lambda #Lambda3<System.Func`2[RuleEngine.Tests.Model.Game,System.String]>(RuleEngine.Tests.Model.Game $var1) {
    $var1.Name = (System.String)"false name"
}
```

# [Rule Factory](/src/RuleFactory)
Library to help create rules using bunch of static methods; or use custom json converter to read-write rules from-to json documents.

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
 - [Release Notes](/RELEASENOTES.MD)

# Thanks to everyone
Started playing with Expression Tree and had some sample/example code going for some years.  Some of the ideas for rule engine came from [MicroRuleEngine](https://github.com/runxc1/MicroRuleEngine); and [AutoMapper](https://github.com/AutoMapper/AutoMapper) source code.  [CoreFx](https://github.com/dotnet/corefx) code and unit tests - an excellent way to study and understand what's going on under the hood with Expression tree.  Thanks to many more blogs, example codes, SO answers, .....  Thought I would just glue my experiments together and release it into the wild.  Hope this helps...
