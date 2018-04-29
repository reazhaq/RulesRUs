using System;
using System.Collections.Generic;
using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using RuleEngine.Interfaces.Rules;
using RuleEngine.Rules;
using RuleFactory.RulesFactory;
using Sample3PlaceOrderRulesUsingFactory.Model;

namespace Sample3PlaceOrderRulesUsingFactory
{
    class Sample3PlaceOrderMain
    {
        private static readonly List<Rule> OrderRules = new List<Rule>();

        static void Main(string[] args)
        {
            Console.WriteLine("Simple Place Order example");
            Console.WriteLine("Rule: order cannot be null, customer can't be null and product can't be null");
            Console.WriteLine("Rule: first name can't be null and has to be 3+ chars");
            Console.WriteLine("Rule: last name can't be null and has to be 4+ chars");
            Console.WriteLine("Rule: product id has to be positive or name has to be 5+ chars");

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
            var nullOrder = ConstantRulesFactory.CreateConstantRule<Order>("null");
            var orderRule =
                ValidationRulesFactory.CreateValidationRule<Order>(LogicalOperatorAtTheRootLevel.NotEqual, nullOrder);
            orderRule.RuleError = new RuleError {Code = "c1", Message = "order can't be null"};
            if(orderRule.Compile())
                OrderRules.Add(orderRule);

            var nullCustomer = ConstantRulesFactory.CreateConstantRule<Customer>("null");
            var nullProduct = ConstantRulesFactory.CreateConstantRule<Product>("null");
            var child1Rule = ValidationRulesFactory.CreateValidationRule<Order>(o => o.Customer,
                LogicalOperatorAtTheRootLevel.NotEqual, nullCustomer);
            var child2Rule = ValidationRulesFactory.CreateValidationRule<Order>(o => o.Product,
                LogicalOperatorAtTheRootLevel.NotEqual, nullProduct);

            var orderCustomerAndProductCannotBeNullRule =
                ValidationRulesFactory.CreateValidationRule<Order>(ChildrenBindingOperator.AndAlso,
                    new List<Rule> {child1Rule, child2Rule});
            orderCustomerAndProductCannotBeNullRule.RuleError =
                new RuleError {Code = "c2", Message = "Customer and/or Product can't be null"};
            if(orderCustomerAndProductCannotBeNullRule.Compile())
                OrderRules.Add(orderCustomerAndProductCannotBeNullRule);

            var nullStrRule = ConstantRulesFactory.CreateConstantRule<string>("null");
            var child3Rule = ValidationRulesFactory.CreateValidationRule<Order>(o => o.Customer.FirstName,
                LogicalOperatorAtTheRootLevel.NotEqual, nullStrRule);
            var len2Rule = ConstantRulesFactory.CreateConstantRule<int>("2");
            var child4Rule = ValidationRulesFactory.CreateValidationRule<Order>(o => o.Customer.FirstName.Length,
                LogicalOperatorAtTheRootLevel.GreaterThan, len2Rule);

            var orderCustomerFirstNameRule =
                ValidationRulesFactory.CreateValidationRule<Order>(ChildrenBindingOperator.AndAlso,
                    new List<Rule> {child3Rule, child4Rule});
            orderCustomerFirstNameRule.RuleError = new RuleError
            {
                Code = "c3",
                Message = "first name can't be null/empty and has to be 3+ chars long"
            };
            if(orderCustomerFirstNameRule.Compile())
                OrderRules.Add(orderCustomerFirstNameRule);

            var child5Rule = ValidationRulesFactory.CreateValidationRule<Order>(o => o.Customer.LastName,
                LogicalOperatorAtTheRootLevel.NotEqual, nullStrRule);
            var len3Rule = ConstantRulesFactory.CreateConstantRule<int>("3");
            var child6Rule = ValidationRulesFactory.CreateValidationRule<Order>(o => o.Customer.LastName.Length,
                LogicalOperatorAtTheRootLevel.GreaterThan, len3Rule);
            var orderCustomerLastNameRule =
                ValidationRulesFactory.CreateValidationRule<Order>(ChildrenBindingOperator.AndAlso,
                    new List<Rule> {child5Rule, child6Rule});
            orderCustomerLastNameRule.RuleError = new RuleError
            {
                Code = "c4",
                Message = "last name can't be null/empty and has to be 4+ chars long"
            };
            if(orderCustomerLastNameRule.Compile())
                OrderRules.Add(orderCustomerLastNameRule);


            var zeroRule = ConstantRulesFactory.CreateConstantRule<int>("0");
            var child7Rule = ValidationRulesFactory.CreateValidationRule<Order>(o => o.Product.Id,
                LogicalOperatorAtTheRootLevel.GreaterThan, zeroRule);

            var child81Rule = ValidationRulesFactory.CreateValidationRule<Order>(o => o.Product.Name,
                LogicalOperatorAtTheRootLevel.NotEqual, nullStrRule);
            var len4Rule = ConstantRulesFactory.CreateConstantRule<int>("4");
            var child82Rule = ValidationRulesFactory.CreateValidationRule<Order>(o => o.Product.Name.Length,
                LogicalOperatorAtTheRootLevel.GreaterThan, len4Rule);
            var child8Rule = ValidationRulesFactory.CreateValidationRule<Order>(ChildrenBindingOperator.AndAlso,
                new List<Rule> {child81Rule, child82Rule});

            var orderProductIdPositiveOrNameGreaterThan5 =
                ValidationRulesFactory.CreateValidationRule<Order>(ChildrenBindingOperator.OrElse, new List<Rule> {child7Rule, child8Rule });
            orderProductIdPositiveOrNameGreaterThan5.RuleError = new RuleError
            {
                Code = "c5",
                Message = "id must be greater than zero or name has to be non-null and 5+ chars"
            };

            if(orderProductIdPositiveOrNameGreaterThan5.Compile())
                OrderRules.Add(orderProductIdPositiveOrNameGreaterThan5);
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
                if (orderRule is IValidationRule<Order> && !((orderRule as IValidationRule<Order>).IsValid(order)))
                    ruleErrors.Add(orderRule.RuleError);
            }

            Console.WriteLine("Errors found:");
            Console.WriteLine(JsonConvert.SerializeObject(ruleErrors, Formatting.Indented));
            Console.WriteLine("hit any key to end");
            Console.ReadKey();
        }
    }
}
