# Quick Start - How To
How to create a rule - it may seem too much.  But breaking the problems into small parts and gluing them together is the way to go.  It says tutorial, but it is more like quick and dirty 'get started' how-to.  Once a rule is created, either instantiating classes directly or using factory methods, save them somewhere as JSON [using custom json converter] - you are home free.

Step-by-step to create a rule - we shall create the following rule using Factory methods.
```
var orderCustomerAndProductCannotBeNullRule = new ValidationRule<Order>
{
    OperatorToUse = "AndAlso",
    RuleError = new RuleError { Code = "c2", Message = "Customer and/or Product can't be null"},
    ChildrenRules =
    {
        new ValidationRule<Order>
        {
            OperatorToUse = "NotEqual",
            ObjectToValidate = "Customer",
            ValueToValidateAgainst = new ConstantRule<Customer>{Value = "null"}
        },
        new ValidationRule<Order>
        {
            OperatorToUse = "NotEqual",
            ObjectToValidate = "Product",
            ValueToValidateAgainst = new ConstantRule<Product>{Value = "null"}}
    }
};
```

## Constant Rule
Most used rule which turns into a simple [typed] Func<T> that returns a constant value.  Mostly used as part of other rules.
```
var nullCustomer = ConstantRulesFactory.CreateConstantRule<Customer>("null");
var nullProduct = ConstantRulesFactory.CreateConstantRule<Product>("null");
```

## Validation Rule
Validation rule returns bool.  Create two children rules that go with parent validation rule.  Children rules:
```
var child1Rule = ValidationRulesFactory.CreateValidationRule<Order>(o => o.Customer,
    LogicalOperatorAtTheRootLevel.NotEqual, nullCustomer);

var child2Rule = ValidationRulesFactory.CreateValidationRule<Order>(o => o.Product,
    LogicalOperatorAtTheRootLevel.NotEqual, nullProduct);
```
Now glue them together to create the parent rule.
```
var orderCustomerAndProductCannotBeNullRule =
    ValidationRulesFactory.CreateValidationRule<Order>(ChildrenBindingOperator.AndAlso,
        new List<Rule> {child1Rule, child2Rule});
```
You can attach a rule error; to get details to show to the user
```
orderCustomerAndProductCannotBeNullRule.RuleError = new RuleError
{
    Code = "c2", Message = "Customer and/or Product can't be null"
};
```
Once compiled, you can see the underlying expression tree:
```
  binaryExpressionBody: (Param_0.Customer != null)
  binaryExpressionBody: (Param_0.Product != null)
ExpressionForThisRule ready to compile:
||- binaryExpression.NodeType: AndAlso
||- binaryExpression.DebugView: $var1.Customer != null && $var1.Product != null
|  |- binaryExpression.NodeType: NotEqual
|  |- binaryExpression.DebugView: $var1.Customer != null
|    |- memberExpression.NodeType: MemberAccess
|    |- memberExpression.DebugView: $var1.Customer
|    |- memberExpression.Member - field: 
|    |- memberExpression.Member - prop: SampleModel.Customer Customer
|    |- constantExpression.Value: null
|    |- constantExpression.Type: SampleModel.Customer
|    |- constantExpression.DebugView: null
|  |- binaryExpression.NodeType: NotEqual
|  |- binaryExpression.DebugView: $var1.Product != null
|    |- memberExpression.NodeType: MemberAccess
|    |- memberExpression.DebugView: $var1.Product
|    |- memberExpression.Member - field: 
|    |- memberExpression.Member - prop: SampleModel.Product Product
|    |- constantExpression.Value: null
|    |- constantExpression.Type: SampleModel.Product
|    |- constantExpression.DebugView: null
```

## Use it
with a successful compile - you can use it over and over with almost next to no execution time - just like a dev time compiled code
```
var compiledResult = orderCustomerAndProductCannotBeNullRule.Compile();
Debug.WriteLine($"compiledResult: {compiledResult}"); // true

var order = new Order();
var ruleResult = orderCustomerAndProductCannotBeNullRule.IsValid(order);
Debug.WriteLine($"ruleResult: {ruleResult}"); // false

// add a customer object
order.Customer = new Customer();
ruleResult = orderCustomerAndProductCannotBeNullRule.IsValid(order);
Debug.WriteLine($"ruleResult: {ruleResult}"); // false - because product is null

// add a product object
order.Product = new Product();
ruleResult = orderCustomerAndProductCannotBeNullRule.IsValid(order);
Debug.WriteLine($"ruleResult: {ruleResult}"); // true - because both are non-null
```

## Convert to Json
You can convert this to Json doc and save it or later use.
```
var converter = new JsonConverterForRule();
var jsonDoc = JsonConvert.SerializeObject(orderCustomerAndProductCannotBeNullRule, Formatting.Indented, converter);
Debug.WriteLine(jsonDoc);
```
And the Json looks like:
```
{
  "RuleType": "ValidationRule`1",
  "BoundingTypes": "SampleModel.Order",
  "ChildrenRules": [
    {
      "RuleType": "ValidationRule`1",
      "BoundingTypes": "SampleModel.Order",
      "ObjectToValidate": "Customer",
      "ChildrenRules": [],
      "Id": 0,
      "ValueToValidateAgainst": {
        "RuleType": "ConstantRule`1",
        "BoundingTypes": "SampleModel.Customer",
        "Value": "null",
        "Id": 0
      },
      "OperatorToUse": "NotEqual"
    },
    {
      "RuleType": "ValidationRule`1",
      "BoundingTypes": "SampleModel.Order",
      "ObjectToValidate": "Product",
      "ChildrenRules": [],
      "Id": 0,
      "ValueToValidateAgainst": {
        "RuleType": "ConstantRule`1",
        "BoundingTypes": "SampleModel.Product",
        "Value": "null",
        "Id": 0
      },
      "OperatorToUse": "NotEqual"
    }
  ],
  "Id": 0,
  "RuleError": {
    "Code": "c2",
    "Message": "Customer and/or Product can't be null"
  },
  "OperatorToUse": "AndAlso"
}
```

## Note
Introduing null check in the chain is very important.  If you call the rule IsValid and pass a null parameter - it is going to fail with null ref exception.  Just add a Order null check validation rule as the first child rule to take care of it.  Remember, these rules are loaded at run time; and null ref exception or any other silly error can bring down your app.