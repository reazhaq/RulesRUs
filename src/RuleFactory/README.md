# Rule Factory
Creating rules, and remembering all the details (like ObjectToValidate is case sensitive; or correct spelling for OperatorToUse) can be overwhelming.  This utility is collection of various tools to help ease creating, saving and loading rules.

# Factory Methods
Please consult various unit tests for examples.  Most of these factory [static] methods are self explanatory; like use CreateConstantRule static method, pass a string for the value to create a constant rule.  Or some rules (like conditional rule) is just an aggregation of 2 or three other rules - so create those rules in order.

# Custom Json Converter
To help save and load rules [to and from] json.
 