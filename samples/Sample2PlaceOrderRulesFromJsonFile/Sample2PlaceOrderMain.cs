using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using RuleEngine.Interfaces.Rules;
using RuleEngine.Rules;
using RuleFactory;
using Sample2PlaceOrderRulesFromJsonFile.Model;

namespace Sample2PlaceOrderRulesFromJsonFile
{
    class Sample2PlaceOrderMain
    {
        private static readonly List<Rule> OrderRules = new List<Rule>();
        private static string _ruleFileName = "order-rule.json";

        static void Main(string[] args)
        {
            Console.WriteLine("Validate an Order example");
            Console.WriteLine("Rule: order cannot be null, customer can't be null and product can't be null");
            Console.WriteLine("Rule: first name can't be null and has to be 3+ chars");
            Console.WriteLine("Rule: last name can't be null and has to be 4+ chars");
            Console.WriteLine("Rule: product id has to be positive or name has to be 5+ chars");

            LoadAndCompileRules();

            var commandLineApplication = new CommandLineApplication(throwOnUnexpectedArg:false);
            commandLineApplication.HelpOption("-? | -h | --help");
            var orderCommand = commandLineApplication.Option(
                 "-o | --order", "Place an order", CommandOptionType.NoValue
            );

            commandLineApplication.OnExecute(() =>
            {
                if (orderCommand.HasValue())
                    PlaceAnOrder();

                return 0;
            });

            commandLineApplication.Execute(args);
        }

        private static void LoadAndCompileRules()
        {
            if (LoadFromFile())
                return;

            // this is mainly used to create rules and save them into a file; to be used during subsequent execution
            var orderRule = new ValidationRule<Order>
            {
                OperatorToUse = "NotEqual",
                ValueToValidateAgainst = new ConstantRule<Order> {Value = null},
                RuleError = new RuleError { Code = "c1", Message = "order can't be null"}
            };
            if(orderRule.Compile())
                OrderRules.Add(orderRule);

            var orderCustomerAndProductCannotBeNullRule = new ValidationRule<Order>
            {
                OperatorToUse = "AndAlso",
                RuleError = new RuleError { Code = "c2", Message = "Customer and/or Product can't be null"},
                ChildrenRules =
                {
                    new ValidationRule<Order>{OperatorToUse = "NotEqual", ObjectToValidate = "Customer", ValueToValidateAgainst = new ConstantRule<Customer>{Value = "null"}},
                    new ValidationRule<Order>{OperatorToUse = "NotEqual", ObjectToValidate = "Product", ValueToValidateAgainst = new ConstantRule<Product>{Value = "null"}}
                }
            };
            if(orderCustomerAndProductCannotBeNullRule.Compile())
                OrderRules.Add(orderCustomerAndProductCannotBeNullRule);

            var orderCustomerFirstNameRule = new ValidationRule<Order>
            {
                OperatorToUse = "AndAlso",
                RuleError = new RuleError { Code = "c3", Message = "first name can't be null/empty and has to be 3+ chars long"},
                ChildrenRules =
                {
                    new ValidationRule<Order>{OperatorToUse = "NotEqual", ObjectToValidate = "Customer.FirstName", ValueToValidateAgainst = new ConstantRule<string>{Value = "null"}},
                    new ValidationRule<Order>{OperatorToUse = "GreaterThan", ObjectToValidate = "Customer.FirstName.Length", ValueToValidateAgainst = new ConstantRule<int>{Value = "2"}}
                }
            };
            if(orderCustomerFirstNameRule.Compile())
                OrderRules.Add(orderCustomerFirstNameRule);

            var orderCustomerLastNameRule = new ValidationRule<Order>
            {
                OperatorToUse = "AndAlso",
                RuleError = new RuleError { Code = "c4", Message = "last name can't be null/empty and has to be 4+ chars long"},
                ChildrenRules =
                {
                    new ValidationRule<Order>{OperatorToUse = "NotEqual", ObjectToValidate = "Customer.LastName", ValueToValidateAgainst = new ConstantRule<string>{Value = "null"}},
                    new ValidationRule<Order>{OperatorToUse = "GreaterThan", ObjectToValidate = "Customer.LastName.Length", ValueToValidateAgainst = new ConstantRule<int>{Value = "3"}}
                }
            };
            if(orderCustomerLastNameRule.Compile())
                OrderRules.Add(orderCustomerLastNameRule);

            var orderProductIdPositiveOrNameGreaterThan5 = new ValidationRule<Order>
            {
                OperatorToUse = "OrElse",
                RuleError = new RuleError
                {
                    Code = "c5",
                    Message = "product id must be greater than zero or name has to be 5+ chars"
                },
                ChildrenRules =
                {
                    new ValidationRule<Order>{OperatorToUse = "GreaterThan", ObjectToValidate = "Product.Id", ValueToValidateAgainst = new ConstantRule<int>{Value = "0"}},
                    new ValidationRule<Order>
                    {
                        OperatorToUse = "AndAlso",
                        ChildrenRules =
                        {
                            new ValidationRule<Order>{OperatorToUse = "NotEqual", ObjectToValidate = "Product.Name", ValueToValidateAgainst = new ConstantRule<string>{Value = "null"}},
                            new ValidationRule<Order>{OperatorToUse = "GreaterThan", ObjectToValidate = "Product.Name.Length", ValueToValidateAgainst = new ConstantRule<int>{Value = "4"}}
                        }
                    }
                }
            };
            if(orderProductIdPositiveOrNameGreaterThan5.Compile())
                OrderRules.Add(orderProductIdPositiveOrNameGreaterThan5);

            SaveRulesToFile();
        }

        // save as new-line json format; just because I wanted to
        private static void SaveRulesToFile()
        {
            var jsonConverter = new JsonConverterForRule();
            using (var file = new StreamWriter(_ruleFileName))
            foreach (var orderRule in OrderRules)
            {
                var json = JsonConvert.SerializeObject(orderRule, jsonConverter);
                file.WriteLine(json);
            }
        }

        private static bool LoadFromFile()
        {
            if (!File.Exists(_ruleFileName)) return false;

            var jsonConverter = new JsonConverterForRule();
            using (var stream = File.OpenText(_ruleFileName))
            {
                var line = stream.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    var jsonRule = JsonConvert.DeserializeObject<Rule>(line, jsonConverter);
                    if(jsonRule.Compile())
                        OrderRules.Add(jsonRule);

                    line = stream.ReadLine();
                }
            }

            return true;
        }

        private static void PlaceAnOrder()
        {
            Console.WriteLine($"Ready to place an order");
            Console.Write("First Name (at least 3 chars): ");
            var firstName = Console.ReadLine();
            Console.Write("Last Name (at least 4 chars): ");
            var lastName = Console.ReadLine();
            Console.Write("Product Id (number): ");
            var productId = Convert.ToInt32(Console.ReadLine());
            Console.Write("Product Name (at least 5 chars): ");
            var productName = Console.ReadLine();

            var order = new Order
            {
                Customer = new Customer {FirstName = firstName, LastName = lastName},
                Product = new Product {Id = productId, Name = productName}
            };

            Console.WriteLine($"Order = {order}");
            var ruleErrors = new List<RuleError>();
            foreach (var orderRule in OrderRules)
            {
                if (orderRule is IValidationRule<Order> rule && !(rule.IsValid(order)))
                    ruleErrors.Add(orderRule.RuleError);
            }

            Console.WriteLine("Errors found:");
            Console.WriteLine(JsonConvert.SerializeObject(ruleErrors, Formatting.Indented));
            Console.WriteLine("hit any key to end");
            Console.ReadKey();
        }
    }
}
