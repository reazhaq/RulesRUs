# Version 8.0.0
- Everything upgraded to dotnet-8
- dependent pkgs updated to latest (as of Jan 18, 2024)

# Version 1.3.0
- New rule:
  - Func Block rule; simple way to combine number of similar rules into one rule - return the result of the last rule's result
  - Func Block rule factory; works with Json converter

# Version 1.2.0
- New rule:
  - Action Block rule; simple way to combine number of similar rules into one rule
  - Action Block rule factory; works with Json converter

# Version 1.1.1
- New rules:
  - :bug: utility method bug fix - get method info was vulnerable to some null parameters

# Version 1.1.0
- New rules:
  - Self Return Rule; returns the same typed value
  - Ref Update Value Rule; can be used to update a variable by ref
  - Static Method Call Rule; that returns a value
  - Static Void Call Rule
- Updated expression tracing; added additional details for some of the expression tree.  More to come in the future
- Renamed CustomRuleJsonConverter to JsonConveterForRule... if you upgrade, just replace the name - everything else is same

# Version 1.0.1
- :bug: bug fix - Conditional Func Rule (two bounding types) were only working for string

# Version 1.0

