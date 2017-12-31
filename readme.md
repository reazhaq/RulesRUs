# Rules R Us
Couldn't come up with a name - this came to mind.
Built using Expression Tree; this helps create-compile-execute simple business rules at runtime; or just wrap some expressions with the same look and feel while writing code.

## How To
Create a Rule, and compile it.  Once compiled, rule can be executed at any time.

## Constant Rule
Constant rule is a simple Func; that returns a constant value.
```
    var ruleReturning55 = new ConstantRule<int>{Value = "55"};
    var compileResult = ruleReturning55.Compile();

    var value = ruleReturning55.Get(); // value is 55
```

## Contains Value Rule

## Expression Action Rule

## Expression Func Rule

## Method Call Rule

## RegEx Rule

## Validation Rule

