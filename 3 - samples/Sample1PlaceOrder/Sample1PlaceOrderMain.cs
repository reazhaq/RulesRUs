using System;
using System.Collections.Generic;
using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using RuleEngine.Rules;
using Sample1PlaceOrder.Model;

namespace Sample1PlaceOrder
{
    class Sample1PlaceOrderMain
    {
        private static readonly List<Rule> _orderRules = new List<Rule>();

        static void Main(string[] args)
        {
            Console.WriteLine("Simple Place Order example");

            LoadAndCompileRules();

            var commmandLineApplication = new CommandLineApplication(throwOnUnexpectedArg:false);
            commmandLineApplication.HelpOption("-? | -h | --help");
            var orderCommand = commmandLineApplication.Option(
                 "-o | --order", "Place an order", CommandOptionType.NoValue
            );

            commmandLineApplication.OnExecute(() =>
            {
                if (orderCommand.HasValue())
                    PlaceAnOrder();

                return 0;
            });

            commmandLineApplication.Execute(args);
        }

        private static void LoadAndCompileRules()
        {
            var orderRule = new ValidationRule<Order>
            {
                OperatorToUse = "NotEqual",
                ValueToValidateAgainst = new ConstantRule<Order> {Value = null},
                RuleError = new RuleError { Code = "c1", Message = "order can't be null"}
            };
            if(orderRule.Compile())
                _orderRules.Add(orderRule);

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
                _orderRules.Add(orderCustomerAndProductCannotBeNullRule);

            var orderCustomerFirstNameRule = new ValidationRule<Order>
            {
                OperatorToUse = "AndAlso",
                RuleError = new RuleError { Code = "c3", Message = "first name can't be null and has to be 3+ chars long"},
                ChildrenRules =
                {
                    new ValidationRule<Order>{OperatorToUse = "NotEqual", ObjectToValidate = "Customer.FirstName", ValueToValidateAgainst = new ConstantRule<string>{Value = "null"}},
                    new ValidationRule<Order>{OperatorToUse = "GreaterThan", ObjectToValidate = "Customer.FirstName.Length", ValueToValidateAgainst = new ConstantRule<int>{Value = "2"}}
                }
            };
            if(orderCustomerFirstNameRule.Compile())
                _orderRules.Add(orderCustomerFirstNameRule);

            var orderCustomerLastNameRule = new ValidationRule<Order>
            {
                OperatorToUse = "AndAlso",
                RuleError = new RuleError { Code = "c4", Message = "last name can't be null and has to be 4+ chars long"},
                ChildrenRules =
                {
                    new ValidationRule<Order>{OperatorToUse = "NotEqual", ObjectToValidate = "Customer.LastName", ValueToValidateAgainst = new ConstantRule<string>{Value = "null"}},
                    new ValidationRule<Order>{OperatorToUse = "GreaterThan", ObjectToValidate = "Customer.LastName.Length", ValueToValidateAgainst = new ConstantRule<int>{Value = "3"}}
                }
            };
            if(orderCustomerLastNameRule.Compile())
                _orderRules.Add(orderCustomerLastNameRule);

            var orderProductIdPositiveOrNameGreaterThan5 = new ValidationRule<Order>
            {
                OperatorToUse = "OrElse",
                RuleError = new RuleError { Code = "c5", Message = "id must be greater than zero or name has to be 5+ chars"},
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
                _orderRules.Add(orderProductIdPositiveOrNameGreaterThan5);
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
            foreach (var orderRule in _orderRules)
            {
                if (orderRule is ValidationRule<Order> && !((orderRule as ValidationRule<Order>).IsValid(order)))
                    ruleErrors.Add(orderRule.RuleError);
            }

            Console.WriteLine("Rule: order cannot be null, customer can't be null and product can't be null");
            Console.WriteLine("Rule: first name can't be null and has to be 3+ chars");
            Console.WriteLine("Rule: last name can't be null and has to be 4+ chars");
            Console.WriteLine("Rule: product id has to be positive or name has to be 5+ chars");
            Console.WriteLine("Errors found:");
            Console.WriteLine(JsonConvert.SerializeObject(ruleErrors, Formatting.Indented));
        }
    }
}
